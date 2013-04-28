namespace System.Linq.Expressions
{
    using System;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Dynamic.Utils;
    using System.Linq.Expressions.Compiler;
    using System.Reflection.Emit;
    using System.Runtime;
    using System.Runtime.CompilerServices;

    [DebuggerTypeProxy(typeof(Expression.LambdaExpressionProxy)), __DynamicallyInvokable]
    public abstract class LambdaExpression : Expression
    {
        private readonly Expression _body;
        private readonly System.Type _delegateType;
        private readonly string _name;
        private readonly ReadOnlyCollection<ParameterExpression> _parameters;
        private readonly bool _tailCall;

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        internal LambdaExpression(System.Type delegateType, string name, Expression body, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters)
        {
            this._name = name;
            this._body = body;
            this._parameters = parameters;
            this._delegateType = delegateType;
            this._tailCall = tailCall;
        }

        internal abstract LambdaExpression Accept(StackSpiller spiller);
        [__DynamicallyInvokable]
        public Delegate Compile()
        {
            return LambdaCompiler.Compile(this, null);
        }

        public Delegate Compile(DebugInfoGenerator debugInfoGenerator)
        {
            ContractUtils.RequiresNotNull(debugInfoGenerator, "debugInfoGenerator");
            return LambdaCompiler.Compile(this, debugInfoGenerator);
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public void CompileToMethod(MethodBuilder method)
        {
            this.CompileToMethodInternal(method, null);
        }

        public void CompileToMethod(MethodBuilder method, DebugInfoGenerator debugInfoGenerator)
        {
            ContractUtils.RequiresNotNull(debugInfoGenerator, "debugInfoGenerator");
            this.CompileToMethodInternal(method, debugInfoGenerator);
        }

        private void CompileToMethodInternal(MethodBuilder method, DebugInfoGenerator debugInfoGenerator)
        {
            ContractUtils.RequiresNotNull(method, "method");
            ContractUtils.Requires(method.IsStatic, "method");
            TypeBuilder declaringType = method.DeclaringType as TypeBuilder;
            if (declaringType == null)
            {
                throw Error.MethodBuilderDoesNotHaveTypeBuilder();
            }
            LambdaCompiler.Compile(this, method, debugInfoGenerator);
        }

        [__DynamicallyInvokable]
        public Expression Body
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._body;
            }
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
        public sealed override ExpressionType NodeType
        {
            [__DynamicallyInvokable]
            get
            {
                return ExpressionType.Lambda;
            }
        }

        [__DynamicallyInvokable]
        public ReadOnlyCollection<ParameterExpression> Parameters
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._parameters;
            }
        }

        [__DynamicallyInvokable]
        public System.Type ReturnType
        {
            [__DynamicallyInvokable]
            get
            {
                return this.Type.GetMethod("Invoke").ReturnType;
            }
        }

        [__DynamicallyInvokable]
        public bool TailCall
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._tailCall;
            }
        }

        [__DynamicallyInvokable]
        public sealed override System.Type Type
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._delegateType;
            }
        }
    }
}

