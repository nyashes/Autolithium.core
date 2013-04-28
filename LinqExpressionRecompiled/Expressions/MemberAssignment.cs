namespace System.Linq.Expressions
{
    using System;
    using System.Reflection;
    using System.Runtime;

    [__DynamicallyInvokable]
    public sealed class MemberAssignment : MemberBinding
    {
        private System.Linq.Expressions.Expression _expression;

        internal MemberAssignment(MemberInfo member, System.Linq.Expressions.Expression expression) : base(MemberBindingType.Assignment, member)
        {
            this._expression = expression;
        }

        [__DynamicallyInvokable]
        public MemberAssignment Update(System.Linq.Expressions.Expression expression)
        {
            if (expression == this.Expression)
            {
                return this;
            }
            return System.Linq.Expressions.Expression.Bind(base.Member, expression);
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
    }
}

