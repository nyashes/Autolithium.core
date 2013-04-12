using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Globalization;

namespace Autolithium.core
{
    public class AutoItVarCompiler
    {
        public static Dictionary<string, AutoItVarCompiler> Get = new Dictionary<string,AutoItVarCompiler>();
        public Dictionary<Type, ParameterExpression> PolymorphList = new Dictionary<Type, ParameterExpression>();
        public List<Type> ActualType = new List<Type>();
        public Expression ArrayIndex;
        public string MyName;

        public static AutoItVarCompiler Createvar(string Name, Expression index = null)
        {
            Get.Add(Name, new AutoItVarCompiler()
            {
                MyName = Name,
                ArrayIndex = index
            });
            return Get[Name];

        }
        public static Expression Assign(string Name, Expression e)
        {
            if (Get.ContainsKey(Name)) return Get[Name].Assign(e);
            else return Createvar(Name).Assign(e);
        }
        public static Expression Access(string Name, List<Expression> Sync, params Type[] desired)
        {
            if (Get.ContainsKey(Name)) return Get[Name].Access(Sync, desired);
            else throw new AutoitException(AutoitExceptionType.MISSINGVAR, 0, 0, Name);
        }
        public Expression Assign(Expression e)
        {
            if (!PolymorphList.ContainsKey(e.Type)) 
                PolymorphList.Add(e.Type, Expression.Parameter(e.Type));
            ActualType.Clear();
            ActualType.Add(e.Type);
            return Expression.Assign(PolymorphList[e.Type], e);
        }
        public Expression Access(List<Expression> Sync, params Type[] desired)
        {
            Sync = Sync ?? new List<Expression>();
            if (ActualType.Any(x => desired.Contains(x))) return PolymorphList[ActualType.First(x => desired.Contains(x))];
            else if (ActualType.Count <= 0)
                throw new AutoitException(AutoitExceptionType.UNASSIGNEDVARIABLE, 0, 0, MyName);
            else
            {
                Sync.Add(
                    Expression.Assign(PolymorphList[desired],
                        Expression.Convert(
                            Expression.Call(
                                typeof(Convert).GetRuntimeMethod("ChangeType", new Type[] { typeof(object), typeof(Type), typeof(IFormatProvider) }),
                                PolymorphList[ActualType[0]],
                                Expression.Constant(desired, typeof(Type)),
                                Expression.Constant(CultureInfo.InvariantCulture, typeof(IFormatProvider))),
                            desired)));
                return PolymorphList[desired];
            }
        }
    }
}
