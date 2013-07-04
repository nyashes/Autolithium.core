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
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using System.Reflection.Emit;
using Autolithium.core;

namespace Autolithium.compiler
{
    public struct AssemblyFunction
    {
        public MethodBuilder Method;
        public Type DelegateType;
        public FieldBuilder DelegateField;
    }
    class AssemblyScope : Autolithium.core.VScope
    {
        private ModuleBuilder Module;
        private TypeBuilder Destination;
        public AssemblyScope(ModuleBuilder M, TypeBuilder T) : base(null)
        {
            this.Module = M;
            this.Destination = T;
        }

        public override object DeclareFunc(string Name, Type Return, IEnumerable<ArgumentMeta> Parameters)
        {
            var DType = Module.CreateDelegateFor(this.Destination.Name + "=&=" + Name, Return, Parameters.Select(x => x.ArgType));
            this.ScopeFunctions.Add(new core.FunctionMeta()
            {
                Name = Name,
                ReturnType = Return,
                Parameters = Parameters,
                Information = new AssemblyFunction()
                {
                    Method = Destination.DefineMethod("&=F" + Name,
                        System.Reflection.MethodAttributes.Static | System.Reflection.MethodAttributes.Public,
                        Return,
                        Parameters.Select(x => x.ArgType).ToArray()),
                    DelegateType = DType,
                    DelegateField = this.Destination.DefineField(Name, DType, System.Reflection.FieldAttributes.Public | System.Reflection.FieldAttributes.Static)
                }
            });
            return this.ScopeFunctions.Count - 1;
        }

        public override Expression CallFunc(FunctionMeta Name, IEnumerable<Expression> Parameters)
        {
            var Info = (AssemblyFunction)Name.Information;
            return Expression.Invoke(
                Expression.MakeMemberAccess(null, Info.DelegateField),
                Parameters);

        }

        public override void DefineFunc(object ID, System.Linq.Expressions.LambdaExpression Corpse)
        {
            Corpse.CompileToMethod(((AssemblyFunction)ScopeFunctions[(int)ID].Information).Method);
        }

        public override void DeclareVar(string Name, Type BaseT)
        {
            ScopeVariables.Add(
                Expression.MakeMemberAccess(null,
                Destination.DefineField(Name, BaseT, System.Reflection.FieldAttributes.Public | System.Reflection.FieldAttributes.Static)));
        }

        public override System.Linq.Expressions.Expression GetVar(string Name, Type Desired)
        {
            if (Desired == null)
                return this.ScopeVariables
                    .Cast<MemberExpression>()
                    .First(x => x.Member.Name == Name && this.OnDateType[Name].Contains(x.Type));

            return this.ScopeVariables
                .Cast<MemberExpression>()
                .First(x => x.Member.Name == Name && x.Type == Desired);
        }

        public override System.Linq.Expressions.Expression SetVar(string Name, System.Linq.Expressions.Expression E)
        {
            return Expression.Assign(this.GetVar(Name, E.Type), E);
        }

        public override bool HasVar(string Name, Type T)
        {
            return this.ScopeVariables
               .Cast<MemberExpression>()
               .Any(x => x.Member.Name == Name && x.Type == T);
        }

        public override bool HasVar(string Name)
        {
            return this.ScopeVariables
               .Cast<MemberExpression>()
               .Any(x => x.Member.Name == Name);
        }
    }
}
