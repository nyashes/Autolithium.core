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
        private Expression ParseKeyword_WHILE(string Keyword)
        {
            var Element = ParseBoolean(false).GetOfType(VarCompilerEngine, VarSynchronisation, typeof(bool));
            ConsumeWS();
            if (!EOL)
            {
                var @break = Expression.Label();
                var @continue = Expression.Label();
                return Expression.Block(
                    Expression.Label(@continue),
                    Expression.IfThen(Expression.Not(Element), Expression.Goto(@break)),
                    ParseBoolean(),
                    Expression.Goto(@continue),
                    Expression.Label(@break));
            }
            else if (!INTERCATIVE)
            {
                var @break = Expression.Label("break");
                var @continue = Expression.Label("continue");
                Contextual.Push(Expression.Goto(@break));
                Contextual.Push(Expression.Goto(@continue));
                List<Expression> Instruction = new List<Expression>();
                Instruction.Add(Expression.Label(@continue));
                Instruction.Add(Expression.IfThen(Expression.Not(Element), Expression.Goto(@break)));
                Instruction.AddRange(ParseBlock(true));
                Instruction.Add(Expression.Goto(@continue));
                Instruction.Add(Expression.Label(@break));
                Contextual.Pop();
                Contextual.Pop();
                return Expression.Block(Instruction.ToArray());
            }
            else throw new AutoitException(AutoitExceptionType.MULTILINEININTERACTIVE, LineNumber, Cursor, Keyword);
        }
    }
}
