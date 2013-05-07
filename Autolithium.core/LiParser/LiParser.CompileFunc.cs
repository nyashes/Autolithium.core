using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public delegate void CompileFuncDelegate(FunctionDefinition FDef, LambdaExpression Body);
    public partial class LiParser
    {
        protected CompileFuncDelegate CompileFunc;
        public void CompileFunction()
        {
            var Matches = Script.Where(x => Regex.IsMatch(x, "^func(.*?)$", RegexOptions.IgnoreCase)).ToList();
            var Lines = Matches.Select(x => 
                new { 
                    Position = Array.IndexOf(Script, x), 
                    FDef = DefinedFunctions.First(y => y.DefinitionSignature == x.GetHashCode()) 
                });
            AutoItVarCompilerEngine VC;
            List<ParameterExpression> Params = new List<ParameterExpression>();
            foreach (var L in Lines)
            {
                GotoLine(L.Position);
                VC = new AutoItVarCompilerEngine();
                VarSynchronisation.Add(VC.Assign("Return-store", Expression.Constant(null, L.FDef.ReturnType)));
                VarSynchronisation.AddRange(
                    L.FDef.MyArguments.Select(x =>
                    {
                        Params.Add(Expression.Parameter(x.MyType));
                        return VC.Assign(x.ArgName,
                            Expression.Coalesce(Params.Last(), x.DefaultValue.GetOfType(VC, VarSynchronisation, Params.Last().Type))
                        );
                    })
                );
                var Ret = Expression.Label();
                Contextual.Push(Expression.Goto(Ret, L.FDef.ReturnType));
                var Block = ParseBlock(VC);
                Block.Add(Expression.Label(Ret));
                Block.Add(VC.Access("Return-store", Block, L.FDef.ReturnType));
                CompileFunc(L.FDef, Expression.Lambda(Expression.Block(VC.DefinedVars, Block), Params));
            }
        }
    }
}
