namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.TryExpressionProxy)), __DynamicallyInvokable]
    public sealed class TryExpression : Expression
    {
        private readonly Expression _body;
        private readonly Expression _fault;
        private readonly Expression _finally;
        private readonly ReadOnlyCollection<CatchBlock> _handlers;
        private readonly System.Type _type;

        internal TryExpression(System.Type type, Expression body, Expression @finally, Expression fault, ReadOnlyCollection<CatchBlock> handlers)
        {
            this._type = type;
            this._body = body;
            this._handlers = handlers;
            this._finally = @finally;
            this._fault = fault;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitTry(this);
        }

        [__DynamicallyInvokable]
        public TryExpression Update(Expression body, IEnumerable<CatchBlock> handlers, Expression @finally, Expression fault)
        {
            if (((body == this.Body) && (handlers == this.Handlers)) && ((@finally == this.Finally) && (fault == this.Fault)))
            {
                return this;
            }
            return Expression.MakeTry(this.Type, body, @finally, fault, handlers);
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
        public Expression Fault
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._fault;
            }
        }

        [__DynamicallyInvokable]
        public Expression Finally
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._finally;
            }
        }

        [__DynamicallyInvokable]
        public ReadOnlyCollection<CatchBlock> Handlers
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._handlers;
            }
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.Try;
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

