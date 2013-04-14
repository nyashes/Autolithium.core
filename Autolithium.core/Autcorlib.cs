using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public static class Autcorlib
    {
        public static AutoItWeakObject CONSOLEWRITEERROR(object Data)
        {
            var str = Data.ToString();
            Debug.WriteLine(str);
            return str.Length;
        }
        public static double TIMERINIT()
        {
            return (DateTime.Now - DateTime.MinValue).TotalSeconds;
        }
        public static double TIMERDIFF(double Data)
        {
            return (DateTime.Now - DateTime.MinValue).TotalSeconds - Data;
        }
        public static long INT(double n1)
        {
            return (long)n1;
        }
    }
}
