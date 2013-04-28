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
                            C1.GetOfType(VarCompilerEngine, VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)),
                            C2.GetOfType(VarCompilerEngine, VarSynchronisation, ExpressionExtension.LargestNumeric(C1.Type, C2.Type)));
                        continue;
                    case '/':
                        if (Peek() == "=") break;
                        C2 = ParseExponent();
                        C1 = Expression.Divide(
                            C1.GetOfType(VarCompilerEngine, VarSynchronisation, typeof(double)),
                            C2.GetOfType(VarCompilerEngine, VarSynchronisation, typeof(double)));
                        continue;
                    case '%':
                        C2 = ParseExponent();
                        C1 = Expression.Modulo(
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
