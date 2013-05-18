using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Autolithium.core
{
    public delegate Expression GetVarDelegate(string Name, Type Desired);
    public delegate void  CreateVarDelegate(string Name, Type Desired);
    public delegate Expression SetVarDelegate(string Name, Expression Value);
    public partial class LiParser
    {
        protected internal GetVarDelegate GetVar;
        protected internal CreateVarDelegate CreateVar;
        protected internal SetVarDelegate SetVar;

        private Expression ParseKeyword_GLOBAL(string Keyword)
        {
            ConsumeWS();
            if (Read() != "$") throw new AutoitException(AutoitExceptionType.EXPECTVAR, LineNumber, Cursor, Keyword);
            string Name = Getstr(Reg_AlphaNum);
            Type T = null;
            Expression Subs;
            if (TryParseSubscript(out Subs)) Name += "[]";
            if (!TryParseCast(out T)) T = typeof(object);
            if (Subs != null) T = T.MakeArrayType();
            this.CreateVar(Name, T);
            ConsumeWS();
            if (T.IsArray)
            {
                return SetVar(Name, Expression.NewArrayBounds(T.GetElementType(), Subs));
            }
            else if (Peek() == "=")
            {
                Consume();
                return SetVar(Name, ParseBoolean());
            }
            return null;
        }
    }
}
