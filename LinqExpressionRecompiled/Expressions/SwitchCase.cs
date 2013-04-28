namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.SwitchCaseProxy)), __DynamicallyInvokable]
    public sealed class SwitchCase
    {
        private readonly Expression _body;
        private readonly ReadOnlyCollection<Expression> _testValues;

        internal SwitchCase(Expression body, ReadOnlyCollection<Expression> testValues)
        {
            this._body = body;
            this._testValues = testValues;
        }

        [__DynamicallyInvokable]
        public override string ToString()
        {
            return ExpressionStringBuilder.SwitchCaseToString(this);
        }

        [__DynamicallyInvokable]
        public SwitchCase Update(IEnumerable<Expression> testValues, Expression body)
        {
            if ((testValues == this.TestValues) && (body == this.Body))
            {
                return this;
            }
            return Expression.SwitchCase(body, testValues);
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
        public ReadOnlyCollection<Expression> TestValues
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._testValues;
            }
        }
    }
}

