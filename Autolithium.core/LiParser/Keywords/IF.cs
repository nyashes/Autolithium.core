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
        private Expression ParseKeyword_IF(string Keyword)
        {
            var Element = ParseBoolean(false).ConvertTo(typeof(bool));
            ConsumeWS();
            if (Peek(4).ToUpper() == "THEN")
            {
                Consume(4);
                ConsumeWS();
                if (!EOL)
                {
                    var Exp = ParseBoolean();
                    NextLine(); ConsumeWS();
                    if (Peek(6).ToUpper() == "ELSEIF") { Consume(6); return Expression.IfThenElse(Element, Exp, ParseKeyword_IF(Keyword)); }
                    else if (Peek(4).ToUpper() == "ELSE") { Consume(4); return Expression.IfThenElse(Element, Exp, ParseBoolean()); }
                    else { NextLine(-1); return Expression.IfThen(Element, Exp); }
                }
                else if (!INTERCATIVE)
                {
                    Expression Condition = Element;
                    List<Expression> Instruction = new List<Expression>();
                    Instruction.AddRange(ParseBlock());
                    Seek();
                    ConsumeWS();
                    var s = Getstr(Reg_AlphaNum).ToUpper();
                    switch (s)
                    {
                        case "ELSE":
                            List<Expression> Otherwise = new List<Expression>();
                            Otherwise.AddRange(ParseBlock());
                            Seek();
                            ConsumeWS();
                            return Expression.IfThenElse(Condition, Expression.Block(Instruction.ToArray()), Expression.Block(Otherwise.ToArray()));
                        case "ELSEIF":
                            return Expression.IfThenElse(Condition, Expression.Block(Instruction.ToArray()), ParseKeywordOrFunc("IF"));
                        default:
                            return Expression.IfThen(Condition, Expression.Block(Instruction.ToArray()));
                    }
                }
                else throw new AutoitException(AutoitExceptionType.MULTILINEININTERACTIVE, LineNumber, Cursor, Keyword);
            }
            else throw new AutoitException(AutoitExceptionType.MISSINGTHEN, LineNumber, Cursor, ScriptLine);
        }
    }
}
