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
using System.Threading.Tasks;
using System.Reflection;

namespace Autolithium.core
{
    public abstract class VScope : IScope
    {
        public IDictionary<string, List<Type>> OnDateType
        {
            get;
            protected internal set;
        }
        public IList<Expression> ScopeVariables
        {
            get;
            protected internal set;
        }
        public IList<FunctionMeta> ScopeFunctions
        {
            get;
            protected internal set;
        }
        public IScope Parent
        {
            get;
            set;
        }

        protected VScope(IScope Parent)
        {
            this.OnDateType = new Dictionary<string, List<Type>>();
            this.ScopeVariables = new List<Expression>();
            this.ScopeFunctions = new List<FunctionMeta>();
            this.Parent = Parent;
        }
        public LocalScope Copy()
        {
            LocalScope Ret = new LocalScope();
            Ret.ScopeVariables = this.ScopeVariables.ToList();
            Ret.OnDateType = this.OnDateType.ToDictionary(x => x.Key, x => x.Value);
            Ret.Parent = this.Parent;
            return Ret;
        }

        public abstract object DeclareFunc(string Name, Type Return, IEnumerable<ArgumentMeta> Parameters);
        public abstract Expression CallFunc(FunctionMeta Name, IEnumerable<Expression> Parameters);
        public abstract void DefineFunc(object ID, LambdaExpression Corpse);

        public abstract void DeclareVar(string Name, Type BaseT);
        public abstract Expression GetVar(string Name, Type Desired);
        public abstract Expression SetVar(string Name, Expression E);
        public abstract bool HasVar(string Name, Type T);
        public abstract bool HasVar(string Name);

        public IEnumerator<IScope> GetEnumerator()
        {
            yield return this;
            if (this.Parent != null) foreach (var E in this.Parent) yield return E;
            yield break;
        }
        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
