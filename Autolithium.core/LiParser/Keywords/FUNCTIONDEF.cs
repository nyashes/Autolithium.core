using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Autolithium.core
{
    public class ArgumentDefinition
    {
        public Type MyType = typeof(object);
        public string ArgName;
        public bool IsByRef =false;
        public Expression DefaultValue = Expression.Constant(null);
    }
    public class FunctionDefinition
    {
        public string MyName;
        public Type ReturnType = typeof(object);
        public List<ArgumentDefinition> MyArguments = new List<ArgumentDefinition>();
        public MethodInfo Body;
        public Type Delegate;
        public FieldInfo DelegateField;
    }
    public partial class LiParser
    {
        public static FunctionDefinition LexKeyword_FUNCTIONDEF(string Definition)
        {
            var Parser = new LiParser(Definition);
            bool IsByRef = false;
            Expression DefaultValue = null;
            Parser.Getstr(Reg_AlphaNum);
            Parser.ConsumeWS();
            var Func = new FunctionDefinition()
                {
                    MyName = Parser.Getstr(Reg_AlphaNum)
                };
            Parser.ConsumeWS();
            while (Parser.Read() != "(") ;
            while (Parser.Peek() != ")")
            {
                Parser.Consume();
                Parser.ConsumeWS();
                var KW = Parser.Getstr(Reg_AlphaNum);
                if (KW.ToUpper() == "BYREF")
                {
                    IsByRef = true;
                    Parser.ConsumeWS();
                    Parser.Consume();
                    KW = Parser.Getstr(Reg_AlphaNum);
                }
                Parser.ConsumeWS();
                if (Parser.Peek() == "=")
                {
                    Parser.Consume();
                    Parser.ConsumeWS();
                    DefaultValue = Parser.ParseUnary();
                }
                Func.MyArguments.Add(new ArgumentDefinition()
                {
                    IsByRef = IsByRef,
                    ArgName = KW,
                    DefaultValue = DefaultValue
                });
                Parser.ConsumeWS();
                //Parser.NextLine();
                
            }
            return Func;
            
        }
        public LambdaExpression ParseKeyword_FUNCTIONDEF(FunctionDefinition Func)
        {
            //Create a new Parser here
            bool Continue = true;
            var Fnb = this.Script.SkipWhile(x => !Regex.IsMatch(x, "^func( +?)" + Regex.Escape(Func.MyName), RegexOptions.IgnoreCase))
                .Skip(1)
                .Where(x => Continue = Continue && x.ToUpper() != "ENDFUNC");
            var Parser = new LiParser(string.Join("\r", Fnb));
            Parser.Included = this.Included;
            Parser.DefinedMethods = this.DefinedMethods;
            Expression ex;
            List<Expression> Output = new List<Expression>();
            List<ParameterExpression> Parameters = new List<ParameterExpression>();

            Output.Add(Parser.VarCompilerEngine.Assign("Return-store", Expression.Constant(default(object), typeof(object))));

            foreach(var x in Func.MyArguments) 
                Parameters.Add((Parser.VarCompilerEngine.Assign(x.ArgName, Expression.Constant(default(object), typeof(object))) as BinaryExpression).Left as ParameterExpression);
            var Ret = Expression.Label();
            Parser.Contextual.Push(Expression.Goto(Ret, Func.ReturnType));
            foreach (var x in Func.MyArguments.Where(x => x.DefaultValue != null))
            {
                var Exp = Parser.VarCompilerEngine.Assign(x.ArgName, 
                    Expression.Coalesce(
                        Parser.VarCompilerEngine.Access(x.ArgName, Parser.VarSynchronisation, x.MyType), 
                        x.DefaultValue.GetOfType(Parser.VarCompilerEngine, Parser.VarSynchronisation, x.MyType)));
                Output.AddRange(Parser.VarSynchronisation);
                Parser.VarSynchronisation.Clear();
                Output.Add(Exp);
            }
            if (Parser.ScriptLine != "" && !Parser.ScriptLine.StartsWith(";"))
            {
                ex = Parser.ParseBoolean();
                foreach (var x in Parser.VarSynchronisation) Output.Add(x);
                Parser.VarSynchronisation.Clear();
                Output.Add(ex);
            }
            while (!Parser.EOF)
            {
                Parser.NextLine();
                Parser.ConsumeWS();
                if (Parser.ScriptLine == "" || Parser.ScriptLine.StartsWith(";")) continue;

                ex = Parser.ParseBoolean();
                Output.AddRange(Parser.VarSynchronisation);
                Parser.VarSynchronisation.Clear();
                Output.Add(ex);
            }
            Parser.Contextual.Pop();
            Output.Add(Expression.Label(Ret));
            Output.Add(Parser.VarCompilerEngine.Access("Return-store", VarSynchronisation, typeof(object)));
            
            List<string> AlreadyAParam = new List<string>();
            var Variables = Parser.VarCompilerEngine.DefinedVars.Except(Parameters);
            /*var Params = Parser.VarCompilerEngine.DefinedVars.Except(Variables).ToList();
            Params.Sort((x, y) =>
            {
                return Func.MyArguments.FindIndex(z => z.ArgName == x.Name) < Func.MyArguments.FindIndex(z => z.ArgName == y.Name) ? 1 : -1;
            });*/
            var Block = Expression.Block(typeof(object), Variables, Output);
            return LambdaExpression.Lambda(Block, Parameters);
        }
    }
}
