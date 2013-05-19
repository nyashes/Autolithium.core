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
