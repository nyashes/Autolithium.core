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

namespace Autolithium.core
{
    public class AutExpression : Expression
    {
        public AutExpressionType ExpressionType;
        
        public static AutExpression EndOfBlock()
        {
            return new AutExpression()
            {
                ExpressionType = AutExpressionType.EndOfBlock
            };
        }
        public override bool CanReduce
        {
            get
            {
                return false;
            }
        }
        protected override Expression Accept(ExpressionVisitor visitor)
        {
            return this;
        }
        public override Expression Reduce()
        {
            return this;
        }
        protected override Expression VisitChildren(ExpressionVisitor visitor)
        {
            return this;
        }
        public override ExpressionType NodeType
        {
            get
            {
                return System.Linq.Expressions.ExpressionType.Parameter;
            }
        }
        public override Type Type
        {
            get
            {
                return typeof(object);
            }
        }

        public static VarAutExpression VariableAccess(string Name, bool? IsGlobal = null, Expression Index = null, Type UnboxTo = null)
        {

            var n = new VarAutExpression()
            {
                ExpressionType = (IsGlobal == null) ? AutExpressionType.Variable : IsGlobal ?? true ? AutExpressionType.GlobalVariable : AutExpressionType.LocalVariable,
                Name = Name,
                Index = Index == null ? null : new List<Expression>() {Index},
                UnboxTo = UnboxTo
            };
            return n;
        }
        public static VarAutExpression VariableAccess(string Name, bool? IsGlobal = null, Type UnboxTo = null, params Expression[] Index)
        {

            var n = new VarAutExpression()
            {
                ExpressionType = (IsGlobal == null) ? AutExpressionType.Variable : IsGlobal ?? true ? AutExpressionType.GlobalVariable : AutExpressionType.LocalVariable,
                Name = Name,
                Index = Index.Length <= 0 ? null : Index.ToList(),
                UnboxTo = UnboxTo
            };
            return n;
        }
    }

    public class VarAutExpression : AutExpression
    {
        public string Name;
        public Type UnboxTo;
        public List<Expression> Index;
        public bool? IsGlobal
        {
            get { return this.ExpressionType == AutExpressionType.GlobalVariable ? true : 
                this.ExpressionType == AutExpressionType.LocalVariable ? false : 
                (bool?)null; }
        }

        public VarAutExpression() : base() { }

        public static bool operator ==(VarAutExpression v1, VarAutExpression v2)
        {
            return v1.Name == v2.Name
                /*&& v1.Index == v2.Index*/;
        }
        public static bool operator !=(VarAutExpression v1, VarAutExpression v2)
        {
            return !(v1 == v2);
        }
        public override int GetHashCode()
        {
            //if (this.Index != null) return this.Name.GetHashCode() ^ this.Index.GetHashCode();
            /*else*/ return this.Name.GetHashCode();
        }
        public override bool Equals(object obj)
        {
            return obj.GetType() == typeof(VarAutExpression) ? this == (VarAutExpression)obj : this == obj;
        }
    }

    public enum AutExpressionType
    {
        EndOfBlock,
        Variable,
        GlobalVariable,
        LocalVariable
    }
}
