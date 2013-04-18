using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public partial class LiParser
    {
        private Expression ParseUnary(Type Desired = null)
        {
            ConsumeWS();
            char ch = char.ToUpper(Read()[0]);

            switch (ch)
            {
                case '+':
                    return ParsePrimary();
                case '-':
                    return Expression.Negate(ParsePrimary().GetOfType(VarCompilerEngine, VarSynchronisation, ExpressionExtension.Numeric));
                case 'N': if (Peek(3).ToUpper() == "OT ")
                    {
                        Consume(3);
                        return Expression.Not(ParsePrimary().GetOfType(VarCompilerEngine, VarSynchronisation, typeof(bool)));
                    }
                    break;
            }
            SeekRelative(-1);
            return ParsePrimary();
        }
    }
}
