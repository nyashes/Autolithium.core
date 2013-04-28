namespace System.Linq.Expressions
{
    using System;
    using System.Diagnostics;
    using System.Dynamic.Utils;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.DebugInfoExpressionProxy)), __DynamicallyInvokable]
    public class DebugInfoExpression : Expression
    {
        private readonly SymbolDocumentInfo _document;

        internal DebugInfoExpression(SymbolDocumentInfo document)
        {
            this._document = document;
        }

        [__DynamicallyInvokable]
        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitDebugInfo(this);
        }

        [__DynamicallyInvokable]
        public SymbolDocumentInfo Document
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._document;
            }
        }

        [__DynamicallyInvokable]
        public virtual int EndColumn
        {
            [__DynamicallyInvokable]
            get
            {
                throw ContractUtils.Unreachable;
            }
        }

        [__DynamicallyInvokable]
        public virtual int EndLine
        {
            [__DynamicallyInvokable]
            get
            {
                throw ContractUtils.Unreachable;
            }
        }

        [__DynamicallyInvokable]
        public virtual bool IsClear
        {
            [__DynamicallyInvokable]
            get
            {
                throw ContractUtils.Unreachable;
            }
        }

        [__DynamicallyInvokable]
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.DebugInfo;
            }
        }

        [__DynamicallyInvokable]
        public virtual int StartColumn
        {
            [__DynamicallyInvokable]
            get
            {
                throw ContractUtils.Unreachable;
            }
        }

        [__DynamicallyInvokable]
        public virtual int StartLine
        {
            [__DynamicallyInvokable]
            get
            {
                throw ContractUtils.Unreachable;
            }
        }

        [__DynamicallyInvokable]
        public sealed override System.Type Type
        {
            [__DynamicallyInvokable]
            get
            {
                return typeof(void);
            }
        }
    }
}

