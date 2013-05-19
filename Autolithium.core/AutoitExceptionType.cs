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

namespace Autolithium.core
{
    public enum AutoitExceptionType
    {
        LEXER_NOTRECOGNISED,
        LEXER_BADFORMAT,
        TOOMANYELSE,
        UNBALANCEDPAREN,
        UNBALANCEDSUBSCRIPT,
        MISSINGIFBEFORETHEN,
        MISSINGTHEN,
        ELSENOMATCHINGIF,
        MULTILINEININTERACTIVE,
        ASSIGNTONOTVARIABLE,
        UNASSIGNEDVARIABLE,
        MISSINGVAR,
        EXITLLOOPOUTSIDELOOP,
        FORWITHOUTTO,
        NOFUNCMATCH,
        EXPECTUNTIL,
        CLASSDOESNOTEXIXTS,
        CONSTRUCTORMISMATCH,
        EXPECTVAR,
        EXPECTSYMBOL,
    }
    public static class AutoitErrorMSG
    {
        public static string of(AutoitExceptionType e)
        {
            switch (e)
            {
                case AutoitExceptionType.LEXER_NOTRECOGNISED:
                    return "The symbol {2} at ({0}, {1}) is unknown to me";
                case AutoitExceptionType.LEXER_BADFORMAT:
                    return "({0}, {1}) Badly formated variable or macro : {2}";
                case AutoitExceptionType.TOOMANYELSE:
                    return "({0}, {1}) Too many \"Else\" statements for matching \"If\" statement.";
                case AutoitExceptionType.UNBALANCEDPAREN:
                    return "({0}, {1}) Unbalanced brackets in expression.";
                case AutoitExceptionType.UNBALANCEDSUBSCRIPT:
                    return "({0}, {1}) Unbalanced subscript in expression.";
                case AutoitExceptionType.MISSINGIFBEFORETHEN:
                    return "({0}, {1}) This then is not preceded by any keyword, previous keyword : {2}.";
                case AutoitExceptionType.ELSENOMATCHINGIF:
                    return "({0}, {1}) \"Else\" statement with no matching \"If\" statement.";
                case AutoitExceptionType.MULTILINEININTERACTIVE:
                    return "({0}, {1}) You cannot use a multiline statement ({2}) in interactive mode.";
                case AutoitExceptionType.MISSINGTHEN:
                    return "({0}, {1}) \"If\" is not followed by \"Then\" in : {2}.";
                case AutoitExceptionType.ASSIGNTONOTVARIABLE:
                    return "({0}, {1}) You try to assign something to {2} which is not a variable.";
                case AutoitExceptionType.UNASSIGNEDVARIABLE:
                    return "AutoItVarCompiler: The variable \"{2}\" is used before being assigned";
                case AutoitExceptionType.MISSINGVAR:
                    return "AutoItVarCompiler: The variable \"{2}\" does not exists";
                case AutoitExceptionType.EXITLLOOPOUTSIDELOOP:
                    return "({0}, {1}) You try to exit or continue a loop outside a loop";
                case AutoitExceptionType.FORWITHOUTTO:
                    return "({0}, {1}) A for loop must be 'for $var = first_value to last_value [step number_to_add]'";
                case AutoitExceptionType.NOFUNCMATCH:
                    return "({0}, {1}) There is no function to match {2}";
                case AutoitExceptionType.EXPECTUNTIL:
                    return "({0}, {1}) The keyword \"until\" is expected, got {2}";
                case AutoitExceptionType.CLASSDOESNOTEXIXTS:
                    return "({0}, {1}) The class \"{2}\" does not exists";
                case AutoitExceptionType.CONSTRUCTORMISMATCH:
                    return "({0}, {1}) Constructor mismatch : unable to get a good one";
                case AutoitExceptionType.EXPECTVAR:
                    return "({0}, {1}) Expect a variable after {2}";
                case AutoitExceptionType.EXPECTSYMBOL:
                    return "({0}, {1}) The symbol \"{2}\" is expected";
            }
            return "Error in the error : this error is unknown :'(";
        }
    }
}
