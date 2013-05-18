﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public partial class LiParser
    {
        public Stack<Expression> Contextual = new Stack<Expression>();
        private Expression ParseKeywordOrFunc(string Keyword, Type Desired = null)
        {
            //Expression Element;
            Keyword = Keyword.ToUpper();
            switch (Keyword)
            {
                case "TRUE": return Expression.Constant(true, typeof(bool));
                case "FALSE": return Expression.Constant(false, typeof(bool));
                case "IF": return ParseKeyword_IF(Keyword);
                case "WHILE": return ParseKeyword_WHILE(Keyword);
                case "FOR": return ParseKeyword_FOR(Keyword);
                case "RETURN": return ParseKeyword_RETURN(Keyword);
                case "DO": return ParseKeyword_DO(Keyword);
                case "NEW": return ParseKeyword_NEW(Keyword);
                case "DEFAULT": return Expression.Constant(null);
                case "GLOBAL": return ParseKeyword_GLOBAL(Keyword);
                case "ENDIF":
                case "NEXT":
                case "END":
                case "WEND":
                case "ENDWITH":
                case "UNTIL":
                case "ENDFUNC":
                    return AutExpression.EndOfBlock();
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
                default: return Parse_CALL(Keyword);
            }
        }
        private List<Expression> ParseBlock(bool RestoreAfter = false)
        {
            Expression Element;
            List<Expression> Instruction = new List<Expression>();

            if (RestoreAfter) ExpressionTypeBeam.TakeSnapshot();
            do
            {
                NextLine();
                Element = ParseBoolean();
                if (!(Element is AutExpression) || (Element as AutExpression).ExpressionType != AutExpressionType.EndOfBlock) Instruction.Add(Element);
            }
            while (!(Element is AutExpression) || (Element as AutExpression).ExpressionType != AutExpressionType.EndOfBlock);
            if (RestoreAfter)
            {
                ExpressionTypeBeam.RestoreSnapshot();
                Seek();
                ConsumeWS();
            }
            return Instruction;
        }


        private List<Expression> ParseArgExpList()
        {
            List<Expression> Arguments = new List<Expression>();
            ConsumeWS();
            if (Peek(2) != "()") while (!EOL)
                {
                    ConsumeWS();
                    if (Peek() == ")") break;
                    else if (Peek() != "," && Peek() != "(") throw new AutoitException(AutoitExceptionType.EXPECTSYMBOL, LineNumber, Cursor, ",");
                    Consume();

                    Arguments.Add(ParseBoolean(false));
                }
            else Consume();
            Consume();
            return Arguments;
        }
        private List<ArgumentDefinition> ParseArgList()
        {
            List<ArgumentDefinition> Arguments = new List<ArgumentDefinition>();
            ArgumentDefinition Ret;
            ConsumeWS();
            if (Peek(2) != "()") 
                while (!EOL)
                {
                    ConsumeWS();
                    if (Peek() == ")") break;
                    else if (Peek() != "," && Peek() != "(") throw new AutoitException(AutoitExceptionType.EXPECTSYMBOL, LineNumber, Cursor, ",");
                    Consume();
                    ConsumeWS();

                    Ret = new ArgumentDefinition();
                    if (Getstr(Reg_AlphaNum).ToUpper() == "BYREF") Ret.IsByRef = true;
                    ConsumeWS();
                    if (Read() != "$") throw new AutoitException(AutoitExceptionType.EXPECTSYMBOL, LineNumber, Cursor, "$");
                    Ret.ArgName = Getstr(Reg_AlphaNum);
                    if (!TryParseCast(out Ret.MyType)) Ret.MyType = typeof(object);
                    ConsumeWS();
                    if (Peek() == "=")
                    {
                        Consume();
                        Ret.DefaultValue = ParseBoolean(false);
                    }
                    else Ret.DefaultValue = Expression.Constant(Ret.MyType.DefaultValue(), Ret.MyType);
                    Arguments.Add(Ret);
                }
            else Consume();
            Consume();
            return Arguments;
        }
        private dynamic SelectOverload(string Name, ref List<Expression> Params, TypeInfo Obj = null)
        {
            var ParamsRO = new List<Expression>(Params);
            var Candidates = Obj == null ?
                    Included.SelectMany(x => x.ExportedTypes)
                    .Concat(new Type[] { typeof(Autcorlib) })
                    .SelectMany(x => x.GetTypeInfo().DeclaredMethods
                    .Where(y => y.Name/*.ToUpper()*/ == Name.ToUpper() &&
                        y.GetParameters().Length >= ParamsRO.Count &&
                        y.IsStatic))
                        : Obj.DeclaredMethods.Where(y => y.Name.ToUpper() == Name.ToUpper() &&
                            y.GetParameters().Length >= ParamsRO.Count &&
                            !y.IsStatic);

            var Func = Candidates.LastOrDefault(x => x.GetParameters()
                                                    .Select(y => y.ParameterType)
                                                    .SequenceEqual(ParamsRO.Select(y => y.Type)));
            if (Func == default(MethodInfo))
            {
                Func = Candidates.LastOrDefault();
                if (Func == default(MethodInfo))
                {
                    var FuncInfo = DefinedFunctions.Where(x =>
                        x.MyName.ToUpper() == Name.ToUpper() &&
                        x.MyArguments.Count >= ParamsRO.Count)
                        .LastOrDefault();

                    if (FuncInfo == default(FunctionDefinition))
                        throw new AutoitException(AutoitExceptionType.NOFUNCMATCH, LineNumber, Cursor, Name + "(" + Params.Count + " parameters)");
                    else
                    {
                        Params.AddRange(Enumerable.Repeat(Expression.Constant(null), FuncInfo.MyArguments.Count - ParamsRO.Count));
                        Params = Params.Zip(FuncInfo.MyArguments, (x, y) =>
                            x.ConvertTo(y.MyType))
                            .ToList();
                        return FuncInfo;
                    }
                }
                else
                {
                    Params.AddRange(Enumerable.Repeat(Expression.Constant(null), Func.GetParameters().Length - ParamsRO.Count));
                    Params = Params.Zip(Func.GetParameters(), (x, y) =>
                        x.ConvertTo(y.ParameterType))
                        .ToList();
                    return Func;
                }
            }
            else return Func;
        }
        private dynamic SelectOverload(string Name, ref List<Expression> Params, Expression Obj)
        {
            return SelectOverload(Name, ref Params, Obj == null ? null : Obj.Type.GetTypeInfo());
        }
    }
}
