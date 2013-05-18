using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Autolithium.core
{
    public static partial class ExpressionTypeBeam
    {
        //Required methods
        private static Expression InvariantCultureFieldAccess = Expression.MakeMemberAccess(null,
            typeof(CultureInfo).GetRuntimeProperty("InvariantCulture"));
        private static MethodInfo Convert_ToString(Type T)
        {
            try
            {
                return typeof(Convert)
                   .GetMethod("ToString", T, typeof(IFormatProvider));
            }
            catch
            {
                return typeof(Convert)
                    .GetMethod("ToString", typeof(object), typeof(IFormatProvider));
            }
        }
        private static MethodInfo Convert_ChangeType = typeof(Convert).GetMethod("ChangeType", typeof(object), typeof(Type));
        private static MethodInfo Numeric_Parse(Type Num)
        {
            return Num.GetMethod("Parse", typeof(string), typeof(IFormatProvider));
        }

        public static Expression ConvertTo(this Expression E, Type T)
        {
            if (E.GetType() == typeof(ConstantExpression)) return ((ConstantExpression)E).ConvertTo(T);
            if (E.GetType() == typeof(VarAutExpression)) return ((VarAutExpression)E).Getter(T);

            if (E.Type == T) return E;
            else if (T == typeof(string)) return E.ConvertToString();
            else if (NumericType.Contains(T)) return E.ConvertToNumber(T);
            else if (T == typeof(bool)) return E.ConvertToBool();
            else if (T == typeof(object)) return Expression.Convert(E, T);
            else return Expression.Convert(Expression.Call(Convert_ChangeType, Expression.Convert(E, typeof(object)), Expression.Constant(T, typeof(Type))), T);
        }

        public static Expression ConvertToNumber(this Expression E, Type Favorite = null)
        {
            if (E.GetType() == typeof(ConstantExpression)) return ((ConstantExpression)E).ConvertTo(Favorite ?? typeof(double));
            if (NumericType.Contains(E.Type))
                if (E.Type == Favorite) return E;
                else if (Favorite != null) return Expression.Convert(E, Favorite);

            if (E.GetType() == typeof(VarAutExpression)) return ((VarAutExpression)E).Getter(Favorite ?? typeof(double));

            if (NumericType.Contains(E.Type)) return (Favorite != null && E.Type != Favorite) ? Expression.Convert(E, Favorite) : E;
            else if (E.Type == typeof(string))
                return Expression.Call(Numeric_Parse(Favorite ?? typeof(double)), E, InvariantCultureFieldAccess);
            else if (E.Type == typeof(bool)) return Expression.IfThenElse(E, Expression.Constant(1, Favorite ?? typeof(int)), Expression.Constant(0, Favorite ?? typeof(int)));
            
            else return Expression.Convert(Expression.Call(Convert_ChangeType, Expression.Convert(E, typeof(object)), Expression.Constant(Favorite, typeof(Type))), Favorite);
        }

        public static Expression ConvertToString(this Expression E)
        {
            if (E.GetType() == typeof(ConstantExpression)) return ((ConstantExpression)E).ConvertTo(typeof(string));
            if (E.GetType() == typeof(VarAutExpression)) return ((VarAutExpression)E).Getter(typeof(string));

            if (E.Type == typeof(string)) return E;
            else return Expression.Call(Convert_ToString(E.Type), E, InvariantCultureFieldAccess);
        }

        public static Expression ConvertToBool(this Expression E)
        {
            if (E.Type == typeof(bool)) return E;
            else if (NumericType.Contains(E.Type)) return Expression.GreaterThan(E, Expression.Constant(0, typeof(int)));
            else if (E.Type == typeof(string)) return Expression.NotEqual(Expression.Constant("", typeof(string)), E);
            else return Expression.NotEqual(E, Expression.Constant(null));
        }
    }
}
