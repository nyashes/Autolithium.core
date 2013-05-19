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
                VarSynchronisation.Add(VarAutExpression.VariableAccess("Return-store", false)
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
