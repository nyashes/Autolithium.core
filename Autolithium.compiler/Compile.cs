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
                    FDef.DelegateField = T.DefineField("&=" + FDef.MyName, FDef.Delegate, FieldAttributes.Public | FieldAttributes.Static);
                    DefinedMethodInfo.Add(FDef);
                },
                (FDef, Lambda) =>
                {
                    Lambda.CompileToMethod(FDef.Body as MethodBuilder);
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
