using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public static partial class ExpressionTypeBeam
    {
        public static Expression ConvertTo(this ConstantExpression E, Type T)
        {
            if (T == typeof(string)) //Convert to string ?
                return Expression.Constant(Convert.ToString(E.Value, CultureInfo.InvariantCulture), T);
            /*else if (NumericType.Contains(T)) //Convert to number ?
                return Expression.Constant(Convert.ToDouble(E.Value, CultureInfo.InvariantCulture), T);*/
            if (T == typeof(bool))
                return Expression.Constant(E.Type == typeof(string) ? (string)E.Value != "" : NumericType.Contains(E.Value) ? (dynamic)E.Value > 0 : E.Value != null, T);
            if (T == typeof(object))
                return Expression.Convert(E, T);
            else try //It's an IConvertible ?
                {
                    return Expression.Constant(Convert.ChangeType(E.Value, T), T);
                }
                catch //It's not, return default
                {
                    return Expression.Constant(T.DefaultValue(), T);
                }
        }
    }
}
