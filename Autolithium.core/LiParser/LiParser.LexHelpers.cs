/*Copyright or © or Copr. THOUVENIN Alexandre (2013)

nem-e5i5software@live.fr

This software is a computer program whose purpose is to [describe
functionalities and technical features of your software].

This software is governed by the CeCILL-C license under French law and
abiding by the rules of distribution of free software.  You can  use, 
modify and/ or redistribute the software under the terms of the CeCILL-C
license as circulated by CEA, CNRS and INRIA at the following URL
"http://www.cecill.info". 

As a counterpart to the access to the source code and  rights to copy,
modify and redistribute granted by the license, users are provided only
with a limited warranty  and the software's author,  the holder of the
economic rights,  and the successive licensors  have only  limited
liability. 

In this respect, the user's attention is drawn to the risks associated
with loading,  using,  modifying and/or developing or reproducing the
software by the user in light of its specific status of free software,
that may mean  that it is complicated to manipulate,  and  that  also
therefore means  that it is reserved for developers  and  experienced
professionals having in-depth computer knowledge. Users are therefore
encouraged to load and test the software's suitability as regards their
requirements in conditions enabling the security of their systems and/or 
data to be ensured and,  more generally, to use and operate it in the 
same conditions as regards security. 

The fact that you are presently reading this means that you have had
knowledge of the CeCILL-C license and that you accept its terms.*/

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
        private const string Reg_Any = "[^,() \t\n\r]";

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
                var s = Getstr(Reg_AlphaNum).ToLower();
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
                if (Peek() == "[") { Consume(); ConsumeWS(); if (Peek() == "]") { Consume(); t = t.MakeArrayType(); } }
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

        public void NextLine(int N = 1)
        {
            if (Script != null && LineNumber + N <= Script.Length)
                ScriptLine = Script[(LineNumber+=N) - 1];
            else throw new IndexOutOfRangeException("Cannot move to next line in interactive mode or EOF reached");
            Cursor = 0;
        }
        public void GotoLine(int L)
        {
            if (Script != null && L < Script.Length)
                ScriptLine = Script[(LineNumber = L + 1) - 1];
            else throw new IndexOutOfRangeException("Cannot move to a line in interactive mode or EOF reached");
            Cursor = 0;
        }

        public bool EOL { get { return Cursor >= ScriptLine.Length; } }
        public bool EOF { get { return (Script == null || (LineNumber >= Script.Length && EOL)); } }
        public bool INTERCATIVE { get { return Script == null || Script.Length <= 1; } }
    }
}
