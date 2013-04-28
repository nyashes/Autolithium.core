namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(System.Linq.Expressions.Expression.InvocationExpressionProxy)), __DynamicallyInvokable]
    public sealed class InvocationExpression : System.Linq.Expressions.Expression, IArgumentProvider
    {
        private IList<System.Linq.Expressions.Expression> _arguments;
        private readonly System.Linq.Expressions.Expression _lambda;
        private readonly System.Type _returnType;

        internal InvocationExpression(System.Linq.Expressions.Expression lambda, IList<System.Linq.Expressions.Expression> arguments, System.Type returnType)
        {
            this._lambda = lambda;
            this._arguments = arguments;
            this._returnType = returnType;
        }

        protected internal override System.Linq.Expressions.Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitInvocation(this);
        }

        internal InvocationExpression Rewrite(System.Linq.Expressions.Expression lambda, System.Linq.Expressions.Expression[] arguments)
        {
            return System.Linq.Expressions.Expression.Invoke(lambda, arguments ?? this._arguments);
        }

        System.Linq.Expressions.Expression IArgumentProvider.GetArgument(int index)
        {
            return this._arguments[index];
        }

        [__DynamicallyInvokable]
        public InvocationExpression Update(System.Linq.Expressions.Expression expression, IEnumerable<System.Linq.Expressions.Expression> arguments)
        {
            if ((expression == this.Expression) && (arguments == this.Arguments))
            {
                return this;
            }
            return System.Linq.Expressions.Expression.Invoke(expression, arguments);
        }

        [__DynamicallyInvokable]
        public ReadOnlyCollection<System.Linq.Expressions.Expression> Arguments
        {
            [__DynamicallyInvokable]
            get
            {
                return System.Linq.Expressions.Expression.ReturnReadOnly<System.Linq.Expressions.Expression>(ref this._arguments);
            }
        }

        [__DynamicallyInvokable]
        public System.Linq.Expressions.Expression Expression
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._lambda;
            }
        }

        internal LambdaExpression LambdaOperand
        {
            get
            {
                if (this._lambda.NodeType != ExpressionType.Quote)
                {
                    return (this._lambda as LambdaExpression);
                }
                return (LambdaExpression) ((UnaryExpression) this._lambda).Operand;
            }
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.Invoke;
            }
        }

        int IArgumentProvider.ArgumentCount
        {
            get
            {
                return this._arguments.Count;
            }
        }

        [__DynamicallyInvokable]
        public sealed override System.Type Type
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._returnType;
            }
        }
    }
}

