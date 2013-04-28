namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime;

    [__DynamicallyInvokable]
    public sealed class MemberMemberBinding : MemberBinding
    {
        private ReadOnlyCollection<MemberBinding> _bindings;

        internal MemberMemberBinding(MemberInfo member, ReadOnlyCollection<MemberBinding> bindings) : base(MemberBindingType.MemberBinding, member)
        {
            this._bindings = bindings;
        }

        [__DynamicallyInvokable]
        public MemberMemberBinding Update(IEnumerable<MemberBinding> bindings)
        {
            if (bindings == this.Bindings)
            {
                return this;
            }
            return Expression.MemberBind(base.Member, bindings);
        }

        [__DynamicallyInvokable]
        public ReadOnlyCollection<MemberBinding> Bindings
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._bindings;
            }
        }
    }
}

