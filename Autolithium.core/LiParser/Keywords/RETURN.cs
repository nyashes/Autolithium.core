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
        private Expression ParseKeyword_RETURN(string Keyword)
        {
            VarSynchronisation.Add(VarCompilerEngine.Assign("Return-store", ParseBoolean(false).GetOfType(VarCompilerEngine, VarSynchronisation, typeof(object))));
            //VarSynchronisation.Add(VarCompilerEngine.Access("Return-store", VarSynchronisation, typeof(object)));
            return Contextual.Peek();
            
        }
    }
}
