using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public partial class LiParser
    {
        private Expression Parse_CALL(string Keyword)
        {
            var Params = ParseArgExpList();
            var Use = SelectOverload(Keyword, ref Params);
            if (Use is MethodInfo) return Expression.Call(null, Use, Params);
            else
            {
                var UseReal = Use as FunctionDefinition;
                if (UseReal.DelegateField != null) return Expression.Invoke(Expression.MakeMemberAccess(null, UseReal.DelegateField), Params);
                else throw new NotImplementedException("Sorry, cannot call something else than a delegate field or a static method for now");
            }
        }
    }
}
