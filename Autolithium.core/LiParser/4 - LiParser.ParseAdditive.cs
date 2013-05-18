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
                            C1.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)),
                            C2.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)));
                        continue;
                    case '-':
                        if (Peek() == "=") break;
                        C2 = ParseMultiplicative();
                        C1 = Expression.Subtract(
                            C1.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)),
                            C2.ConvertTo(ExpressionTypeBeam.Largest(C1.Type, C2.Type)));
                        continue;
                }
                SeekRelative(-1);
                break;
            } while (!EOL);
            return C1;
        }
    }
}
