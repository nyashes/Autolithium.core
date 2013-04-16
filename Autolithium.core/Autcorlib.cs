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
        public static int CONSOLEWRITEERROR(object Data)
        {
            var str = Data.ToString();
            Debug.WriteLine(str);
            return str.Length;
        }
        
        public static long INT(double n1)
        {
            return (long)n1;
        }

        #region Trigonometric functions
        public static double ACOS(double n) { return Math.Acos(n); }
        public static double ASIN(double n) { return Math.Asin(n); }
        public static double ATAN(double n) { return Math.Atan(n); }
        public static double COS (double n) { return Math.Cos(n); }
        public static double SIN (double n) { return Math.Sin(n); }
        public static double TAN (double n) { return Math.Tan(n); }
        #endregion
        #region Time management
        public static DateTime TIMERINIT()
        {
            return DateTime.Now;
        }
        public static double TIMERDIFF(DateTime Data)
        {
            return (DateTime.Now - Data).TotalMilliseconds;
        }
        public static void SLEEP(int time) { Task.Delay(time).Wait(); }
        #endregion
        #region Interop service
        #endregion
    }
}
