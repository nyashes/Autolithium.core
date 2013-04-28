namespace System.Linq.Expressions
{
    using System;
    using System.Diagnostics;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.GotoExpressionProxy)), __DynamicallyInvokable]
    public sealed class GotoExpression : Expression
    {
        private readonly GotoExpressionKind _kind;
        private readonly LabelTarget _target;
        private readonly System.Type _type;
        private readonly Expression _value;

        internal GotoExpression(GotoExpressionKind kind, LabelTarget target, Expression value, System.Type type)
        {
            this._kind = kind;
            this._value = value;
            this._target = target;
            this._type = type;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitGoto(this);
        }

        [__DynamicallyInvokable]
        public GotoExpression Update(LabelTarget target, Expression value)
        {
            if ((target == this.Target) && (value == this.Value))
            {
                return this;
            }
            return Expression.MakeGoto(this.Kind, target, value, this.Type);
        }

        [__DynamicallyInvokable]
        public GotoExpressionKind Kind
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._kind;
            }
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.Goto;
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
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._type;
            }
        }

        [__DynamicallyInvokable]
        public Expression Value
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._value;
            }
        }
    }
}

