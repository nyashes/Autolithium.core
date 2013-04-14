using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public class LiParser
    {
        private const string Reg_AlphaNum = @"[a-zA-Z0-9_]";
        private static readonly TypeInfo AutoItWeakObjectInfo = typeof(AutoItWeakObject).GetTypeInfo();
        private string Lexer_CSString()
        {
            string Ret = "";
            string f = Read();
            while (Peek() != f || (Peek(2) == (f + f))) Ret += Read();
            Consume();
            return Ret;
        }
        private string Getstr(string reg)
        {
            string r = "";
            while (!EOL && System.Text.RegularExpressions.Regex.IsMatch(Peek(), reg)) r += Read();
            return r;
        }
        private string GetNbr()
        {
            var m = System.Text.RegularExpressions.Regex.Match(ScriptLine.Substring(Cursor), @"^-?(([1-9]\d*)|0)(.0*[1-9](0*[1-9])*)?");
            Cursor += m.Length;
            return m.Value;
        }
        private string ScriptLine;
        private string[] Script;
        private int Cursor = 0;
        private Type PreferedType = typeof(object);
        public int LineNumber;


        public List<Expression> VarSynchronisation = new List<Expression>();

        public string Peek(int ChrCount = 1)
        {
            if (Cursor + ChrCount > ScriptLine.Length) return new string('\0', ChrCount);
            return ScriptLine.Substring(Cursor, ChrCount);
        }
        public string Read(int ChrCount = 1)
        {
            var s = Peek(ChrCount);
            Consume(ChrCount);
            return s;
        }
        public void Consume(int ChrCount = 1)
        {
            Cursor += ChrCount;
        }
        public void ConsumeWS()
        {
            while (!EOL && (Peek() == " " || Peek() == "\t")) Cursor++;
        }
        public void NextLine()
        {
            if (Script != null && LineNumber < Script.Length)
                ScriptLine = Script[LineNumber++];
            else throw new IndexOutOfRangeException("Cannot move to next line in interactive mode or EOF reached");
            Cursor = 0;
        }
        public void Seek(int Pos = 0)
        {
            Cursor = Pos;
        }
        public void SeekRelative(int Offset = 0)
        {
            Cursor += Offset;
        }
        public bool EOL { get { return Cursor >= ScriptLine.Length; } }
        public bool EOF { get { return (Script == null || LineNumber >= Script.Length); } }
        public bool INTERCATIVE { get { return Script == null || Script.Length <= 1; } }
        public List<ParameterExpression> DefinedVar = new List<ParameterExpression>();
        public List<ParameterExpression> DefinedArray = new List<ParameterExpression>();
        

        public LiParser(string Line, int LNumber = -1)
        {
            ScriptLine = Line;
            LineNumber = LNumber;
        }
        public LiParser(string Text)
        {
            Script = Text.Split(new string[] { "\r\n", "\r", "\n"}, StringSplitOptions.None);
            LineNumber = 1;
            ScriptLine = Script[0];
        }

        private bool TryParseSubscript(out Expression e)
        {
            Expression Ret;
            ConsumeWS();
            char ch = Read()[0];
            if (ch == '[')
            {
                if (Peek() == "]")
                {
                    e = null;
                    return true;
                }
                Ret = ParseBoolean();
                if (Peek() != "]") throw new AutoitException(AutoitExceptionType.UNBALANCEDSUBSCRIPT, LineNumber, Cursor);
                Consume();
                e = Ret;
                return true;
            }
            else
            {
                SeekRelative(-1);
                e = null;
                return false;
            }
        }
        private Expression ParsePrimary(Type Desired = null)
        {
            ConsumeWS();
            char ch = Read()[0];
            Expression Ret;
            string szTemp;
            switch (ch)
            {
                case ']':
                    throw new AutoitException(AutoitExceptionType.UNBALANCEDSUBSCRIPT, LineNumber, Cursor);
                case '(':
                    Ret = ParseBoolean();
                    if (Peek() != ")") throw new AutoitException(AutoitExceptionType.UNBALANCEDPAREN, LineNumber, Cursor);
                    Consume();
                    return Ret;
                case ')':
                    throw new AutoitException(AutoitExceptionType.UNBALANCEDPAREN, LineNumber, Cursor);
                case ';':
                    return null;

                case '$':
                    szTemp = Getstr(Reg_AlphaNum);

                    if (szTemp == "")
                    {
                        throw new AutoitException(AutoitExceptionType.LEXER_BADFORMAT, LineNumber, Cursor);
                    }
                    if (!AutoItVarCompiler.Get.ContainsKey(szTemp))
                    {
                        AutoItVarCompiler.Createvar(szTemp);
                        return Expression.Parameter(typeof(object), szTemp);
                    }
                    else return AutoItVarCompiler.Get[szTemp].ActualValue;
                    if (TryParseSubscript(out Ret))
                    {
                        return Expression.ArrayIndex(Expression.Parameter(typeof(object[]), szTemp), Ret);
                    }
                    else
                    {

                        return Expression.Parameter(typeof(object), szTemp);;
                    }
                    

                case '@':
                    szTemp = Getstr(Reg_AlphaNum);

                    if (szTemp == "")
                    {
                        throw new AutoitException(AutoitExceptionType.LEXER_BADFORMAT, LineNumber, Cursor);
                    }
                    return Expression.Call(BasicMacro.GetMacroInfo, Expression.Constant(szTemp, typeof(string)));

                case '"':
                case '\'':
                    SeekRelative(-1);
                    return Expression.Constant(Lexer_CSString(), typeof(string));
                default:
                    
                    SeekRelative(-1);
                    szTemp = GetNbr();
                    if (szTemp != "")
                    {
                        if (szTemp.Contains(".") || Math.Abs(double.Parse(szTemp, CultureInfo.InvariantCulture)) > long.MaxValue)
                            return Expression.Constant(double.Parse(szTemp, CultureInfo.InvariantCulture), typeof(double));
                        else if (Math.Abs(double.Parse(szTemp, CultureInfo.InvariantCulture)) > int.MaxValue)
                            return Expression.Constant(long.Parse(szTemp, CultureInfo.InvariantCulture), typeof(long));
                        else
                            return Expression.Constant(int.Parse(szTemp, CultureInfo.InvariantCulture), typeof(int));
                    }
                    szTemp = Getstr(Reg_AlphaNum);
                    if (szTemp != "") return ParseKeywordOrFunc(szTemp);
                    throw new AutoitException(AutoitExceptionType.LEXER_NOTRECOGNISED, LineNumber, Cursor, "" + ch);
            }
        }
        private Expression ParseUnary(Type Desired = null)
        {
            ConsumeWS();
            char ch = char.ToUpper(Read()[0]);

            switch (ch)
            {
                case '+':
                    return ParsePrimary();
                case '-':
                    return Expression.Negate(ParsePrimary());
                case 'N': if (Peek(3).ToUpper() == "OT ")
                    {
                        Consume(3);
                        return Expression.Not(ParsePrimary());
                    }
                    break;
            }
            SeekRelative(-1);
            return ParsePrimary();
        }

        public Stack<Expression> Contextual = new Stack<Expression>();
        private Expression ParseKeywordOrFunc(string Keyword, Type Desired = null)
        {
            Expression Element;
            Keyword = Keyword.ToUpper();
            switch (Keyword)
            {
                case "IF":
                    Element = ParseBoolean(false).GetOfType(VarSynchronisation, typeof(bool));
                    ConsumeWS();
                    if (Peek(4).ToUpper() == "THEN")
                    {
                        Consume(4);
                        if (!EOL) return Expression.IfThen(Element, ParseBoolean());
                        else if (!INTERCATIVE)
                        {
                            Expression Condition = Element;
                            List<Expression> Instruction = new List<Expression>();
                            Instruction.AddRange(ParseBlock());
                            Seek();
                            ConsumeWS();
                            var s = Getstr(Reg_AlphaNum).ToUpper();
                            switch (s)
                            {
                                case "ELSE":
                                    List<Expression> Otherwise = new List<Expression>();
                                    Otherwise.AddRange(ParseBlock());
                                    Seek();
                                    ConsumeWS();
                                    return Expression.IfThenElse(Condition, Expression.Block(Instruction.ToArray()), Expression.Block(Otherwise.ToArray()));
                                case "ELSEIF":
                                    return Expression.IfThenElse(Condition, Expression.Block(Instruction.ToArray()), ParseKeywordOrFunc("IF"));
                                default:
                                    return Expression.IfThen(Condition, Expression.Block(Instruction.ToArray()));
                            }
                        }
                        else throw new AutoitException(AutoitExceptionType.MULTILINEININTERACTIVE, LineNumber, Cursor);
                    }
                    else throw new AutoitException(AutoitExceptionType.MISSINGTHEN, LineNumber, Cursor, ScriptLine);
                case "WHILE":
                    Element = ParseBoolean(false).GetOfType(VarSynchronisation, typeof(bool));
                    ConsumeWS();
                    if (!EOL) 
                    {
                        var @break = Expression.Label();
                        var @continue = Expression.Label();
                        return Expression.Block(
                            Expression.Label(@continue),
                            Expression.IfThen(Expression.Not(Element), Expression.Goto(@break)),
                            ParseBoolean(),
                            Expression.Goto(@continue),
                            Expression.Label(@break));
                    }
                    else if (!INTERCATIVE)
                    {
                        var @break = Expression.Label("break");
                        var @continue = Expression.Label("continue");
                        Contextual.Push(Expression.Goto(@break));
                        Contextual.Push(Expression.Goto(@continue));
                        List<Expression> Instruction = new List<Expression>();
                        Instruction.Add(Expression.Label(@continue));
                        Instruction.Add(Expression.IfThen(Expression.Not(Element), Expression.Goto(@break)));
                        Instruction.AddRange(ParseBlock(true));
                        Instruction.Add(Expression.Goto(@continue));
                        Instruction.Add(Expression.Label(@break));
                        Contextual.Pop();
                        Contextual.Pop();
                        return Expression.Block(Instruction.ToArray());
                    }
                    else throw new AutoitException(AutoitExceptionType.MULTILINEININTERACTIVE, LineNumber, Cursor);

                case "ENDIF":
                case "NEXT":
                case "END":
                case "WEND":
                case "ENDWITH":
                    return null;
                case "EXITLOOP":
                    ConsumeWS();
                    int goback = 1;
                    string str = GetNbr();
                    if (str != "") goback = int.Parse(str);
                    if (Contextual.Count > 1 + (goback - 1) * 2) return Contextual.ElementAt(1 + (goback - 1) * 2);
                    else throw new AutoitException(AutoitExceptionType.EXITLLOOPOUTSIDELOOP, LineNumber, Cursor);
                case "CONTINUELOOP":
                    int goback2 = 1;
                    string str2 = GetNbr();
                    if (str2 != "") goback2 = int.Parse(str2);
                    if (Contextual.Count > (goback2 - 1) * 2) return Contextual.ElementAt((goback2 - 1) * 2);
                    else throw new AutoitException(AutoitExceptionType.EXITLLOOPOUTSIDELOOP, LineNumber, Cursor);
                default: //Function
                    var Func = typeof(Autcorlib).GetTypeInfo().DeclaredMethods.FirstOrDefault(x => x.Name == Keyword);
                    List<Expression> Arguments = new List<Expression>();
                    if (Func != default(MethodInfo))
                    {
                        ConsumeWS();
                        var param = Func.GetParameters();
                        if (Peek(2) != "()") while (!EOL)
                            {
                                ConsumeWS();
                                if (Peek() == ")") break;
                                Consume();
                                
                                Arguments.Add(ParseBoolean().GetOfType(VarSynchronisation, param[Arguments.Count].ParameterType));
                            }
                        else Consume();
                        Consume();
                        return Expression.Call(Func, Arguments.ToArray());
                    }
                    return null;
            }
        }
        private List<Expression> ParseBlock(bool IsLoop = false)
        {
            Expression Element;
            List<Expression> Instruction = new List<Expression>();
            var Dump = AutoItVarCompiler.Save();
            do
            {
                NextLine();
                Element = ParseBoolean();
                if (VarSynchronisation.Count > 0) Instruction.AddRange(VarSynchronisation);
                VarSynchronisation.Clear();
                if (Element != null) Instruction.Add(Element);
            }
            while (Element != null);
            if (IsLoop)
            {
                AutoItVarCompiler.Restore(Dump, VarSynchronisation);
                if (VarSynchronisation.Count > 0) Instruction.AddRange(VarSynchronisation);
                VarSynchronisation.Clear();
                Seek();
                ConsumeWS();
            }
            return Instruction;
        }

        private Expression ParseExponent(Type Desired = null)
        {
            ConsumeWS();
            Expression C1, C2;
            char ch;
            C1 = ParseUnary();
            do
            {
                ConsumeWS();
                ch = Read()[0];
                switch (ch)
                {
                    case '^':
                        C2 = ParseUnary();
                        C1 = Expression.Power(
                            C1.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)), 
                            C2.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)));
                        continue;

                }
                SeekRelative(-1);
                break;
            } while (!EOL);
            return C1;
        }
        private Expression ParseMultiplicative(Type Desired = null)
        {
            ConsumeWS();
            Expression C1, C2;
            char ch;
            C1 = ParseExponent();
            do
            {
                ConsumeWS();
                ch = Read()[0];
                switch (ch)
                {
                    case '*':
                        if (Peek() == "=") break;
                        C2 = ParseExponent();
                        C1 = Expression.Multiply(
                            C1.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)),
                            C2.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)));
                        continue;
                    case '/':
                        if (Peek() == "=") break;
                        C2 = ParseExponent();
                        C1 = Expression.Divide(
                            C1.GetOfType(VarSynchronisation, typeof(double)),
                            C2.GetOfType(VarSynchronisation, typeof(double)));
                            continue;
                    case '%':
                        C2 = ParseExponent();
                        C1 = Expression.Divide(
                            C1.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)),
                            C2.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)));
                            continue;
                }
                SeekRelative(-1);
                break;
            } while (!EOL);
            return C1;
        }
        private Expression ParseAdditive(Type Desired = null)
        {
            ConsumeWS();
            Expression C1, C2;
            char ch;
            C1 = ParseMultiplicative();
            do
            {
                ConsumeWS();
                ch = Read()[0];
                switch (ch)
                {
                    case '+':
                        if (Peek() == "=") break;
                        C2 = ParseMultiplicative();
                        C1 = Expression.Add(
                            C1.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)), 
                            C2.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)));
                        continue;
                    case '-':
                        if (Peek() == "=") break;
                        C2 = ParseMultiplicative();
                        C1 = Expression.Subtract(
                            C1.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)), 
                            C2.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)));
                            continue;
                }
                SeekRelative(-1);
                break;
            } while (!EOL);
            return C1;
        }
        private Expression ParseConcat(Type Desired = null)
        {
            ConsumeWS();
            Expression C1;
            char ch;
            C1 = ParseAdditive();
            do
            {
                ConsumeWS();
                ch = Read()[0];
                switch (ch)
                {
                    case '&':
                        C1 = Expression.Call(
                            typeof(string).GetRuntimeMethod("Concat", new Type[]{typeof(string), typeof(string)}), 
                            new Expression[] {C1.GetOfType(VarSynchronisation, typeof(string)),  
                                ParseAdditive().GetOfType(VarSynchronisation, typeof(string))});
                        continue;
                }
                SeekRelative(-1);
                break;
            } while (!EOL);
            return C1;
        }
        private Expression ParseRelationnal(bool ExpectAssign = true, Type Desired = null)
        {
            ConsumeWS();
            Expression C1, C2;
            char ch;
            C1 = ParseConcat();
            do
            {
                ConsumeWS();
                ch = Read()[0];
                switch (ch)
                {
                    case '<':
                        if (Peek() == ">")
                        {
                            Consume();
                            C2 = ParseConcat().GetOfType(VarSynchronisation, C1.Type);
                            C1 = Expression.NotEqual(C1, C2);

                        }
                        else if (Peek() == "=")
                        {
                            Consume();
                            C2 = ParseConcat();
                            C1 = Expression.LessThanOrEqual(
                                C1.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)),
                                C2.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)));
                        }
                        else
                        {
                            C2 = ParseConcat();
                            C1 = Expression.LessThan(
                                C1.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)),
                                C2.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)));
                        }
                        
                        continue;
                    case '>':
                        if (Peek() == "=")
                        {
                            Consume();
                            C2 = ParseConcat();
                            C1 = Expression.GreaterThanOrEqual(
                                C1.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)),
                                C2.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)));
                        }
                        else 
                        {
                            C2 = ParseConcat();
                            C1 = Expression.GreaterThan(
                                C1.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)),
                                C2.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)));
                        }
                        continue;
                    case '=':
                        if (Peek() == "=")
                        {
                            throw new NotSupportedException("Sorry, No support for ==");
                            C2 = ParseConcat();
                            C1 = Expression.AndAlso(Expression.TypeEqual(C1, C2.Type), Expression.Equal(C1, C2));
                        }
                        else if (ExpectAssign)
                        {
                            if (C1.NodeType == ExpressionType.Parameter || C1.NodeType == ExpressionType.ArrayIndex)
                            {
                                Consume();
                                var value = ParseBoolean(false);
                                C1 = AutoItVarCompiler.Assign((C1 as ParameterExpression).Name, value);

                            }
                            else throw new AutoitException(AutoitExceptionType.ASSIGNTONOTVARIABLE, LineNumber, Cursor, C1.ToString());
                        }
                        else
                        {
                            C2 = ParseConcat();
                            C1 = Expression.Equal(
                                C1.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)),
                                C2.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)));
                        }
                        continue;
                    case '+':
                        if (Peek() != "=" || !ExpectAssign) break;
                        Consume();
                        if (C1.NodeType == ExpressionType.Parameter || C1.NodeType == ExpressionType.ArrayIndex)
                        {
                            Consume();
                            C2 = ParseBoolean(false);
                            C1 = AutoItVarCompiler.Assign((C1 as ParameterExpression).Name, 
                                Expression.Add(
                                    AutoItVarCompiler.Access((C1 as ParameterExpression).Name, VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)),
                                    C2.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type))));

                        }
                        else throw new AutoitException(AutoitExceptionType.ASSIGNTONOTVARIABLE, LineNumber, Cursor, C1.ToString());
                        continue;
                    case '-':
                        if (Peek() != "=" || !ExpectAssign) break;
                        Consume();
                        if (C1.NodeType == ExpressionType.Parameter || C1.NodeType == ExpressionType.ArrayIndex)
                        {
                            Consume();
                            C2 = ParseBoolean(false);
                            C1 = AutoItVarCompiler.Assign((C1 as ParameterExpression).Name,
                                Expression.Subtract(
                                    AutoItVarCompiler.Access((C1 as ParameterExpression).Name, VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)),
                                    C2.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type))));

                        }
                        else throw new AutoitException(AutoitExceptionType.ASSIGNTONOTVARIABLE, LineNumber, Cursor, C1.ToString());
                        continue;
                    case '*':
                        if (Peek() != "=" || !ExpectAssign) break;
                        Consume();
                        if (C1.NodeType == ExpressionType.Parameter || C1.NodeType == ExpressionType.ArrayIndex)
                        {
                            Consume();
                            C2 = ParseBoolean(false);
                            C1 = AutoItVarCompiler.Assign((C1 as ParameterExpression).Name,
                                Expression.Multiply(
                                    AutoItVarCompiler.Access((C1 as ParameterExpression).Name, VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)),
                                    C2.GetOfType(VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type))));

                        }
                        else throw new AutoitException(AutoitExceptionType.ASSIGNTONOTVARIABLE, LineNumber, Cursor, C1.ToString());
                        continue;
                    case '/':
                        if (Peek() != "=" || !ExpectAssign) break;
                        Consume();
                        if (C1.NodeType == ExpressionType.Parameter || C1.NodeType == ExpressionType.ArrayIndex)
                        {
                            Consume();
                            C2 = ParseBoolean(false);
                            C1 = AutoItVarCompiler.Assign((C1 as ParameterExpression).Name,
                                Expression.Divide(
                                    AutoItVarCompiler.Access((C1 as ParameterExpression).Name, VarSynchronisation, typeof(double)),
                                    C2.GetOfType(VarSynchronisation, typeof(double))));

                        }
                        else throw new AutoitException(AutoitExceptionType.ASSIGNTONOTVARIABLE, LineNumber, Cursor, C1.ToString());
                        continue;
                }
                SeekRelative(-1);
                break;
            } while (!EOL);
            return C1;
        }
        private Expression ParseBoolean(bool ExpectAssign = true, Type Desired = null)
        {
            ConsumeWS();
            Expression C1;
            char ch;
            C1 = ParseRelationnal(ExpectAssign);
            do
            {
                ConsumeWS();
                ch = char.ToUpper(Read()[0]);
                switch (ch)
                {
                    case 'A': if (Peek(3).ToUpper() == "ND ")
                        {
                            Consume(3);
                            C1 = Expression.AndAlso(C1.GetOfType(VarSynchronisation, typeof(bool)), ParseRelationnal(ExpectAssign).GetOfType(VarSynchronisation, typeof(bool)));
                        }
                        else break;
                        continue;
                    case 'O': if (Peek(2).ToUpper() == "R ")
                        {
                            Consume(2);
                            C1 = Expression.OrElse(C1.GetOfType(VarSynchronisation, typeof(bool)), ParseRelationnal(ExpectAssign).GetOfType(VarSynchronisation, typeof(bool)));
                        }
                        else break;
                        continue;
                }
                SeekRelative(-1);
                break;
            } while (!EOL);
            return C1;
        }

        public static LambdaExpression Parse(string s, string LocalDir = "")
        {
            var l = new LiParser(s);
            Expression ex;
            List<Expression> Output = new List<Expression>();
            List<Expression> Vars = new List<Expression>();
            Output.Add(Expression.Call(
                typeof(Assembly).GetTypeInfo().DeclaredMethods.First(x => x.Name == "Load"), 
                Expression.Constant(typeof(LiParser).GetTypeInfo().Assembly.FullName, typeof(string))));
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
                foreach (var x in l.VarSynchronisation) Output.Add(x);
                l.VarSynchronisation.Clear();
                Output.Add(ex);
            }
            BlockExpression e = Expression.Block(AutoItVarCompiler.DefinedVars, Output.ToArray().Where(x => x != null));
            
            return Expression.Lambda<Action<string[]>>(e, Expression.Parameter(typeof(string[]), "CmdLine"));
        }
    }
}
