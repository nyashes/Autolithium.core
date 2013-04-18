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
    public class AutoItVarCompilerEngine
    {
        public Dictionary<string, AutoItVarCompiler> Get = new Dictionary<string, AutoItVarCompiler>();
        public KeyValuePair<string, Type[]>[] Save()
        {
            return Get.Select(x => new KeyValuePair<string, Type[]>(x.Key, x.Value.ActualType.ToArray())).ToArray();
        }
        public void Restore(KeyValuePair<string, Type[]>[] d, List<Expression> Sync)
        {
            foreach (var e in d)
                if (e.Value.Any(x => !Get[e.Key].ActualType.Contains(x)))
                    foreach (var i in e.Value.Where(x => !Get[e.Key].ActualType.Contains(x)))
                        Access(e.Key, Sync, i);
        }
        public AutoItVarCompiler Createvar(string Name)
        {
            Get.Add(Name, new AutoItVarCompiler()
            {
                MyName = Name,
            });
            return Get[Name];

        }
        public Expression Assign(string Name, Expression e)
        {
            if (Get.ContainsKey(Name)) return Get[Name].Assign(e);
            else return Createvar(Name).Assign(e);
        }
        public Expression Access(string Name, List<Expression> Sync, params Type[] desired)
        {
            if (Get.ContainsKey(Name)) return Get[Name].Access(Sync, desired);
            else throw new AutoitException(AutoitExceptionType.MISSINGVAR, 0, 0, Name);
        }
        public ParameterExpression[] DefinedVars
        {
            get
            {
                return Get.SelectMany(x => x.Value.PolymorphList.Select(y => y.Value)).ToArray();
            }
        }
    }
    public class AutoItVarCompiler
    {
        public static readonly MemberInfo InvariantCultureInfo = typeof(CultureInfo).GetRuntimeProperty("InvariantCulture");

        
        public Dictionary<Type, ParameterExpression> PolymorphList = new Dictionary<Type, ParameterExpression>();
        public List<Type> ActualType = new List<Type>();
        public Stack<Expression> ArrayIndex = new Stack<Expression>();
        public Stack<Type> MyType = new Stack<Type>();
        public string MyName;
        public AutoItVarCompilerEngine Parent;
        

        
        public Expression Assign(Expression e)
        {
            if (this.MyName.EndsWith("[]"))
            {

                return Expression.Assign(Expression.ArrayAccess(PolymorphList.First().Value, ArrayIndex.Pop()), 
                    Expression.Convert(e, typeof(object)));
            }
            if (!PolymorphList.ContainsKey(e.Type)) 
                PolymorphList.Add(e.Type, Expression.Parameter(e.Type, this.MyName));

            ActualType.Clear();
            ActualType.Add(e.Type);
            if (e.NodeType == ExpressionType.Add)
            {
                if ((e as BinaryExpression).Left == PolymorphList[e.Type])
                {
                    if ((e as BinaryExpression).Right.NodeType == ExpressionType.Constant && (dynamic)((e as BinaryExpression).Right as ConstantExpression).Value == 1)
                        return Expression.PreIncrementAssign(PolymorphList[e.Type]);
                    return Expression.AddAssign(PolymorphList[e.Type], (e as BinaryExpression).Right);
                }
            }
            return Expression.Assign(PolymorphList[e.Type], e);
        }
        public Expression Access(List<Expression> Sync, params Type[] desired)
        {
            Sync = Sync ?? new List<Expression>();
            if (this.MyName.EndsWith("[]"))
            {
                var T = MyType.Pop();
                var Index = ArrayIndex.Pop();
                Expression Ret = PolymorphList.First().Value;
                if (Index != null) Ret = Expression.ArrayAccess(Ret, Index.GetOfType(Parent, Sync, typeof(int)));
                if (T != null) Ret = Expression.Convert(Ret, T);
                if (T != null && desired.First() != T) //We should convert it correctly
                {
                    if (ExpressionExtension.Numeric.Contains(T)
                    && desired.Any(x => ExpressionExtension.Numeric.Contains(x))) // number to number, do it fast
                    {
                        return Expression.Convert(Ret,
                            ExpressionExtension.LargestNumeric(desired));
                    }
                    else //Long way
                        return GenericConvert(Ret, desired.First());
                }
                else if (desired.First() != T) Ret = GenericConvert(Ret, desired.First()); //Arbitrary and blind choice
                return Ret;

            }
            if (ActualType.Any(x => desired.Contains(x))) return PolymorphList[ActualType.First(x => desired.Contains(x))];
            else if (ActualType.Count <= 0)
                throw new AutoitException(AutoitExceptionType.UNASSIGNEDVARIABLE, 0, 0, MyName);
            else
            {
                if (!PolymorphList.ContainsKey(desired.First())) PolymorphList.Add(desired.First(), Expression.Parameter(desired.First(), this.MyName));
                if (ActualType.Any(x => ExpressionExtension.Numeric.Contains(x))
                    && desired.Any(x => ExpressionExtension.Numeric.Contains(x)))
                {
                    return Expression.Convert(PolymorphList[ExpressionExtension.LargestNumeric(ActualType)],
                        ExpressionExtension.LargestNumeric(desired));
                }
                else Sync.Add(
                    Expression.Assign(PolymorphList[desired.First()],
                        GenericConvert(PolymorphList[ActualType[0]], desired.First())));
                ActualType.Add(desired.First());
                return PolymorphList[desired.First()];
            }
        }
        public Expression ActualValue
        {
            get
            {
                return this.PolymorphList[this.ActualType.First()];
            }
        }
        public static Expression GenericConvert(Expression e, Type Destination)
        {
            if (Destination == typeof(object)) return
                Expression.Convert(e, typeof(object));
            if (Destination == typeof(bool)) return
                Expression.GreaterThan(GenericConvert(e, typeof(double)), Expression.Constant(0, typeof(double)));
            else if (Destination == typeof(string)) return
                Expression.Call(
                                typeof(Convert).GetRuntimeMethod("ToString", new Type[] { e.Type, typeof(IFormatProvider) }),
                                e,
                                Expression.MakeMemberAccess(null, AutoItVarCompiler.InvariantCultureInfo));
            else return Expression.Convert(
                            Expression.Call(
                                typeof(Convert).GetRuntimeMethod("ChangeType", new Type[] { typeof(object), typeof(Type), typeof(IFormatProvider) }),
                                Expression.Convert(e, typeof(object)),
                                Expression.Constant(Destination, typeof(Type)),
                                Expression.MakeMemberAccess(null, AutoItVarCompiler.InvariantCultureInfo)),
                            Destination);
        }
        public static Expression DynamicUnbox(Expression e)
        {
            return Expression.Call(DynamicUnboxInfo, e);
        }
        public static readonly MethodInfo DynamicUnboxInfo = typeof(AutoItVarCompiler).GetRuntimeMethod("Unbox", new Type[] { typeof(object) });
        public static dynamic Unbox(object e)
        {
            return (dynamic)e;
        }
    }
    public static class ExpressionExtension
    {
        public static readonly Type[] Numeric = new Type[] { typeof(int), typeof(long), typeof(double) };
        public static Expression GetOfType(this Expression value, AutoItVarCompilerEngine e, List<Expression> Sync, params Type[] desired)
        {
            if (!(value is ParameterExpression) || !(value as ParameterExpression).Name.EndsWith("[]"))
            {
                if (desired.Contains(typeof(object))) return Expression.Convert(value, typeof(object));
                if (desired.Contains(value.Type)) return value;
            }
            switch (value.NodeType)
            {
                case ExpressionType.Constant:
                    var val = value as ConstantExpression;
                    if (desired.Contains(val.Type)) return val;
                    else
                    {
                        if (val.Type == typeof(double) || val.Type == typeof(long) || val.Type == typeof(int) || val.Type == typeof(bool))
                        {
                            if (desired.Contains(typeof(double))) return Expression.Constant((double)(dynamic)val.Value, typeof(double));
                            else if (desired.Contains(typeof(long))) return Expression.Constant((long)(dynamic)val.Value, typeof(long));
                            else if (desired.Contains(typeof(int))) return Expression.Constant((int)(dynamic)val.Value, typeof(int));
                            else if (desired.Contains(typeof(bool))) return Expression.Constant((dynamic)val.Value > 0, typeof(bool));
                            else return Expression.Constant(Convert.ToString(val.Value, CultureInfo.InvariantCulture), typeof(string));
                        }
                        else if (val.Type == typeof(string))
                        {
                            return Expression.Constant(Convert.ChangeType(val.Value, desired.First()), desired.First());
                        }
                        else return val;
                    }
                case ExpressionType.Parameter:
                    var val2 = value as ParameterExpression;
                    return e.Access(val2.Name, Sync, desired);
                case ExpressionType.Call:
                    if (ExpressionExtension.Numeric.Contains(value.Type)
                    && desired.Any(x => ExpressionExtension.Numeric.Contains(x)))
                    {
                        return Expression.Convert(value,
                            ExpressionExtension.LargestNumeric(desired));
                    }
                    else return Expression.Convert(
                            Expression.Call(
                                typeof(Convert).GetRuntimeMethod("ChangeType", new Type[] { typeof(object), typeof(Type), typeof(IFormatProvider) }),
                                Expression.Convert(value, typeof(object)),
                                Expression.Constant(desired.First(), typeof(Type)),
                                Expression.MakeMemberAccess(null, AutoItVarCompiler.InvariantCultureInfo)),
                            desired.First());
                default:
                    return value;
            }
        }
        public static Type LargestNumeric(Type t1, Type t2)
        {
            if (t1 == typeof(double) || t2 == typeof(double)) return typeof(double);
            if (t1 == typeof(long) || t2 == typeof(long)) return typeof(long);
            if (t1 == typeof(int) || t2 == typeof(int)) return typeof(int);
            else return t1;
        }
        public static Type LargestNumeric(IEnumerable<Type> t)
        {
            if (t.Contains(typeof(double))) return typeof(double);
            if (t.Contains(typeof(long))) return typeof(long);
            if (t.Contains(typeof(int))) return typeof(int);
            else return t.First();
        }
    }
}
