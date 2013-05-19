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
