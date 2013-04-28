namespace System.Linq.Expressions
{
    using System;
    using System.Diagnostics;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.LabelExpressionProxy)), __DynamicallyInvokable]
    public sealed class LabelExpression : Expression
    {
        private readonly Expression _defaultValue;
        private readonly LabelTarget _target;

        internal LabelExpression(LabelTarget label, Expression defaultValue)
        {
            this._target = label;
            this._defaultValue = defaultValue;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitLabel(this);
        }

        [__DynamicallyInvokable]
        public LabelExpression Update(LabelTarget target, Expression defaultValue)
        {
            if ((target == this.Target) && (defaultValue == this.DefaultValue))
            {
                return this;
            }
            return Expression.Label(target, defaultValue);
        }

        [__DynamicallyInvokable]
        public Expression DefaultValue
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._defaultValue;
            }
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.Label;
            }
        }

        [__DynamicallyInvokable]
        public LabelTarget Target
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._target;
            }
        }

        [__DynamicallyInvokable]
        public sealed override System.Type Type
        {
            [__DynamicallyInvokable]
            get
            {
                return this._target.Type;
            }
        }
    }
}

