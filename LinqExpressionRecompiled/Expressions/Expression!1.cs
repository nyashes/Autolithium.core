namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Dynamic.Utils;
    using System.Linq.Expressions.Compiler;
    using System.Runtime.CompilerServices;

    [__DynamicallyInvokable]
    public sealed class Expression<TDelegate> : LambdaExpression
    {
        internal Expression(Expression body, string name, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters) : base(typeof(TDelegate), name, body, tailCall, parameters)
        {
        }

        internal override LambdaExpression Accept(StackSpiller spiller)
        {
            return spiller.Rewrite<TDelegate>((Expression<TDelegate>) this);
        }

        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitLambda<TDelegate>((Expression<TDelegate>) this);
        }

        [__DynamicallyInvokable]
        public TDelegate Compile()
        {
            return (TDelegate) LambdaCompiler.Compile(this, null);
        }

        public TDelegate Compile(DebugInfoGenerator debugInfoGenerator)
        {
            ContractUtils.RequiresNotNull(debugInfoGenerator, "debugInfoGenerator");
            return (TDelegate) LambdaCompiler.Compile(this, debugInfoGenerator);
        }

        internal static LambdaExpression Create(Expression body, string name, bool tailCall, ReadOnlyCollection<ParameterExpression> parameters)
        {
            return new Expression<TDelegate>(body, name, tailCall, parameters);
        }

        [__DynamicallyInvokable]
        public Expression<TDelegate> Update(Expression body, IEnumerable<ParameterExpression> parameters)
        {
            if ((body == base.Body) && (parameters == base.Parameters))
            {
                return (Expression<TDelegate>) this;
            }
            return Expression.Lambda<TDelegate>(body, base.Name, base.TailCall, parameters);
        }
    }
}

