namespace System.Linq.Expressions
{
    using System;
    using System.Diagnostics;
    using System.Dynamic.Utils;
    using System.Reflection;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(System.Linq.Expressions.Expression.MemberExpressionProxy)), __DynamicallyInvokable]
    public class MemberExpression : System.Linq.Expressions.Expression
    {
        private readonly System.Linq.Expressions.Expression _expression;

        internal MemberExpression(System.Linq.Expressions.Expression expression)
        {
            this._expression = expression;
        }

        [__DynamicallyInvokable]
        protected internal override System.Linq.Expressions.Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitMember(this);
        }

        internal virtual MemberInfo GetMember()
        {
            throw ContractUtils.Unreachable;
        }

        internal static MemberExpression Make(System.Linq.Expressions.Expression expression, MemberInfo member)
        {
            if (member.MemberType == MemberTypes.Field)
            {
                return new FieldExpression(expression, (FieldInfo) member);
            }
            return new PropertyExpression(expression, (PropertyInfo) member);
        }

        [__DynamicallyInvokable]
        public MemberExpression Update(System.Linq.Expressions.Expression expression)
        {
            if (expression == this.Expression)
            {
                return this;
            }
            return System.Linq.Expressions.Expression.MakeMemberAccess(expression, this.Member);
        }

        [__DynamicallyInvokable]
        public System.Linq.Expressions.Expression Expression
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._expression;
            }
        }

        [__DynamicallyInvokable]
        public MemberInfo Member
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.GetMember();
            }
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.MemberAccess;
            }
        }
    }
}

