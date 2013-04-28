namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime;

    [__DynamicallyInvokable]
    public sealed class ElementInit : IArgumentProvider
    {
        private MethodInfo _addMethod;
        private ReadOnlyCollection<Expression> _arguments;

        internal ElementInit(MethodInfo addMethod, ReadOnlyCollection<Expression> arguments)
        {
            this._addMethod = addMethod;
            this._arguments = arguments;
        }

        Expression IArgumentProvider.GetArgument(int index)
        {
            return this._arguments[index];
        }

        [__DynamicallyInvokable]
        public override string ToString()
        {
            return ExpressionStringBuilder.ElementInitBindingToString(this);
        }

        [__DynamicallyInvokable]
        public ElementInit Update(IEnumerable<Expression> arguments)
        {
            if (arguments == this.Arguments)
            {
                return this;
            }
            return Expression.ElementInit(this.AddMethod, arguments);
        }

        [__DynamicallyInvokable]
        public MethodInfo AddMethod
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._addMethod;
            }
        }

        [__DynamicallyInvokable]
        public ReadOnlyCollection<Expression> Arguments
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._arguments;
            }
        }

        int IArgumentProvider.ArgumentCount
        {
            get
            {
                return this._arguments.Count;
            }
        }
    }
}

