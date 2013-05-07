using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public delegate Expression GetVar(string Name, Type T);
    public delegate Expression SetVar(string Name, Type T, object Value);
    public delegate Expression CallFunc(string Name, params Expression[] Args);

    public partial class LiParser
    {
        public List<Expression> VarSynchronisation = new List<Expression>();
        public List<Assembly> Included = new List<Assembly>();
        public List<FunctionDefinition> DefinedFunctions = new List<FunctionDefinition>();
        public AutoItVarCompilerEngine VarCompilerEngine = new AutoItVarCompilerEngine();

        public LiParser(string Line, int LNumber = -1)
        {
            ScriptLine = Line;
            LineNumber = LNumber;
        }
        public LiParser(string Text)
        {
            Script = Text.Split(new string[] { "\r", "\n"}, StringSplitOptions.None);
            LineNumber = 1;
            ScriptLine = Script[0];
        }

        public static LambdaExpression Parse(
            string s, 
            DefineFuncDelegate D,
            CompileFuncDelegate C,
            params Assembly[] Require)
        {
            var l = new LiParser(
                    Regex.Replace(s, ";(.*)((\r\n)|(\r)|(\n))", "\r\n")
                    .Replace("\r\n", "\r")
            );
            
            l.Included = Require.ToList();
            l.DefineFunc = D;
            l.CompileFunc = C;
            l.DefineFunction();
            l.CompileFunction();

            l.Script = Regex.Replace(string.Join("\r", l.Script), "func(.*?)endfunc", "", RegexOptions.IgnoreCase | RegexOptions.Singleline).Split('\r');
            l.GotoLine(0);

            Expression ex;
            List<Expression> Output = new List<Expression>();

            if (l.ScriptLine != "" && !l.ScriptLine.StartsWith(";"))
            {
                ex = l.ParseBoolean();
                foreach (var x in l.VarSynchronisation) Output.Add(x);
                l.VarSynchronisation.Clear();
                Output.Add(ex);
            }
            while (!l.EOF)
            {
                l.NextLine();
                l.ConsumeWS();
                if (l.ScriptLine == "" || l.ScriptLine.StartsWith(";")) continue;
                
                ex = l.ParseBoolean();
                Output.AddRange(l.VarSynchronisation);
                l.VarSynchronisation.Clear();
                Output.Add(ex);
            }
            if (Output.Count <= 0) return null;
            BlockExpression e = Expression.Block(l.VarCompilerEngine.DefinedVars, Output.ToArray().Where(x => x != null));
            
            return Expression.Lambda<Action<string[]>>(e, Expression.Parameter(typeof(string[]), "CmdLine"));
        }
    }
}
