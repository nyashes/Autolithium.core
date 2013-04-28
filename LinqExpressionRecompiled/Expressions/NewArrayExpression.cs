namespace System.Linq.Expressions
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Diagnostics;
    using System.Runtime;

    [DebuggerTypeProxy(typeof(Expression.NewArrayExpressionProxy)), __DynamicallyInvokable]
    public class NewArrayExpression : Expression
    {
        private readonly ReadOnlyCollection<Expression> _expressions;
        private readonly System.Type _type;

        internal NewArrayExpression(System.Type type, ReadOnlyCollection<Expression> expressions)
        {
            this._expressions = expressions;
            this._type = type;
        }

        [__DynamicallyInvokable]
        protected internal override Expression Accept(ExpressionVisitor visitor)
        {
            return visitor.VisitNewArray(this);
        }

        internal static NewArrayExpression Make(ExpressionType nodeType, System.Type type, ReadOnlyCollection<Expression> expressions)
        {
            if (nodeType == ExpressionType.NewArrayInit)
            {
                return new NewArrayInitExpression(type, expressions);
            }
            return new NewArrayBoundsExpression(type, expressions);
        }

        [__DynamicallyInvokable]
        public NewArrayExpression Update(IEnumerable<Expression> expressions)
        {
            if (expressions == this.Expressions)
            {
                return this;
            }
            if (this.NodeType == ExpressionType.NewArrayInit)
            {
                return Expression.NewArrayInit(this.Type.GetElementType(), expressions);
            }
            return Expression.NewArrayBounds(this.Type.GetElementType(), expressions);
        }

        [__DynamicallyInvokable]
        public ReadOnlyCollection<Expression> Expressions
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._expressions;
            }
        }

        [__DynamicallyInvokable]
        public sealed override System.Type Type
        {
            [__DynamicallyInvokable, TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                return this._type;
            }
        }
    }
}

