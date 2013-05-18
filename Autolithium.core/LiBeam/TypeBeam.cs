using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public static partial class ExpressionTypeBeam
    {
        public static readonly IList<Type> NumericType = new List<Type> { typeof(double), typeof(long), typeof(int) };
        public static MethodInfo GetMethod(this Type T, string Name, params Type[] Parameters) { return T.GetRuntimeMethod(Name, Parameters); }

        public static object DefaultValue(this Type T)
        {
            return T.GetTypeInfo().IsValueType ? Activator.CreateInstance(T) : null;
        }

        public static Type Largest(params Type[] T)
        {
            if (T.Contains(typeof(double))) return typeof(double);
            if (T.Contains(typeof(long))) return typeof(long);
            if (T.Contains(typeof(int))) return typeof(int);
            return typeof(double);
        }
    }
}
