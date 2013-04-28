namespace System.Linq.Expressions
{
    using System;
    using System.Reflection;
    using System.Runtime;

    [__DynamicallyInvokable]
    public abstract class MemberBinding
    {
        private MemberInfo _member;
        private MemberBindingType _type;

        [Obsolete("Do not use this constructor. It will be removed in future releases."), TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        protected MemberBinding(MemberBindingType type, MemberInfo member)
        {
            this._type = type;
            this._member = member;
        }

        [__DynamicallyInvokable]
        public override string ToString()
        {
            return ExpressionStringBuilder.MemberBindingToString(this);
        }

        [__DynamicallyInvokable]
        public MemberBindingType BindingType
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._type;
            }
        }

        [__DynamicallyInvokable]
        public MemberInfo Member
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._member;
            }
        }
    }
}

