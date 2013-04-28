namespace System.Linq.Expressions
{
    using System;
    using System.Dynamic.Utils;
    using System.Linq.Expressions.Compiler;
    using System.Runtime;

    [__DynamicallyInvokable]
    public class SymbolDocumentInfo
    {
        private readonly string _fileName;

        internal SymbolDocumentInfo(string fileName)
        {
            ContractUtils.RequiresNotNull(fileName, "fileName");
            this._fileName = fileName;
        }

        [__DynamicallyInvokable]
        public virtual Guid DocumentType
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return SymbolGuids.DocumentType_Text;
            }
        }

        [__DynamicallyInvokable]
        public string FileName
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._fileName;
            }
        }

        [__DynamicallyInvokable]
        public virtual Guid Language
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return Guid.Empty;
            }
        }

        [__DynamicallyInvokable]
        public virtual Guid LanguageVendor
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return Guid.Empty;
            }
        }
    }
}

