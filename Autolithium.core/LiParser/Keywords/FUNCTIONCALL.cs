using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Autolithium.core
{
    public partial class LiParser
    {
        private Expression ParseKeyword_FUNCTIONCALL(string Keyword)
        {
            var Func = typeof(Autcorlib).GetTypeInfo().DeclaredMethods.FirstOrDefault(x => x.Name == Keyword);
            List<Expression> Arguments = new List<Expression>();
            if (Func != default(MethodInfo))
            {
                ConsumeWS();
                var param = Func.GetParameters();
                if (Peek(2) != "()") while (!EOL)
                    {
                        ConsumeWS();
                        if (Peek() == ")") break;
                        Consume();

                        Arguments.Add(ParseBoolean(false).GetOfType(VarSynchronisation, param[Arguments.Count].ParameterType));
                    }
                else Consume();
                Consume();
                return Expression.Call(Func, Arguments.ToArray());
            }
            return null;
        }
    }
}
