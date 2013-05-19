using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public partial class LiParser
    {
        public List<Expression> DefineGlobal()
        {
            var Matches = Script.Where(x => Regex.IsMatch(x, "^(?:\t| )*(global(?:.*?))$", RegexOptions.IgnoreCase)).ToList();
            var Lines = Matches.Select(x => Array.IndexOf(Script, x));
            List<Expression> Ret = new List<Expression>();
            foreach (var L in Lines)
            {
                this.GotoLine(L);
                ConsumeWS();
                if (Read(6).ToUpper() != "GLOBAL") throw new Exception("WHAT'S THE FU.U.U..U.U ....");
                Ret.Add(ParseKeyword_GLOBAL("GLOBAL"));
            }
            return Ret;
        }
    }
}
