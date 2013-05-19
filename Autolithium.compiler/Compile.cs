using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using Autolithium.core;
using System.Net;
using System.Linq.Expressions;

namespace Autolithium.compiler
{
    partial class Program
    {
        public static List<FunctionDefinition> DefinedMethodInfo = new List<FunctionDefinition>();
        public static List<FieldBuilder> GlobalVars = new List<FieldBuilder>();

        public static MethodBuilder CompileMain(ModuleBuilder M, string Script, params Assembly[] References)
        {
            var T = M.DefineType("Autolithium-" + Script.GetHashCode());
            MethodBuilder DelegateBinder = T.DefineMethod("&=.DelegateBinder", MethodAttributes.Static);
            ASMInclude.ResxLoader(T, DelegateBinder);
            
            var lambda = LiParser.Parse(
                Script,
                FDef =>
                {
                    FDef.Body = T.DefineMethod(
                            FDef.MyName,
                            MethodAttributes.Public | MethodAttributes.Static,
                            FDef.ReturnType,
                            FDef.MyArguments.Select(x => x.MyType).ToArray()
                        );
                    FDef.Delegate = M.CreateDelegateFor(FDef);
                    FDef.AditionnalInfo = T.DefineField("&=" + FDef.MyName, FDef.Delegate, FieldAttributes.Public | FieldAttributes.Static);
                    FDef.DelegateAccess = Expression.MakeMemberAccess(null, (MemberInfo)FDef.AditionnalInfo);
                    DefinedMethodInfo.Add(FDef);
                },
                (FDef, Lambda) =>
                {
                    Lambda.CompileToMethod(FDef.Body as MethodBuilder);
                },
                (Name, Desired) =>
                {
                    var F = GlobalVars.FirstOrDefault(x => x.Name.ToUpper() == Name.ToUpper());
                    if (F == default(MethodBuilder)) return null;
                    else if (Desired == null) return Expression.MakeMemberAccess(null, F);
                    else return Expression.MakeMemberAccess(null, F)
                    .ConvertTo(Desired);
                },
                (Name, Expression) =>
                {
                    var F = GlobalVars.FirstOrDefault(x => x.Name.ToUpper() == Name.ToUpper());
                    if (F == default(MethodBuilder)) return null;
                    else return Expression.Assign(
                        Expression.MakeMemberAccess(null, F), 
                        Expression.ConvertTo(F.FieldType));
                },
                (Name, Type) =>
                {
                    GlobalVars.Add(T.DefineField(Name, Type, FieldAttributes.Public | FieldAttributes.Static));
                },
                References);

            ASMInclude.GenerateDelegateBinder(DelegateBinder, DefinedMethodInfo);
            MethodBuilder Method = null;
            if (lambda != null)
            {
                Method = T.DefineMethod("Autolithium-Main", MethodAttributes.Public | MethodAttributes.Static, typeof(int), new Type[] { typeof(string[]) });
                lambda.CompileToMethod(Method);
            }
            T.CreateType();
            return Method;
        }
    }
}
