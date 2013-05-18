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
        private Expression ParseKeyword_RETURN(string Keyword)
        {
            return Expression.Block(Contextual.Peek().Type, 
                VarAutExpression.VariableAccess("Return-store").Setter(ParseBoolean(false)).ConvertTo(Contextual.Peek().Type)
                , Contextual.Peek());
            /*VarSynchronisation.Add();
            return Contextual.Peek();*/
            
        }
    }
}
