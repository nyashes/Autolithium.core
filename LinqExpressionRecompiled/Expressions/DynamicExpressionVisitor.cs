namespace System.Linq.Expressions
{
    using System;
    using System.Runtime;

    [__DynamicallyInvokable]
    public abstract class DynamicExpressionVisitor : ExpressionVisitor
    {
        [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        protected DynamicExpressionVisitor()
        {
        }

        [__DynamicallyInvokable]
        protected internal override Expression VisitDynamic(DynamicExpression node)
        {
            return base.VisitDynamic(node);
        }
    }
}

