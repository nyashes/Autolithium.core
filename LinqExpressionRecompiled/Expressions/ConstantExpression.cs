namespace System.Linq.Expressions
{
    using System;
    using System.Diagnostics;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.ConstantExpressionProxy)), __DynamicallyInvokable]
    public class ConstantExpression : Expression
    {
        private readonly object _value;

        internal ConstantExpression(object value)
        {
            this._value = value;
        }

        [__DynamicallyInvokable]
        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitConstant(this);
        }

        internal static ConstantExpression Make(object value, System.Type type)
        {
            if (((value != null) || (type != typeof(object))) && ((value == null) || !(value.GetType() == type)))
            {
                return new TypedConstantExpression(value, type);
            }
            return new ConstantExpression(value);
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.Constant;
            }
        }

        [__DynamicallyInvokable]
        public override System.Type Type
        {
            [__DynamicallyInvokable]
            get
            {
                if (this._value == null)
                {
                    return typeof(object);
                }
                return this._value.GetType();
            }
        }

        [__DynamicallyInvokable]
        public object Value
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._value;
            }
        }
    }
}

