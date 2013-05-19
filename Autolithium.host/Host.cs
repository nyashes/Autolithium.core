using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using Autolithium.core;

namespace Autolithium.host
{
    public class Host
    {
        public static Dictionary<string, Host> Scripts = new Dictionary<string, Host>();

        public Dictionary<string, dynamic> GlobalVars = new Dictionary<string,dynamic>();
        public Dictionary<string, Delegate> Functions = new Dictionary<string,Delegate>();
        public Action<string[]> Main;

        protected internal static readonly FieldInfo ScriptsField = typeof(Host).GetRuntimeField("Scripts");
        protected internal static readonly FieldInfo FunctionsField = typeof(Host).GetRuntimeField("Functions");
        protected internal static readonly FieldInfo GlobalVarsField = typeof(Host).GetRuntimeField("GlobalVars");
        protected internal static readonly PropertyInfo ScriptsIndexer = ScriptsField.FieldType.GetRuntimeProperty("Item");    
        protected internal static readonly PropertyInfo FunctionsIndexer = FunctionsField.FieldType.GetRuntimeProperty("Item");
        protected internal static readonly PropertyInfo GlobalVarsIndexer = GlobalVarsField.FieldType.GetRuntimeProperty("Item");

        private static readonly List<Assembly> Included = new List<Assembly>();
        private static readonly List<Type> IncludedType = new List<Type>();

        private Host() { }

        public static void CreateScript(string ScopeName, string S)
        {
            if (Scripts.ContainsKey(ScopeName)) Scripts[ScopeName] = NewScript(ScopeName, S);
            else Scripts.Add(ScopeName, NewScript(ScopeName, S));
        }
        public static void UseAssembly(Assembly A)
        {
            Included.Add(A);
        }
        public static void UseType(Type T)
        {
            IncludedType.Add(T);
        }
        public void ClearVars()
        {
            GlobalVars.Clear();
        }
        public void ClearFuncs()
        {
            Functions.Clear();
        }
        public void Clear()
        {
            ClearVars(); ClearFuncs();
        }

        private static Host NewScript(string ScopeName, string S)
        {
            var r = new Host();
            r.Main = (Action<string[]>) Autolithium.core.LiParser.Parse(S,
                (Fdef) =>
                {
                    r.Functions.Add(Fdef.MyName, null);
                    Fdef.DelegateAccess =
                        Expression.MakeIndex(
                            Expression.MakeMemberAccess(
                                Expression.MakeIndex(
                                    Expression.MakeMemberAccess(null, ScriptsField), 
                                    ScriptsIndexer, 
                                    new Expression[] {Expression.Constant(ScopeName, typeof(string))}), 
                            FunctionsField),
                            FunctionsIndexer,
                            new Expression[] { Expression.Constant(Fdef.MyName) });
                },
                (Fdef, Lambda) =>
                {
                    r.Functions[Fdef.MyName] = Lambda.Compile();
                },
                (Name, Desired) =>
                {
                    //try { var F = GlobalVars.First(x => x.Key.ToUpper() == Name.ToUpper()); }
                    //catch { return null; }
                    var ExpAcc = Expression.MakeIndex(
                            Expression.MakeMemberAccess(
                                Expression.MakeIndex(
                                    Expression.MakeMemberAccess(null, ScriptsField),
                                    ScriptsIndexer,
                                    new Expression[] { Expression.Constant(ScopeName, typeof(string)) }), 
                                GlobalVarsField),
                        GlobalVarsIndexer,
                        new Expression[] { Expression.Constant(Name) });
                    if (Desired == null) return ExpAcc;
                    else return ExpAcc.ConvertTo(Desired);
                },
                (Name, Exp) =>
                {
                    //try { var F = GlobalVars.First(x => x.Key.ToUpper() == Name.ToUpper()); }
                    //catch { return null; }
                    return Expression.Assign(Expression.MakeIndex(
                            Expression.MakeMemberAccess(
                                Expression.MakeIndex(
                                    Expression.MakeMemberAccess(null, ScriptsField),
                                    ScriptsIndexer,
                                    new Expression[] { Expression.Constant(ScopeName, typeof(string)) }), 
                                GlobalVarsField),
                        GlobalVarsIndexer,
                        new Expression[] { Expression.Constant(Name) }), Exp.ConvertTo(typeof(object)));
                },
                (Name, T) =>
                {
                    r.GlobalVars.Add(Name, null);
                }, 
                Included, IncludedType).Compile();
            return r;
        }
    }
}
