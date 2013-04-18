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
            var Params = new List<Type>();
            List<Expression> Arguments = new List<Expression>();
                ConsumeWS();
                if (Peek(2) != "()") while (!EOL)
                    {
                        ConsumeWS();
                        if (Peek() == ")") break;
                        Consume();

                        Arguments.Add(ParseBoolean(false));
                        Params.Add(Arguments.Last().Type);
                    }
                else Consume();
                Consume();
                var Candidates = 
                    Included.SelectMany(x => x.ExportedTypes)
                    .Concat(new Type[]{typeof(Autcorlib)})
                    .SelectMany(x => x.GetTypeInfo().GetDeclaredMethods(Keyword))
                    .Concat(DefinedMethods)
                    .Where(x => x.GetParameters().Length == Params.Count && x.IsStatic);

                var Func = Candidates.LastOrDefault(x => x.GetParameters()
                                                        .Select(y => y.ParameterType)
                                                        .SequenceEqual(Params));
                if (Func == default(MethodInfo))
                {
                    Func = Candidates.LastOrDefault();
                    if (Func == default(MethodInfo)) throw new AutoitException(AutoitExceptionType.NOFUNCMATCH, LineNumber, Cursor, Keyword + "(" + Params.Count + " parameters)");
                    return Expression.Call(Func, Arguments.Zip(Func.GetParameters(), (x, y) => x.GetOfType(VarCompilerEngine, VarSynchronisation, y.ParameterType)));
                }
                else return Expression.Call(Func, Arguments);
        }
    }
}
