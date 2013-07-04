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
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Autolithium.core
{
    //public delegate void CompileFuncDelegate(FunctionDefinition FDef, LambdaExpression Body);
    public partial class LiParser
    {
        //protected CompileFuncDelegate CompileFunc;
        public void LiCompileFunction()
        {
            foreach (var ID in FunctionIdentifiers)
            {
                GotoLine(ID.Key);
                var FMeta = GetFunctionMeta();
                NextLine();
                var Ret = Expression.Label();
                Contextual.Push(Expression.Goto(Ret, FMeta.ReturnType));
                ExpressionTypeBeam.PushScope();
                foreach (var arg in FMeta.Parameters) ExpressionTypeBeam.CurrentScope.SetVarCached(arg.Name, Expression.Constant(arg.ArgType.DefaultValue(), arg.ArgType), false);
                ExpressionTypeBeam.CurrentScope.DefineFunc(ID.Value, Expression.Lambda(ParseBlock(), ExpressionTypeBeam.PopScope()));
            }
            /*
            return;
            var Matches = Script.Where(x => Regex.IsMatch(x, "^(?:\t| )*func(.*?)$", RegexOptions.IgnoreCase)).ToList();
            var Lines = Matches.Select(x => Array.IndexOf(Script, x));
            List<ParameterExpression> Params = new List<ParameterExpression>();
            foreach (var L in Lines)
            {
                GotoLine(L);
                var FMeta = GetFunctionMeta();
                ExpressionTypeBeam.PushScope();
                List<Expression> PreFunction = new List<Expression>();

                var MyFunc = ExpressionTypeBeam.CurrentScope.Parent.ScopeFunctions.First(x => x.Name == FMeta.Name && x.Parameters.SequenceEqual(FMeta.Parameters) && x.ReturnType == FMeta.ReturnType);

                PreFunction.Add(VarAutExpression.VariableAccess("Return-store", false)
                    .Setter(Expression.Constant(FMeta.ReturnType.DefaultValue(), FMeta.ReturnType)));
                PreFunction.AddRange(
                    MyFunc.Parameters.Select(x =>
                    {
                        var PType = x.GetTypeInfo().IsValueType ? typeof(Nullable<>).MakeGenericType(x) : x;
                        Params.Add(Expression.Parameter(PType));
                        return VarAutExpression.VariableAccess(x.ArgName).Setter(
                            Expression.Coalesce(Params.Last(), x.DefaultValue().ConvertTo(x))
                        );
                    })
                );
                var Ret = Expression.Label();
                Contextual.Push(Expression.Goto(Ret, FMeta.ReturnType));
                
                var Block = ParseBlock();
                
                Block.Add(Expression.Label(Ret));
                Block.Add(VarAutExpression.VariableAccess("Return-store", false).Getter(L.FDef.ReturnType));
                ExpressionTypeBeam.CurrentScope.DefineFunc(, Expression.Lambda(Expression.Block(ExpressionTypeBeam.PopScope(), Block), Params));
                Block.Clear();
                Params.Clear();
            }*/
        }
    }
}
