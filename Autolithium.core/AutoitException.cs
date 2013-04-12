using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public class AutoitException : Exception
    {
        public AutoitExceptionType Type;
        int Line, Char;
        string ErrorString;
        public AutoitException(AutoitExceptionType t, int line, int chr, string ProblematicString = "") { Type = t; Line = line; Char = chr; ErrorString = ProblematicString; }
        public override string Source
        {
            get
            {
                return "\nLine: " + Line + "\nChar: " + Char ;
            }
            set
            {
                base.Source = value;
            }
        }
        public override string Message
        {
            get
            {
                return string.Format(AutoitErrorMSG.of(Type), Line, Char, ErrorString);
            }
        }
    }
}
