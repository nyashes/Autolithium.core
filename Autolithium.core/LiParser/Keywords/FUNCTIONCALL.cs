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
        //For some reason I need to desactivate the check of Expression.Call so I reflect IT for internal access
        private static TypeInfo MethodCallExpressionN = Expression.Call(null, typeof(LiParser).GetRuntimeMethod("Dummy", new Type[] {})).GetType().GetTypeInfo();
        private static TypeInfo InstanceMethodCallExpressionN = Expression.Call(Expression.Constant(null, typeof(LiParser)), typeof(LiParser).GetRuntimeMethod("Dummy2", new Type[] { })).GetType().GetTypeInfo();
        private static ConstructorInfo CreateMethodCallExpression = MethodCallExpressionN.DeclaredConstructors.First();
        private static ConstructorInfo CreateInstanceMethodCallExpression = InstanceMethodCallExpressionN.DeclaredConstructors.First();
        public static void Dummy() { }
        public void Dummy2() { }
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
                    .Concat(new Type[] { typeof(Autcorlib) })
                    .SelectMany(x => x.GetTypeInfo().GetDeclaredMethods(Keyword))
                    .Where(x => x.GetParameters().Length == Params.Count && x.IsStatic);

                var Func = Candidates.LastOrDefault(x => x.GetParameters()
                                                        .Select(y => y.ParameterType)
                                                        .SequenceEqual(Params));
                if (Func == default(MethodInfo))
                {
                    Func = Candidates.LastOrDefault();
                    if (Func == default(MethodInfo))
                    {
                        var FuncInfo = DefinedMethods.Where(x => x.MyName.ToUpper() == Keyword.ToUpper() && x.MyArguments.Count >= Params.Count)
                            .LastOrDefault();
                        if (FuncInfo == default(FunctionDefinition)) throw new AutoitException(AutoitExceptionType.NOFUNCMATCH, LineNumber, Cursor, Keyword + "(" + Params.Count + " parameters)");
                        /*return (MethodCallExpression)CreateInstanceMethodCallExpression.Invoke(new object[]{FuncInfo.Body, Expression.Field(null, FuncInfo.Instance), FuncInfo.MyArguments.Count != 0 ?
                            Arguments.Zip(FuncInfo.MyArguments, 
                                (x, y) => 
                                    x.GetOfType(VarCompilerEngine, VarSynchronisation, y.MyType)).ToList() : new List<Expression>()});*/
                        return Expression.Invoke(Expression.Field(null, FuncInfo.DelegateField), Arguments
                            .Concat(FuncInfo.MyArguments.Skip(Params.Count).Select(x => Expression.Constant(null)))
                            .Zip(FuncInfo.MyArguments, 
                                (x, y) => 
                                    x.GetOfType(VarCompilerEngine, VarSynchronisation, y.MyType)));
                        /*return Expression.Call(null, typeof(Autcorlib).GetTypeInfo().DeclaredMethods.First(x => x.Name == "CALL"), 
                            Expression.Constant(Keyword, typeof(string)),
                            Expression.NewArrayInit(typeof(object), Arguments.Zip(FuncInfo.MyArguments, 
                                (x, y) => 
                                    x.GetOfType(VarCompilerEngine, VarSynchronisation, y.MyType)).ToList()));*/
                    }
                    else return Expression.Call(Func, Arguments.Zip(Func.GetParameters(), (x, y) => x.GetOfType(VarCompilerEngine, VarSynchronisation, y.ParameterType)));
                }
                else return Expression.Call(Func, Arguments);
        }
    }
}
