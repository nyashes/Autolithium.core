using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public delegate void DefineFuncDelegate(FunctionDefinition FDef);
    public partial class LiParser
    {
        protected DefineFuncDelegate DefineFunc;
        public void DefineFunction()
        {
            var Matches = Script.Where(x => Regex.IsMatch(x, "^(?:\t| )*func(.*?)$", RegexOptions.IgnoreCase)).ToList();
            var Lines = Matches.Select(x =>
                new
                {
                    Position = Array.IndexOf(Script, x),
                    Signature = x.GetHashCode()
                });
            FunctionDefinition Def;
            foreach (var L in Lines)
            {
                this.GotoLine(L.Position);
                Def = new FunctionDefinition();
                Def.DefinitionSignature = L.Signature;
                ConsumeWS();
                if (Read(4).ToUpper() != "FUNC") throw new Exception("WHAT'S THE FU.U.U..U.U ....");
                ConsumeWS();
                Def.MyName = Getstr(Reg_AlphaNum);
                if (Peek() != "(") throw new AutoitException(AutoitExceptionType.EXPECTSYMBOL, LineNumber, Cursor, "(");
                Def.MyArguments = ParseArgList();
                if (!TryParseCast(out Def.ReturnType)) Def.ReturnType = typeof(object);
                DefineFunc(Def);
                DefinedFunctions.Add(Def);
            }
        }
    }
}
