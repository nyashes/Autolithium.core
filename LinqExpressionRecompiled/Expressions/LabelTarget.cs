namespace System.Linq.Expressions
{
    using System;
    using System.Runtime;

    [__DynamicallyInvokable]
    public sealed class LabelTarget
    {
        private readonly string _name;
        private readonly System.Type _type;

        internal LabelTarget(System.Type type, string name)
        {
            this._type = type;
            this._name = name;
        }

        [__DynamicallyInvokable]
        public override string ToString()
        {
            if (!string.IsNullOrEmpty(this.Name))
            {
                return this.Name;
            }
            return "UnamedLabel";
        }

        [__DynamicallyInvokable]
        public string Name
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._name;
            }
        }

        [__DynamicallyInvokable]
        public System.Type Type
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._type;
            }
        }
    }
}

