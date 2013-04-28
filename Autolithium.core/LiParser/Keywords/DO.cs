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
        private Expression ParseKeyword_DO(string Keyword)
        {
            if (!INTERCATIVE)
            {
                var @break = Expression.Label("break");
                var @continue = Expression.Label("continue");
                Contextual.Push(Expression.Goto(@break));
                Contextual.Push(Expression.Goto(@continue));
                List<Expression> Instruction = new List<Expression>();
                Instruction.Add(Expression.Label(@continue));
                Instruction.AddRange(ParseBlock(true));
                LineNumber--;
                if (Peek(5).ToUpper() != "UNTIL") throw new AutoitException(AutoitExceptionType.EXPECTUNTIL, LineNumber, Cursor, Getstr(Reg_AlphaNum));
                Consume(5);
                ConsumeWS();
                var Cond = Expression.Not(ParseBoolean(false).GetOfType(VarCompilerEngine, VarSynchronisation, typeof(bool)));
                Instruction.AddRange(VarSynchronisation);
                VarSynchronisation.Clear();
                Instruction.Add(Expression.IfThen(Cond, Expression.Goto(@continue)));
                Instruction.Add(Expression.Label(@break));
                Contextual.Pop();
                Contextual.Pop();
                return Expression.Block(Instruction.ToArray());
            }
            else throw new AutoitException(AutoitExceptionType.MULTILINEININTERACTIVE, LineNumber, Cursor, Keyword);
        }
    }
}
