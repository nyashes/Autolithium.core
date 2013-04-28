using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public partial class LiParser
    {
        public Stack<Expression> Contextual = new Stack<Expression>();
        private Expression ParseKeywordOrFunc(string Keyword, Type Desired = null)
        {
            //Expression Element;
            Keyword = Keyword.ToUpper();
            switch (Keyword)
            {
                case "TRUE": return Expression.Constant(true, typeof(bool));
                case "FALSE": return Expression.Constant(false, typeof(bool));
                case "IF": return ParseKeyword_IF(Keyword);
                case "WHILE": return ParseKeyword_WHILE(Keyword);
                case "FOR": return ParseKeyword_FOR(Keyword);
                case "RETURN": return ParseKeyword_RETURN(Keyword);
                case "DO": return ParseKeyword_DO(Keyword);
                case "DEFAULT": return Expression.Constant(null);
                case "ENDIF":
                case "NEXT":
                case "END":
                case "WEND":
                case "ENDWITH":
                case "UNTIL":
                    return null;
                case "EXITLOOP":
                    ConsumeWS();
                    int goback = 1;
                    string str = GetNbr();
                    if (str != "") goback = int.Parse(str);
                    if (Contextual.Count > 1 + (goback - 1) * 2) return Contextual.ElementAt(1 + (goback - 1) * 2);
                    else throw new AutoitException(AutoitExceptionType.EXITLLOOPOUTSIDELOOP, LineNumber, Cursor);
                case "CONTINUELOOP":
                    int goback2 = 1;
                    string str2 = GetNbr();
                    if (str2 != "") goback2 = int.Parse(str2);
                    if (Contextual.Count > (goback2 - 1) * 2) return Contextual.ElementAt((goback2 - 1) * 2);
                    else throw new AutoitException(AutoitExceptionType.EXITLLOOPOUTSIDELOOP, LineNumber, Cursor);
                default: return ParseKeyword_FUNCTIONCALL(Keyword);
            }
        }
        private List<Expression> ParseBlock(bool IsLoop = false)
        {
            Expression Element;
            List<Expression> Instruction = new List<Expression>();
            var Dump = VarCompilerEngine.Save();
            do
            {
                NextLine();
                Element = ParseBoolean();
                if (VarSynchronisation.Count > 0) Instruction.AddRange(VarSynchronisation);
                VarSynchronisation.Clear();
                if (Element != null) Instruction.Add(Element);
            }
            while (Element != null);
            if (IsLoop)
            {
                VarCompilerEngine.Restore(Dump, VarSynchronisation);
                if (VarSynchronisation.Count > 0) Instruction.AddRange(VarSynchronisation);
                VarSynchronisation.Clear();
                Seek();
                ConsumeWS();
            }
            return Instruction;
        }
    }
}
