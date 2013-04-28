namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Reflection;
    using System.Runtime;

    [__DynamicallyInvokable]
    public sealed class MemberListBinding : MemberBinding
    {
        private ReadOnlyCollection<ElementInit> _initializers;

        internal MemberListBinding(MemberInfo member, ReadOnlyCollection<ElementInit> initializers) : base(MemberBindingType.ListBinding, member)
        {
            this._initializers = initializers;
        }

        [__DynamicallyInvokable]
        public MemberListBinding Update(IEnumerable<ElementInit> initializers)
        {
            if (initializers == this.Initializers)
            {
                return this;
            }
            return Expression.ListBind(base.Member, initializers);
        }

        [__DynamicallyInvokable]
        public ReadOnlyCollection<ElementInit> Initializers
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._initializers;
            }
        }
    }
}

