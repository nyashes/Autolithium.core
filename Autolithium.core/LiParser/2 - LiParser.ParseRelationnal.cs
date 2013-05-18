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
        private Expression ParseRelationnal(bool ExpectAssign = true, Type Desired = null)
        {
            ConsumeWS();
            Expression C1, C2;
            char ch;
            C1 = ParseConcat();
            do
            {
                ConsumeWS();
                ch = Read()[0];
                switch (ch)
                {
                    case '<':
                        if (Peek() == ">")
                        {
                            Consume();
                            C2 = ParseConcat().ConvertTo(C1.Type);
                            C1 = Expression.NotEqual(C1, C2);

                        }
                        else if (Peek() == "=")
                        {
                            Consume();
                            C2 = ParseConcat();
                            C1 = Expression.LessThanOrEqual(
                                C1.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)),
                                C2.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)));
                        }
                        else
                        {
                            C2 = ParseConcat();
                            C1 = Expression.LessThan(
                                C1.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)),
                                C2.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)));
                        }

                        continue;
                    case '>':
                        if (Peek() == "=")
                        {
                            Consume();
                            C2 = ParseConcat();
                            C1 = Expression.GreaterThanOrEqual(
                                C1.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)),
                                C2.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)));
                        }
                        else
                        {
                            C2 = ParseConcat();
                            C1 = Expression.GreaterThan(
                                C1.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)),
                                C2.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)));
                        }
                        continue;
                    case '=':
                        if (Peek() == "=")
                        {
                            throw new NotSupportedException("Sorry, No support for ==");
                            C2 = ParseConcat();
                            C1 = Expression.AndAlso(Expression.TypeEqual(C1, C2.Type), Expression.Equal(C1, C2));
                        }
                        else if (ExpectAssign)
                        {
                            if (C1 is VarAutExpression)
                            {
                                Consume();
                                var value = ParseBoolean(false);
                                C1 = (C1 as VarAutExpression).Setter(value);
                            }
                            else throw new AutoitException(AutoitExceptionType.ASSIGNTONOTVARIABLE, LineNumber, Cursor, C1.ToString());
                        }
                        else
                        {
                            C2 = ParseConcat();
                            C1 = Expression.Equal(
                                C1.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)),
                                C2.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)));
                        }
                        continue;
                    case '+':
                        if (Peek() != "=" || !ExpectAssign) break;
                        Consume();
                        if (C1 is VarAutExpression)
                        {
                            ConsumeWS();
                            C2 = ParseBoolean(false);
                            C1 = (C1 as VarAutExpression).Setter(
                                Expression.Add(
                                    (C1 as VarAutExpression).Getter(ExpressionTypeBeam.Largest(C1.Type, C2.Type)), 
                                    C2.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type))
                                )
                            );          
                        }
                        else throw new AutoitException(AutoitExceptionType.ASSIGNTONOTVARIABLE, LineNumber, Cursor, C1.ToString());
                        continue;
                    case '-':
                        if (Peek() != "=" || !ExpectAssign) break;
                        Consume();
                        if (C1 is VarAutExpression)
                        {
                            ConsumeWS();
                            C2 = ParseBoolean(false);
                            C1 = (C1 as VarAutExpression).Setter(
                                Expression.Subtract(
                                    (C1 as VarAutExpression).Getter(ExpressionTypeBeam.Largest(C1.Type, C2.Type)),
                                    C2.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type))
                                )
                            );
                        }
                        else throw new AutoitException(AutoitExceptionType.ASSIGNTONOTVARIABLE, LineNumber, Cursor, C1.ToString());
                        continue;
                    case '*':
                        if (Peek() != "=" || !ExpectAssign) break;
                        Consume();
                        if (C1 is VarAutExpression)
                        {
                            ConsumeWS();
                            C2 = ParseBoolean(false);
                            C1 = (C1 as VarAutExpression).Setter(
                                Expression.Multiply(
                                    (C1 as VarAutExpression).Getter(ExpressionTypeBeam.Largest(C1.Type, C2.Type)),
                                    C2.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type))
                                )
                            );
                        }
                        else throw new AutoitException(AutoitExceptionType.ASSIGNTONOTVARIABLE, LineNumber, Cursor, C1.ToString());
                        continue;
                    case '/':
                        if (Peek() != "=" || !ExpectAssign) break;

                        ConsumeWS();
                        if (C1 is VarAutExpression)
                        {
                            ConsumeWS();
                            C2 = ParseBoolean(false);
                            C1 = (C1 as VarAutExpression).Setter(
                                Expression.Divide(
                                    (C1 as VarAutExpression).Getter(ExpressionTypeBeam.Largest(C1.Type, C2.Type)),
                                    C2.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type))
                                )
                            );
                        }
                        else throw new AutoitException(AutoitExceptionType.ASSIGNTONOTVARIABLE, LineNumber, Cursor, C1.ToString());
                        continue;
                }
                SeekRelative(-1);
                break;
            } while (!EOL);
            return C1;
        }
    }
}
