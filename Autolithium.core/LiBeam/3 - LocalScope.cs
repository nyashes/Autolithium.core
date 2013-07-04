﻿/*Copyright or © or Copr. THOUVENIN Alexandre (2013)

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
using System.Threading.Tasks;
using System.Reflection;

namespace Autolithium.core
{
    public class LocalScope : VScope
    {
        public LocalScope(IScope Parent = null) : base(Parent) { }

        public override object DeclareFunc(string Name, Type Return, IEnumerable<ArgumentMeta> Parameters) { return this.Parent.DeclareFunc(Name, Return, Parameters); }
        public override Expression CallFunc(FunctionMeta Name, IEnumerable<Expression> Parameters) { return this.Parent.CallFunc(Name, Parameters); }
        public override void DefineFunc(object ID, LambdaExpression Corpse) { this.Parent.DefineFunc(ID, Corpse); }

        public override void DeclareVar(string Name, Type BaseT)
        {
            this.ScopeVariables.Add(Expression.Parameter(BaseT, Name));
            if (!this.OnDateType.ContainsKey(Name)) this.OnDateType.Add(Name, new List<Type>());
            this.OnDateType[Name].Add(BaseT);
        }
        public override Expression GetVar(string Name, Type Desired)
        {
            if (Desired == null) 
                return this.ScopeVariables
                    .Cast<ParameterExpression>()
                    .First(x => x.Name == Name && this.OnDateType[x.Name].Contains(x.Type));
            return this.ScopeVariables
                .Cast<ParameterExpression>()
                .First(x => x.Name == Name && x.Type == Desired);
        }
        public override Expression SetVar(string Name, Expression E)
        {
            return Expression.Assign(this.GetVar(Name, E.Type), E);
        }

        public override bool HasVar(string Name, Type T)
        {
            return this.ScopeVariables
                .Cast<ParameterExpression>()
                .Any(x => x.Name == Name && x.Type == T && OnDateType[Name].Contains(T));
        }
        public override bool HasVar(string Name)
        {
            return this.ScopeVariables
                .Cast<ParameterExpression>()
                .Any(x => x.Name == Name);
        }
    }
}