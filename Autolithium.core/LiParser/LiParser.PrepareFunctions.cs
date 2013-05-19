/*Copyright or © or Copr. THOUVENIN Alexandre (2013)

nem-e5i5software@live.fr

This software is a computer program whose purpose is to [describe
functionalities and technical features of your software].

This software is governed by the CeCILL-C license under French law and
abiding by the rules of distribution of free software.  You can  use, 
modify and/ or redistribute the software under the terms of the CeCILL-C
license as circulated by CEA, CNRS and INRIA at the following URL
"http://www.cecill.info". 

As a counterpart to the access to the source code and  rights to copy,
modify and redistribute granted by the license, users are provided only
with a limited warranty  and the software's author,  the holder of the
economic rights,  and the successive licensors  have only  limited
liability. 

In this respect, the user's attention is drawn to the risks associated
with loading,  using,  modifying and/or developing or reproducing the
software by the user in light of its specific status of free software,
that may mean  that it is complicated to manipulate,  and  that  also
therefore means  that it is reserved for developers  and  experienced
professionals having in-depth computer knowledge. Users are therefore
encouraged to load and test the software's suitability as regards their
requirements in conditions enabling the security of their systems and/or 
data to be ensured and,  more generally, to use and operate it in the 
same conditions as regards security. 

The fact that you are presently reading this means that you have had
knowledge of the CeCILL-C license and that you accept its terms.*/

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
