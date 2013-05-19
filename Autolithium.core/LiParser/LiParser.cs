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
    public partial class LiParser
    {
        public List<Assembly> Included = new List<Assembly>();
        public List<Type> IncludedType = new List<Type>();
        public List<FunctionDefinition> DefinedFunctions = new List<FunctionDefinition>();
        //public AutoItVarCompilerEngine VarCompilerEngine;

        //public LiParser() { VarCompilerEngine = new AutoItVarCompilerEngine(this); }
        public LiParser(string Line, int LNumber = -1) 
        {
            ScriptLine = Line;
            LineNumber = LNumber;
        }
        public LiParser(string Text) 
        {
            Script = Text.Split(new string[] { "\r", "\n"}, StringSplitOptions.RemoveEmptyEntries);
            LineNumber = 1;
            ScriptLine = Script[0];
        }

        public static LambdaExpression Parse(
            string s, 
            DefineFuncDelegate D,
            CompileFuncDelegate C,
            GetVarDelegate G,
            SetVarDelegate S,
            CreateVarDelegate CR,
            IEnumerable<Assembly> RequireASM = null,
            IEnumerable<Type> RequireType = null)
        {
            var l = new LiParser(
                    Regex.Replace(s, ";(.*)((\r\n)|(\r)|(\n))", "\r\n")
                    .Replace("\r\n", "\r")
            );
            
            l.Included = RequireASM != null ? RequireASM.ToList() : new List<Assembly>();
            l.IncludedType = RequireType != null ? RequireType.ToList() : new List<Type>();
            l.DefineFunc = D;
            l.CompileFunc = C;
            l.GetVar = G;
            l.SetVar = S;
            l.CreateVar = CR;
            ExpressionTypeBeam.InitializeParameterEngine(G, S, CR);
            ExpressionTypeBeam.PushScope();

            List<Expression> Output = l.DefineGlobal();
            l.DefineFunction();
            l.LiCompileFunction();
            
            l.Script = Regex.Replace(string.Join("\r", l.Script), "func(.*?)endfunc", "", RegexOptions.IgnoreCase | RegexOptions.Singleline).Split('\r');
            l.Script = l.Script.Where(x => !Regex.IsMatch(x, "^(\t| )*global(.*?)$", RegexOptions.IgnoreCase)).ToArray();
            if (l.Script.Length > 0)
                l.GotoLine(0);

            Expression ex;
            
            var cmd = AutExpression.VariableAccess("CmdLine", false, Expression.Constant(1, typeof(int)), typeof(string)).Generator();
            if (l.ScriptLine != "" && !l.ScriptLine.StartsWith(";") && !l.EOF)
            {
                ex = l.ParseBoolean();
                if (ex is VarAutExpression) ex = (ex as VarAutExpression).Generator();
                Output.Add(ex);
            }
            while (!l.EOF)
            {
                l.NextLine();
                l.ConsumeWS();
                if (l.ScriptLine == "" || l.ScriptLine.StartsWith(";")) continue;
                
                ex = l.ParseBoolean();
                if (ex is VarAutExpression) ex = (ex as VarAutExpression).Generator();
                Output.Add(ex);
            }
            if (Output.Count <= 0) return null;
            var SC = ExpressionTypeBeam.PopScope();
            BlockExpression e = Expression.Block(SC.Where(x => x.Name != "CmdLine" || x.Type != typeof(string[])), Output.ToArray().Where(x => x != null));
            
            return Expression.Lambda<Action<string[]>>(e, SC.First(x => x.Name == "CmdLine" && x.Type == typeof(string[])));
        }
    }
}
