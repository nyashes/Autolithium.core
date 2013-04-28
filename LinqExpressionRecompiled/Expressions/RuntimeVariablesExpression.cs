namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime;
    using System.Runtime.CompilerServices;

    [DebuggerTypeProxy(typeof(Expression.RuntimeVariablesExpressionProxy)), __DynamicallyInvokable]
    public sealed class RuntimeVariablesExpression : Expression
    {
        private readonly ReadOnlyCollection<ParameterExpression> _variables;

        internal RuntimeVariablesExpression(ReadOnlyCollection<ParameterExpression> variables)
        {
            this._variables = variables;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitRuntimeVariables(this);
        }

        [__DynamicallyInvokable]
        public RuntimeVariablesExpression Update(IEnumerable<ParameterExpression> variables)
        {
            if (variables == this.Variables)
            {
                return this;
            }
            return Expression.RuntimeVariables(variables);
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.RuntimeVariables;
            }
        }

        [__DynamicallyInvokable]
        public sealed override System.Type Type
        {
            [__DynamicallyInvokable]
            get
            {
                return typeof(IRuntimeVariables);
            }
        }

        [__DynamicallyInvokable]
        public ReadOnlyCollection<ParameterExpression> Variables
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._variables;
            }
        }
    }
}

