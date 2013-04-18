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
            }
            return "Error in the error : this error is unknown :'(";
        }
    }
}
