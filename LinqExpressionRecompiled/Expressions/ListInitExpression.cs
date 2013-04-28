namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.ListInitExpressionProxy)), __DynamicallyInvokable]
    public sealed class ListInitExpression : Expression
    {
        private readonly ReadOnlyCollection<ElementInit> _initializers;
        private readonly System.Linq.Expressions.NewExpression _newExpression;

        internal ListInitExpression(System.Linq.Expressions.NewExpression newExpression, ReadOnlyCollection<ElementInit> initializers)
        {
            this._newExpression = newExpression;
            this._initializers = initializers;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitListInit(this);
        }

        [__DynamicallyInvokable]
        public override Expression Reduce()
        {
            return MemberInitExpression.ReduceListInit(this._newExpression, this._initializers, true);
        }

        [__DynamicallyInvokable]
        public ListInitExpression Update(System.Linq.Expressions.NewExpression newExpression, IEnumerable<ElementInit> initializers)
        {
            if ((newExpression == this.NewExpression) && (initializers == this.Initializers))
            {
                return this;
            }
            return Expression.ListInit(newExpression, initializers);
        }

        [__DynamicallyInvokable]
        public override bool CanReduce
        {
            [__DynamicallyInvokable]
            get
            {
                return true;
            }
        }

        [__DynamicallyInvokable]
        public ReadOnlyCollection<ElementInit> Initializers
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._initializers;
            }
        }

        [__DynamicallyInvokable]
        public System.Linq.Expressions.NewExpression NewExpression
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._newExpression;
            }
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.ListInit;
            }
        }

        [__DynamicallyInvokable]
        public sealed override System.Type Type
        {
            [__DynamicallyInvokable]
            get
            {
                return this._newExpression.Type;
            }
        }
    }
}

