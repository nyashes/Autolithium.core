namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Dynamic.Utils;
    using System.Reflection;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.SwitchExpressionProxy)), __DynamicallyInvokable]
    public sealed class SwitchExpression : Expression
    {
        private readonly ReadOnlyCollection<SwitchCase> _cases;
        private readonly MethodInfo _comparison;
        private readonly Expression _defaultBody;
        private readonly Expression _switchValue;
        private readonly System.Type _type;

        internal SwitchExpression(System.Type type, Expression switchValue, Expression defaultBody, MethodInfo comparison, ReadOnlyCollection<SwitchCase> cases)
        {
            this._type = type;
            this._switchValue = switchValue;
            this._defaultBody = defaultBody;
            this._comparison = comparison;
            this._cases = cases;
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitSwitch(this);
        }

        [__DynamicallyInvokable]
        public SwitchExpression Update(Expression switchValue, IEnumerable<SwitchCase> cases, Expression defaultBody)
        {
            if (((switchValue == this.SwitchValue) && (cases == this.Cases)) && (defaultBody == this.DefaultBody))
            {
                return this;
            }
            return Expression.Switch(this.Type, switchValue, defaultBody, this.Comparison, cases);
        }

        [__DynamicallyInvokable]
        public ReadOnlyCollection<SwitchCase> Cases
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._cases;
            }
        }

        [__DynamicallyInvokable]
        public MethodInfo Comparison
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._comparison;
            }
        }

        [__DynamicallyInvokable]
        public Expression DefaultBody
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._defaultBody;
            }
        }

        internal bool IsLifted
        {
            get
            {
                if (!this._switchValue.Type.IsNullableType())
                {
                    return false;
                }
                if (this._comparison != null)
                {
                    return !TypeUtils.AreEquivalent(this._switchValue.Type, this._comparison.GetParametersCached()[0].ParameterType.GetNonRefType());
                }
                return true;
            }
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.Switch;
            }
        }

        [__DynamicallyInvokable]
        public Expression SwitchValue
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._switchValue;
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

