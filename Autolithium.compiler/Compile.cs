using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Reflection;
using Autolithium.core;

namespace Autolithium.compiler
{
    partial class Program
    {
        public static List<MethodBuilder> DefinedMethods = new List<MethodBuilder>();
        public static List<FunctionDefinition> DefinedMethodInfo = new List<FunctionDefinition>();
        public static void ReadMethodsDefinition(TypeBuilder T, string Script)
        {
            var Matches = Regex.Matches(Script, "func(.*?)endfunc", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            foreach (Match m in Matches)
            {
                var FDef = LiParser.LexKeyword_FUNCTIONDEF(m.Value);
                DefinedMethodInfo.Add(FDef);
                DefinedMethods.Add(T.DefineMethod(
                    FDef.MyName, 
                    MethodAttributes.Public | MethodAttributes.Static,
                    typeof(object),
                    FDef.MyArguments.Select(x => x.MyType).ToArray()));

            }
        }
        public static void CreateMethods(TypeBuilder T, string Script, params Assembly[] Included)
        {
            var Matches = Regex.Matches(Script, "func(.*?)endfunc", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            int i = 0;
            foreach (Match m in Matches)
            {
                var FDef = LiParser.LexKeyword_FUNCTIONDEF(m.Value);
                var p = new LiParser(Script);
                p.Included = Included.ToList();
                p.DefinedMethods = DefinedMethods.Cast<MethodInfo>().ToList();
                p.ParseKeyword_FUNCTIONDEF(DefinedMethodInfo[i]).CompileToMethod(DefinedMethods[i++]);

            }
        }
        public static MethodBuilder CompileMain(TypeBuilder T, string Script, params Assembly[] References)
        {
            Script = Regex.Replace(Script, "func(.*?)endfunc", "", RegexOptions.IgnoreCase | RegexOptions.Singleline);
            var Method = T.DefineMethod("Autolithium-Main", MethodAttributes.Public | MethodAttributes.Static, typeof(int), new Type[] { typeof(string[]) });
            LiParser.Parse(Script, DefinedMethods.Cast<MethodInfo>().ToList(), References).CompileToMethod(Method);
            return Method;
        }
    }
}
