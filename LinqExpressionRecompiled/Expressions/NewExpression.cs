namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Reflection;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.NewExpressionProxy)), __DynamicallyInvokable]
    public class NewExpression : Expression, IArgumentProvider
    {
        private IList<Expression> _arguments;
        private readonly ConstructorInfo _constructor;
        private readonly ReadOnlyCollection<MemberInfo> _members;

        internal NewExpression(ConstructorInfo constructor, IList<Expression> arguments, ReadOnlyCollection<MemberInfo> members)
        {
            this._constructor = constructor;
            this._arguments = arguments;
            this._members = members;
        }

        [__DynamicallyInvokable]
        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitNew(this);
        }

        Expression IArgumentProvider.GetArgument(int index)
        {
            return this._arguments[index];
        }

        [__DynamicallyInvokable]
        public NewExpression Update(IEnumerable<Expression> arguments)
        {
            if (arguments == this.Arguments)
            {
                return this;
            }
            if (this.Members != null)
            {
                return Expression.New(this.Constructor, arguments, this.Members);
            }
            return Expression.New(this.Constructor, arguments);
        }

        [__DynamicallyInvokable]
        public ReadOnlyCollection<Expression> Arguments
        {
            [__DynamicallyInvokable]
            get
            {
                return Expression.ReturnReadOnly<Expression>(ref this._arguments);
            }
        }

        [__DynamicallyInvokable]
        public ConstructorInfo Constructor
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._constructor;
            }
        }

        [__DynamicallyInvokable]
        public ReadOnlyCollection<MemberInfo> Members
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._members;
            }
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.New;
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
        public override System.Type Type
        {
            [__DynamicallyInvokable]
            get
            {
                return this._constructor.DeclaringType;
            }
        }
    }
}

