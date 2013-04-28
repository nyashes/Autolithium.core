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
        public static List<MethodBuilder> DefinedMethods = new List<MethodBuilder>();
        public static List<MethodBuilder> GeneratedMethods = new List<MethodBuilder>();
        public static List<FunctionDefinition> DefinedMethodInfo = new List<FunctionDefinition>();
        public static List<TypeBuilder> MethodContainers = new List<TypeBuilder>();
        public static void ReadMethodsDefinition(TypeBuilder T, ModuleBuilder M, string Script)
        {
            MethodContainers.Add(T);
            var Matches = Regex.Matches(Script, "func(.*?)endfunc", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match m in Matches)
            {
                var FDef = LiParser.LexKeyword_FUNCTIONDEF(m.Value);
                DefinedMethodInfo.Add(FDef);
                DefinedMethods.Add((MethodBuilder)T.DefineMethod(
                    FDef.MyName,
                    MethodAttributes.Public | MethodAttributes.Static,
                    typeof(object),
                    FDef.MyArguments.Select(x => x.MyType).ToArray()));
                FDef.Delegate = M.CreateDelegateFor(FDef);
                FDef.DelegateField = T.DefineField("&=" + FDef.MyName, FDef.Delegate, FieldAttributes.Public | FieldAttributes.Static);
            }
        }
        public static void CreateMethods(TypeBuilder T, ModuleBuilder M, string Script, params Assembly[] Included)
        {
            var Matches = Regex.Matches(Script, "func(.*?)endfunc", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            int i = 0;
            foreach (Match m in Matches)
            {
                var FDef = LiParser.LexKeyword_FUNCTIONDEF(m.Value);
                var p = new LiParser(Script);
                p.Included = Included.ToList();
                p.DefinedMethods = DefinedMethodInfo;
                p.ParseKeyword_FUNCTIONDEF(DefinedMethodInfo[i]).CompileToMethod(DefinedMethods[i]);

                DefinedMethodInfo[i].Body = DefinedMethods[i];
                i++;
            }
        }
        public static MethodBuilder CompileMain(ModuleBuilder M, string Script, params Assembly[] References)
        {
            var T = M.DefineType("Autolithium-" + Script.GetHashCode());
            ReadMethodsDefinition(T, M, Script);
            MethodBuilder DelegateBinder = T.DefineMethod("&=.DelegateBinder", MethodAttributes.Static);
            ASMInclude.ResxLoader(T, DelegateBinder);
            CreateMethods(T, M, Script, References);
            ASMInclude.GenerateDelegateBinder(DelegateBinder, DefinedMethodInfo);
            
            Script = Regex.Replace(Script, "func(.*?)endfunc", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var lambda = LiParser.Parse(Script, DefinedMethodInfo, References);
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
