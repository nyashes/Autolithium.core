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
                    while (TryParseSubscript(out Ret)) { SubScript.Add(Ret); }
                    TryParseCast(out t);
                    ConsumeWS();
                    return VarAutExpression.VariableAccess(szTemp, null, t, SubScript.ToArray());

                case '@':
                    var CPos = Cursor - 1;
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
