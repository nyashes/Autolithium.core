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
                            C1.GetOfType(VarCompilerEngine, VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)),
                            C2.GetOfType(VarCompilerEngine, VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)));
                        continue;

                }
                SeekRelative(-1);
                break;
            } while (!EOL);
            return C1;
        }
    }
}
