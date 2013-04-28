namespace System.Linq.Expressions
{
    using System;
    using System.Diagnostics;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.DefaultExpressionProxy)), __DynamicallyInvokable]
    public sealed class DefaultExpression : Expression
    {
        private readonly System.Type _type;

        internal DefaultExpression(System.Type type)
        {
            this._type = type;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitDefault(this);
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.Default;
            }
        }

        [__DynamicallyInvokable]
        public sealed override System.Type Type
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._type;
            }
        }
    }
}

