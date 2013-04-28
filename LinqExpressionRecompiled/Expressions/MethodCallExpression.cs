namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Dynamic.Utils;
    using System.Reflection;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.MethodCallExpressionProxy)), __DynamicallyInvokable]
    public class MethodCallExpression : Expression, IArgumentProvider
    {
        private readonly MethodInfo _method;

        internal MethodCallExpression(MethodInfo method)
        {
            this._method = method;
        }

        [__DynamicallyInvokable]
        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitMethodCall(this);
        }

        internal virtual Expression GetInstance()
        {
            return null;
        }

        internal virtual ReadOnlyCollection<Expression> GetOrMakeArguments()
        {
            throw ContractUtils.Unreachable;
        }

        internal virtual MethodCallExpression Rewrite(Expression instance, IList<Expression> args)
        {
            throw ContractUtils.Unreachable;
        }

        Expression IArgumentProvider.GetArgument(int index)
        {
            throw ContractUtils.Unreachable;
        }

        [__DynamicallyInvokable]
        public MethodCallExpression Update(Expression @object, IEnumerable<Expression> arguments)
        {
            if ((@object == this.Object) && (arguments == this.Arguments))
            {
                return this;
            }
            return Expression.Call(@object, this.Method, arguments);
        }

        [__DynamicallyInvokable]
        public ReadOnlyCollection<Expression> Arguments
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.GetOrMakeArguments();
            }
        }

        [__DynamicallyInvokable]
        public MethodInfo Method
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._method;
            }
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.Call;
            }
        }

        [__DynamicallyInvokable]
        public Expression Object
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this.GetInstance();
            }
        }

        int IArgumentProvider.ArgumentCount
        {
            get
            {
                throw ContractUtils.Unreachable;
            }
        }

        [__DynamicallyInvokable]
        public sealed override System.Type Type
        {
            [__DynamicallyInvokable]
            get
            {
                return this._method.ReturnType;
            }
        }
    }
}

