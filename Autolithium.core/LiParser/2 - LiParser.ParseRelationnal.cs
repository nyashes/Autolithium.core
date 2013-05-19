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
