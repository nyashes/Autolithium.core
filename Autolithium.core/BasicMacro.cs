using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public static class BasicMacro
    {
        public static MethodInfo GetMacroInfo = typeof(BasicMacro).GetRuntimeMethod("GetMacro", new Type[] {typeof(string)});
        public static object GetMacro(string Name)
        {
            return null;
        }
    }
}
