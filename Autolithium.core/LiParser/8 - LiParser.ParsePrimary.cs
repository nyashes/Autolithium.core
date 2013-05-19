using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public partial class LiParser
    {
        private Expression ParseUltraPrimary()
        {
            ConsumeWS();
            char ch = Read()[0];
            string szTemp;
            Expression Ret;
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
                    Type t;

                    if (szTemp == "")
                        throw new AutoitException(AutoitExceptionType.LEXER_BADFORMAT, LineNumber, Cursor);

                    List<Expression> SubScript = new List<Expression>();
                    while (TryParseSubscript(out Ret)) { szTemp += "[]"; SubScript.Add(Ret); }
                    TryParseCast(out t);
                    ConsumeWS();
                    return VarAutExpression.VariableAccess(szTemp, null, t, SubScript.ToArray());

                    /*if (TryParseSubscript(out Ret))
                    {
                        szTemp += "[]";
                        Type t;
                        TryParseCast(out t);
                        return AutExpression.VariableAccess(szTemp, null, Ret, t);
                        if (!VarCompilerEngine.Get.ContainsKey(szTemp))
                        {
                            var F = GetVar(szTemp, this, null);
                            if (F != null)
                            {
                                if (TryParseCast(out t)) return Expression.Convert(Expression.ArrayIndex(F, Ret), t);
                                return Expression.ArrayIndex(F, Ret);
                            }
                            else
                            {
                                var a = VarCompilerEngine.Createvar(szTemp);
                                VarCompilerEngine.Get[szTemp].ArrayIndex.Push(Ret);
                                a.ActualType.Add(typeof(object[]));
                                a.PolymorphList.Add(typeof(object[]), ParameterExpression.Parameter(typeof(object[]), szTemp));
                                return Expression.Assign(VarCompilerEngine.Get[szTemp].ActualValue,
                                    Expression.NewArrayBounds(typeof(object), Ret));
                            }
                        }
                        VarCompilerEngine.Get[szTemp].ArrayIndex.Push(Ret);
                        if (TryParseCast(out t))
                            VarCompilerEngine.Get[szTemp].MyType.Push(t);
                        else VarCompilerEngine.Get[szTemp].MyType.Push(null);
                        return Expression.Parameter(typeof(object[]), szTemp);
                    }
                    if (!VarCompilerEngine.Get.ContainsKey(szTemp))
                    {
                        var F = GetVar(szTemp, this, null);
                        if (F != null) return F;
                        else
                        {
                            VarCompilerEngine.Createvar(szTemp);
                            return Expression.Parameter(typeof(object), szTemp);
                        }
                    }
                    else return VarCompilerEngine.Get[szTemp].ActualValue;*/

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
                    szTemp = Getstr(Reg_Any);
                    if (szTemp != "")
                    {
                        if (szTemp.Contains("."))
                        {
                            var AType = Included.SelectMany(x => x.ExportedTypes).Concat(IncludedType) .FirstOrDefault(
                                x => {
                                    var Nr = x.FullName.ToUpper().Split('.');
                                    return Nr.SequenceEqual(szTemp.ToUpper().Split('.').Take(Nr.Count()));
                                });
                            if (AType != default(Type))
                            {
                                SeekRelative(AType.FullName.Length - szTemp.Length);
                                return Expression.Constant(null, AType.GetTypeInfo().IsValueType ? typeof(Nullable<>).MakeGenericType(AType) : AType);
                            }
                        }
                        else return ParseKeywordOrFunc(szTemp);
                    }
                    throw new AutoitException(AutoitExceptionType.LEXER_NOTRECOGNISED, LineNumber, Cursor, "" + ch);
            }
        }
        private Expression ParsePrimary(Type Desired = null)
        {

            Expression Ret;

            Ret = ParseUltraPrimary();
            ConsumeWS();
            while (Peek() == ".")
            {
                Consume();
                ConsumeWS();
                var MName = Getstr(Reg_AlphaNum);
                ConsumeWS();
                Ret = (Ret is VarAutExpression) ? (Ret as VarAutExpression).Getter(null) : Ret;
                var RetType = (Ret.Type.GetTypeInfo().IsGenericType && Ret.Type.GetGenericTypeDefinition() == typeof(Nullable<>) ?
                    Ret.Type.GetTypeInfo().GenericTypeArguments.First().GetTypeInfo() :
                    Ret.Type.GetTypeInfo());
                var RetVal = (Ret.NodeType == ExpressionType.Constant && (Ret as ConstantExpression).Value == null) ? null : Ret;
                if (Peek() == "(")
                {
                    var Args = ParseArgExpList();
                    var Method = (MethodInfo)SelectOverload(MName, ref Args, RetType);
                    Ret = Expression.Call(RetVal, Method, Args);
                }
                else Ret = Expression.MakeMemberAccess(RetVal, RetType.DeclaredMembers.First(x => x.Name == MName));
                ConsumeWS();
            } 
            return Ret;
        }        
    }
}
