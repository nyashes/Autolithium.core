/*Copyright or © or Copr. THOUVENIN Alexandre (2013)

nem-e5i5software@live.fr

This software is a computer program whose purpose is to [describe
functionalities and technical features of your software].

This software is governed by the CeCILL-C license under French law and
abiding by the rules of distribution of free software.  You can  use, 
modify and/ or redistribute the software under the terms of the CeCILL-C
license as circulated by CEA, CNRS and INRIA at the following URL
"http://www.cecill.info". 

As a counterpart to the access to the source code and  rights to copy,
modify and redistribute granted by the license, users are provided only
with a limited warranty  and the software's author,  the holder of the
economic rights,  and the successive licensors  have only  limited
liability. 

In this respect, the user's attention is drawn to the risks associated
with loading,  using,  modifying and/or developing or reproducing the
software by the user in light of its specific status of free software,
that may mean  that it is complicated to manipulate,  and  that  also
therefore means  that it is reserved for developers  and  experienced
professionals having in-depth computer knowledge. Users are therefore
encouraged to load and test the software's suitability as regards their
requirements in conditions enabling the security of their systems and/or 
data to be ensured and,  more generally, to use and operate it in the 
same conditions as regards security. 

The fact that you are presently reading this means that you have had
knowledge of the CeCILL-C license and that you accept its terms.*/

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
        //public List<FunctionDefinition> DefinedFunctions = new List<FunctionDefinition>();
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
            IScope GlobalScope,
            IEnumerable<Assembly> RequireASM = null,
            IEnumerable<Type> RequireType = null)
        {
            var l = new LiParser(
                    Regex.Replace(s, ";(.*)((\r\n)|(\r)|(\n))", "\r\n")
                    .Replace("\r\n", "\r")
            );
            
            l.Included = RequireASM != null ? RequireASM.ToList() : new List<Assembly>();
            l.IncludedType = RequireType != null ? RequireType.ToList() : new List<Type>();
            /*l.DefineFunc = D;
            l.CompileFunc = C;
            l.GetVar = G;
            l.SetVar = S;
            l.CreateVar = CR;*/
            GlobalScope.Parent = new ClrScope((RequireType ?? new List<Type>()).Concat((RequireASM ?? new List<Assembly>()).SelectMany(x => x.ExportedTypes)));
            ExpressionTypeBeam.InitializeParameterEngine(GlobalScope);
            

            List<Expression> Output = new List<Expression>();
            l.DefineGlobal();
            l.DeclareAllFunctions();
            l.LiCompileFunction();

            ExpressionTypeBeam.PushScope();
            l.Script = Regex.Replace(string.Join("\r", l.Script), "func(.*?)endfunc", "", RegexOptions.IgnoreCase | RegexOptions.Singleline).Split('\r');
            if (l.Script.Length > 0)
                l.GotoLine(0);

            Expression ex;

            var cmd = AutExpression.VariableAccess("CmdLine", false, Expression.Constant(1, typeof(int)), typeof(string)).Generator();
            l.ConsumeWS();
            if (l.ScriptLine != "" && !l.ScriptLine.StartsWith(";") && !l.EOF && !l.EOL)
            {
                ex = l.ParseBoolean();
                if (ex is VarAutExpression) ex = (ex as VarAutExpression).Generator();
                Output.Add(ex);
            }
            while (!l.EOF)
            {
                try
                {
                    l.NextLine();
                }
                catch { break; }
                l.ConsumeWS();
                if (l.ScriptLine == "" || l.ScriptLine.StartsWith(";") || l.EOF || l.EOL) continue;
                
                ex = l.ParseBoolean();
                if (ex is VarAutExpression) ex = (ex as VarAutExpression).Generator();
                Output.Add(ex);
            }
            if (Output.Count(x => x != null) <= 0) return null;
            var SC = ExpressionTypeBeam.PopScope();
            BlockExpression e = Expression.Block(SC.Where(x => x.Name != "CmdLine" || x.Type != typeof(string[])), Output.ToArray().Where(x => x != null));
            
            return Expression.Lambda<Action<string[]>>(e, SC.First(x => x.Name == "CmdLine" && x.Type == typeof(string[])));
        }
    }
}
