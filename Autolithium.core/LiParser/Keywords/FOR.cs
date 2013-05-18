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
        private Expression ParseKeyword_FOR(string Keyword)
        {
            Expression InitVal, LastVal, Step;
            string ForVarName;
            ForVarName = (ParsePrimary() as ParameterExpression).Name;
            ConsumeWS();
            if (Peek(1).ToUpper() != "=") throw new AutoitException(AutoitExceptionType.FORWITHOUTTO, LineNumber, Cursor);
            Consume(); ConsumeWS();
            InitVal = ParseBoolean().ConvertToNumber();
            ConsumeWS();
            if (Peek(2).ToUpper() != "TO") throw new AutoitException(AutoitExceptionType.FORWITHOUTTO, LineNumber, Cursor);
            Consume(2); ConsumeWS();
            LastVal = ParseBoolean().ConvertTo(InitVal.Type);
            ConsumeWS();
            if (Peek(4).ToUpper() == "STEP")
            {
                Consume(4); ConsumeWS();
                Step = ParseBoolean().ConvertTo(InitVal.Type);
            }
            else Step = Expression.Constant(1, InitVal.Type);
            if (!INTERCATIVE)
            {
                var @break = Expression.Label("break");
                var @continue = Expression.Label("continue");
                var @DownTo = VarAutExpression.VariableAccess("downto-" + Contextual.Count, false);
                var @LoopVar = VarAutExpression.VariableAccess(ForVarName, false);
                Contextual.Push(Expression.Goto(@break));
                Contextual.Push(Expression.Goto(@continue));
                List<Expression> Instruction = new List<Expression>();
                if (Step is ConstantExpression)
                    Instruction.Add(@DownTo.Setter((ConstantExpression)Expression.Constant((dynamic)(Step as ConstantExpression).Value < 0, typeof(bool))));
                else Instruction.Add(@DownTo.Setter(Expression.LessThan(Step, Expression.Constant(0, InitVal.Type))));
                Instruction.Add(@LoopVar.Setter(InitVal));
                Instruction.Add(Expression.Label(@continue));
                Instruction.Add(Expression.IfThen(
                    Expression.OrElse(
                        Expression.AndAlso(
                            @DownTo.Getter(typeof(bool)),
                            Expression.LessThan(@LoopVar.Getter(InitVal.Type), LastVal)),
                        Expression.AndAlso(
                            Expression.Not(@DownTo.Getter(typeof(bool))),
                            Expression.GreaterThan(@LoopVar.Getter(InitVal.Type), LastVal)))
                    , Expression.Goto(@break)));
                Instruction.AddRange(ParseBlock(true));
                Instruction.Add(Expression.Assign(
                    @LoopVar.Getter(InitVal.Type),
                    Expression.Add(@LoopVar.Getter(InitVal.Type), Step)));
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
