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
    }
}
