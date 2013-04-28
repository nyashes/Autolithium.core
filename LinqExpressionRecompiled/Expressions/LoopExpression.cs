namespace System.Linq.Expressions
{
    using System;
    using System.Diagnostics;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.LoopExpressionProxy)), __DynamicallyInvokable]
    public sealed class LoopExpression : Expression
    {
        private readonly Expression _body;
        private readonly LabelTarget _break;
        private readonly LabelTarget _continue;

        internal LoopExpression(Expression body, LabelTarget @break, LabelTarget @continue)
        {
            this._body = body;
            this._break = @break;
            this._continue = @continue;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitLoop(this);
        }

        [__DynamicallyInvokable]
        public LoopExpression Update(LabelTarget breakLabel, LabelTarget continueLabel, Expression body)
        {
            if (((breakLabel == this.BreakLabel) && (continueLabel == this.ContinueLabel)) && (body == this.Body))
            {
                return this;
            }
            return Expression.Loop(body, breakLabel, continueLabel);
        }

        [__DynamicallyInvokable]
        public Expression Body
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._body;
            }
        }

        [__DynamicallyInvokable]
        public LabelTarget BreakLabel
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._break;
            }
        }

        [__DynamicallyInvokable]
        public LabelTarget ContinueLabel
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._continue;
            }
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.Loop;
            }
        }

        [__DynamicallyInvokable]
        public sealed override System.Type Type
        {
            [__DynamicallyInvokable]
            get
            {
                if (this._break != null)
                {
                    return this._break.Type;
                }
                return typeof(void);
            }
        }
    }
}

