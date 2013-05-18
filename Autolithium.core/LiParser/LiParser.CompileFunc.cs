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
        public void LiCompileFunction()
        {
            var Matches = Script.Where(x => Regex.IsMatch(x, "^func(.*?)$", RegexOptions.IgnoreCase)).ToList();
            var Lines = Matches.Select(x => 
                new { 
                    Position = Array.IndexOf(Script, x), 
                    FDef = DefinedFunctions.First(y => y.DefinitionSignature == x.GetHashCode()) 
                });
            List<ParameterExpression> Params = new List<ParameterExpression>();
            foreach (var L in Lines)
            {
                GotoLine(L.Position);
                ExpressionTypeBeam.PushScope();
                List<Expression> VarSynchronisation = new List<Expression>();
                VarSynchronisation.Add(VarAutExpression.VariableAccess("Return-store")
                    .Setter(Expression.Constant(null, L.FDef.ReturnType)));
                VarSynchronisation.AddRange(
                    L.FDef.MyArguments.Select(x =>
                    {
                        Params.Add(Expression.Parameter(x.MyType));
                        return VarAutExpression.VariableAccess(x.ArgName).Setter(
                            Expression.Coalesce(Params.Last(), x.DefaultValue.ConvertTo(Params.Last().Type))
                        );
                    })
                );
                var Ret = Expression.Label();
                Contextual.Push(Expression.Goto(Ret, L.FDef.ReturnType));
                
                var Block = ParseBlock();
                Block = VarSynchronisation.Concat(Block).ToList();
                Block.Add(Expression.Label(Ret));
                Block.Add(VarAutExpression.VariableAccess("Return-store", false).Getter(L.FDef.ReturnType));
                CompileFunc(L.FDef, Expression.Lambda(Expression.Block(ExpressionTypeBeam.PopScope(), Block), Params));
            }
        }
    }
}
