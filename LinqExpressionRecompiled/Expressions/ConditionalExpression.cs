namespace System.Linq.Expressions
{
    using System;
    using System.Diagnostics;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.ConditionalExpressionProxy)), __DynamicallyInvokable]
    public class ConditionalExpression : Expression
    {
        private readonly Expression _test;
        private readonly Expression _true;

        internal ConditionalExpression(Expression test, Expression ifTrue)
        {
            this._test = test;
            this._true = ifTrue;
        }

        [__DynamicallyInvokable]
        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitConditional(this);
        }

        internal virtual Expression GetFalse()
        {
            return Expression.Empty();
        }

        internal static ConditionalExpression Make(Expression test, Expression ifTrue, Expression ifFalse, System.Type type)
        {
            if ((ifTrue.Type != type) || (ifFalse.Type != type))
            {
                return new FullConditionalExpressionWithType(test, ifTrue, ifFalse, type);
            }
            if ((ifFalse is DefaultExpression) && (ifFalse.Type == typeof(void)))
            {
                return new ConditionalExpression(test, ifTrue);
            }
            return new FullConditionalExpression(test, ifTrue, ifFalse);
        }

        [__DynamicallyInvokable]
        public ConditionalExpression Update(Expression test, Expression ifTrue, Expression ifFalse)
        {
            if (((test == this.Test) && (ifTrue == this.IfTrue)) && (ifFalse == this.IfFalse))
            {
                return this;
            }
            return Expression.Condition(test, ifTrue, ifFalse, this.Type);
        }

        [__DynamicallyInvokable]
        public Expression IfFalse
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.GetFalse();
            }
        }

        [__DynamicallyInvokable]
        public Expression IfTrue
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._true;
            }
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.Conditional;
            }
        }

        [__DynamicallyInvokable]
        public Expression Test
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._test;
            }
        }

        [__DynamicallyInvokable]
        public override System.Type Type
        {
            [__DynamicallyInvokable]
            get
            {
                return this.IfTrue.Type;
            }
        }
    }
}

