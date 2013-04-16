using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public partial class LiParser
    {
        private const string Reg_AlphaNum = @"[a-zA-Z0-9_]";

        #region Fields
        private string ScriptLine;
        private string[] Script;
        private int Cursor = 0;
        public int LineNumber;
        
        #endregion

        #region Data block reading
        private string Lexer_CSString()
        {
            string Ret = "";
            string f = Read();
            while (Peek() != f || (Peek(2) == (f + f))) Ret += Read();
            Consume();
            return Ret;
        }
        private string Getstr(string reg)
        {
            string r = "";
            while (!EOL && System.Text.RegularExpressions.Regex.IsMatch(Peek(), reg)) r += Read();
            return r;
        }
        private string GetNbr()
        {
            var m = System.Text.RegularExpressions.Regex.Match(ScriptLine.Substring(Cursor), @"^-?\d*(\.\d*)?");
            Cursor += m.Length;
            return m.Value;
        }
        private bool TryParseSubscript(out Expression e)
        {
            Expression Ret;
            ConsumeWS();
            char ch = Read()[0];
            if (ch == '[')
            {
                if (Peek() == "]")
                {
                    e = null;
                    return true;
                }
                Ret = ParseBoolean();
                if (Peek() != "]") throw new AutoitException(AutoitExceptionType.UNBALANCEDSUBSCRIPT, LineNumber, Cursor);
                Consume();
                e = Ret;
                return true;
            }
            else
            {
                SeekRelative(-1);
                e = null;
                return false;
            }
        }
        private bool TryParseCast(out Type t)
        {
            if (Peek(2) != "::")
            {
                t = null;
                return false;
            }
            else
            {
                Consume(2);
                var s = Getstr(Reg_AlphaNum);
                switch (s)
                {
                    case "float":
                    case "double":
                    case "real":
                        t = typeof(double);
                        break;
                    case "long":
                        t = typeof(long);
                        break;
                    case "int":
                        t = typeof(int);
                        break;
                    case "string":
                        t = typeof(string);
                        break;
                    case "bool":
                    case "boolean":
                        t = typeof(bool);
                        break;
                    default:
                        t = typeof(object);
                        break;
                }
                return true;
            }
                
        }
        #endregion

        public string Peek(int ChrCount = 1)
        {
            if (Cursor + ChrCount > ScriptLine.Length) return new string('\0', ChrCount);
            return ScriptLine.Substring(Cursor, ChrCount);
        }
        public string Read(int ChrCount = 1)
        {
            var s = Peek(ChrCount);
            Consume(ChrCount);
            return s;
        }
        public void Consume(int ChrCount = 1)
        {
            Cursor += ChrCount;
        }
        public void ConsumeWS()
        {
            while (!EOL && (Peek() == " " || Peek() == "\t")) Cursor++;
        }
        
        public void Seek(int Pos = 0)
        {
            Cursor = Pos;
        }
        public void SeekRelative(int Offset = 0)
        {
            Cursor += Offset;
        }

        public void NextLine()
        {
            if (Script != null && LineNumber < Script.Length)
                ScriptLine = Script[LineNumber++];
            else throw new IndexOutOfRangeException("Cannot move to next line in interactive mode or EOF reached");
            Cursor = 0;
        }

        public bool EOL { get { return Cursor >= ScriptLine.Length; } }
        public bool EOF { get { return (Script == null || LineNumber >= Script.Length); } }
        public bool INTERCATIVE { get { return Script == null || Script.Length <= 1; } }
    }
}
