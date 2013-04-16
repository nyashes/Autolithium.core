using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace Autolithium.core
{
    public partial class LiParser
    {
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
                            typeof(string).GetRuntimeMethod("Concat", new Type[] { typeof(string), typeof(string) }),
                            new Expression[] {C1.GetOfType(VarSynchronisation, typeof(string)),  
                                ParseAdditive().GetOfType(VarSynchronisation, typeof(string))});
                        continue;
                }
                SeekRelative(-1);
                break;
            } while (!EOL);
            return C1;
        }
    }
}
