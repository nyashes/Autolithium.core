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
        public List<Expression> VarSynchronisation = new List<Expression>();

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

        public static LambdaExpression Parse(string s, string LocalDir = "")
        {
            var l = new LiParser(
                    Regex.Replace(s, ";(.*)((\r\n)|(\r)|(\n))", "\r\n")
                    .Replace("\r\n", "\r")
            );
            Expression ex;
            List<Expression> Output = new List<Expression>();
            List<Expression> Vars = new List<Expression>();
            /*Output.Add(Expression.Call(
                typeof(Assembly).GetTypeInfo().DeclaredMethods.First(x => x.Name == "Load"), 
                Expression.Constant(typeof(LiParser).GetTypeInfo().Assembly.FullName, typeof(string))));*/
            ex = l.ParseBoolean();
            foreach (var x in l.VarSynchronisation) Output.Add(x);
            l.VarSynchronisation.Clear();
            Output.Add(ex);
            while (!l.EOF)
            {
                l.NextLine();
                l.ConsumeWS();
                if (l.ScriptLine == "" || l.ScriptLine.StartsWith(";")) continue;
                if (l.ScriptLine.StartsWith("#"))
                {
                    var cmd = l.ScriptLine.Split(new char[] {' ', '\t'}, StringSplitOptions.RemoveEmptyEntries);
                    switch (cmd[0].ToUpper())
                    {
                        case "#REQUIRE":
                            if (cmd[1].StartsWith("<"))
                            {
                                cmd[1] = cmd[1].Substring(1, cmd[1].Length - 2);
                                Output.Add(Expression.Call(
                                    typeof(Assembly).GetTypeInfo().DeclaredMethods.First(x => x.Name == "Load"),
                                    Expression.Constant("Include\\" + cmd[1], typeof(string))));
                            }
                            else if (cmd[1].StartsWith("\""))
                            {
                                cmd[1] = cmd[1].Substring(1, cmd[1].Length - 2);
                                Output.Add(Expression.Call(
                                    typeof(Assembly).GetTypeInfo().DeclaredMethods.First(x => x.Name == "Load"),
                                    Expression.Constant(LocalDir + cmd[1], typeof(string))));
                            }
                            break;
                    }
                }
                ex = l.ParseBoolean();
                Output.AddRange(l.VarSynchronisation);
                l.VarSynchronisation.Clear();
                Output.Add(ex);
            }
            BlockExpression e = Expression.Block(AutoItVarCompiler.DefinedVars, Output.ToArray().Where(x => x != null));
            
            return Expression.Lambda<Action<string[]>>(e, Expression.Parameter(typeof(string[]), "CmdLine"));
        }
    }
}
