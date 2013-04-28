namespace System.Linq.Expressions
{
    using System;
    using System.Diagnostics;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.CatchBlockProxy)), __DynamicallyInvokable]
    public sealed class CatchBlock
    {
        private readonly Expression _body;
        private readonly Expression _filter;
        private readonly Type _test;
        private readonly ParameterExpression _var;

        internal CatchBlock(Type test, ParameterExpression variable, Expression body, Expression filter)
        {
            this._test = test;
            this._var = variable;
            this._body = body;
            this._filter = filter;
        }

        [__DynamicallyInvokable]
        public override string ToString()
        {
            return ExpressionStringBuilder.CatchBlockToString(this);
        }

        [__DynamicallyInvokable]
        public CatchBlock Update(ParameterExpression variable, Expression filter, Expression body)
        {
            if (((variable == this.Variable) && (filter == this.Filter)) && (body == this.Body))
            {
                return this;
            }
            return Expression.MakeCatchBlock(this.Test, variable, body, filter);
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
        public Expression Filter
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._filter;
            }
        }

        [__DynamicallyInvokable]
        public Type Test
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._test;
            }
        }

        [__DynamicallyInvokable]
        public ParameterExpression Variable
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._var;
            }
        }
    }
}

