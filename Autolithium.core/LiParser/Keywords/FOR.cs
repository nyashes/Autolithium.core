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
        private Expression ParseKeyword_FOR(string Keyword)
        {
            Expression InitVal, LastVal, Step;
            string ForVarName;
            ForVarName = (ParsePrimary() as VarAutExpression).Name;
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
            else Step = Expression.Constant(1, typeof(int)).ConvertTo(InitVal.Type);
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
