using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public static class Autcorlib
    {
        private static SortedDictionary<string, Delegate> FunctionTable = new SortedDictionary<string, Delegate>();
        private static Random RandGenerator = new Random();

        public static void FUNCTIONDEFINE(string Name)
        {
            FunctionTable.Add(Name.ToUpper(), null);
        }
        public static void FUNCTIONREGISTER(string Name, Delegate D)
        {
            FunctionTable[Name.ToUpper()] = D;
        }
        public static Delegate FUNCTIONGET(string Name)
        {
            return FunctionTable[Name.ToUpper()];
        }
        public static object CALL(string Name, params object[] Arguments)
        {
            return FunctionTable[Name.ToUpper()].DynamicInvoke(Arguments);
        }
        public static int CONSOLEWRITEERROR(object Data)
        {
            var str = Data.ToString();
            Debug.WriteLine(str);
            return str.Length;
        }

        #region Maths Functions
        public static long INT(double n1){return (long)n1;}
        public static long INT(long n1) { return n1; }
        public static int INT(int n1) { return n1; }
        public static long INT(string n1) { return Convert.ToInt64(n1, 10); }

        public static double ABS(double n1) { return Math.Abs(n1); }
        public static int ABS(int n1) { return Math.Abs(n1); }
        public static long ABS(long n1) { return Math.Abs(n1); }
        public static int ABS(string n1) { return 0; }

        public static long CEILING(double n1) { return (long)n1 + 1; }
        public static long CEILING(long n1) { return n1; }
        public static int CEILING(int n1) { return n1; }

        public static long FLOOR(double n1) { return (long)n1; }
        public static long FLOOR(long n1) { return n1; }
        public static int  FLOOR(int n1) { return n1; }

        public static long ROUND(double n1) { return (long)Math.Round(n1); }
        public static long ROUND(long n1) { return n1; }
        public static int  ROUND(int n1) { return n1; }

        public static double EXP(double n1) { return Math.Exp(n1); }
        public static double EXP(long n1) { return Math.Exp(n1); }
        public static double EXP(int n1) { return Math.Exp(n1); }
        public static double LOG(double n1) { return Math.Log(n1); }
        public static double LOG(long n1) { return Math.Log(n1); }
        public static double LOG(int n1) { return Math.Log(n1); }
        public static double SQRT(double n1) { return Math.Sqrt(n1); }
        public static double SQRT(long n1) { return Math.Sqrt(n1); }
        public static double SQRT(int n1) { return Math.Sqrt(n1); }

        public static double MOD(double n1, double n2) { return Math.IEEERemainder(n1, n2); }
        public static long MOD(long n1, long n2) { return n1 % n2; }
        public static int MOD(int n1, int n2) { return n1 % n2; }

        public static double RANDOM() { return RandGenerator.NextDouble(); }
        public static double RANDOM(double max) { return RandGenerator.NextDouble() * max; }
        public static double RANDOM(double min, double max) { return RandGenerator.NextDouble() * ABS(max - min) + min; }
        public static int RANDOMI() { return RandGenerator.Next(0, 2); }
        public static int RANDOMI(int max) { return RandGenerator.Next(0, max); }
        public static int RANDOMI(int min, int max) { return RandGenerator.Next(min, max); }

        public static void SRANDOM(int seed) { RandGenerator = new Random(seed); }
        #endregion

        #region String Mamagement
        public static int ASC(string c) { return ASCW(c); }
        public static int ASCW(string c) { return (int)c[0]; }
        public static string CHR(int c) { return CHRW(c); }
        public static string CHRW(int c) { return Convert.ToChar(c).ToString(); }

        public static long DEC(string n1)
        {
            if (n1[0] == '0')
                if (char.ToLower(n1[1]) == 'x') return Convert.ToInt64(n1.Substring(2), 16);
                else if (char.ToLower(n1[1]) == 'b') return Convert.ToInt64(n1.Substring(2), 2);
            //else return Convert.ToInt64(n1.Substring(1), 8);
            return Convert.ToInt64(n1, 16);
        }
        public static string HEX(double n1){return string.Join("", BitConverter.GetBytes(n1).Select(x => Convert.ToString(x, 16)));}
        public static string HEX(long n1) { return string.Join("", BitConverter.GetBytes(n1).Select(x => Convert.ToString(x, 16))); }
        public static string HEX(int n1) { return string.Join("", BitConverter.GetBytes(n1).Select(x => Convert.ToString(x, 16))); }

        public static string STRING(object obj) { return obj.ToString(); }
        public static string STRING(double obj) { return obj.ToString(CultureInfo.InvariantCulture); }
        public static string STRING(long obj) { return obj.ToString(CultureInfo.InvariantCulture); }
        public static string STRING(int obj) { return obj.ToString(CultureInfo.InvariantCulture); }
        public static string STRING(string obj) { return obj; }

        public static int STRINGCOMPARE(string s1, string s2, int? casesence) 
        {
            casesence = casesence ?? 0; 
            return (casesence != 1 ? s1.ToUpper() : s1).CompareTo((casesence != 1 ? s2.ToUpper() : s2)); 
        }
        public static int STRINGINSTR(string str, string substr, int? casesence, int? ocurrence, int? start, int? count)
        {
            casesence = casesence ?? 0;
            ocurrence = ocurrence ?? 1;
            count = count ?? str.Length;

            int pos = start ?? 1; pos--;

            if (ocurrence < 0)
            {
                ocurrence = -ocurrence;
                pos--;
                for (int i = 0; i < ocurrence; i++) pos = str.LastIndexOf(substr,
                    (int)pos + 1,
                    (int)count,
                    casesence != 1 ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
                return pos + 1;
            }

            pos++;
            for (int i = 0; i < ocurrence; i++) pos = str.IndexOf(substr, 
                (int)pos - 1,
                (int)count, 
                casesence != 1 ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal);
            return pos + 1;
        }
        public static bool STRINGISALNUM(string s)
        {
            return s.ToCharArray().All(x => (x >= '0' && x <= '9') || (x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z'));
        }
        public static bool STRINGISALPHA(string s)
        {
            return s.ToCharArray().All(x => (x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z'));
        }
        public static bool STRINGISASCII(string s)
        {
            return s.ToCharArray().All(x => x >= 0 && x <= 0x7f);
        }
        public static bool STRINGISDIGIT(string s)
        {
            return s.ToCharArray().All(x => x >= '0' && x <= '9');
        }
        public static bool STRINGISFLOAT(string s)
        {
            double Dummy;
            return double.TryParse(s, NumberStyles.Float | NumberStyles.AllowThousands, CultureInfo.InvariantCulture, out Dummy);
        }
        public static string STRINGFORMAT(string s, params object[] args)
        {
            return string.Format(s, args);
        }
        #endregion

        #region Trigonometric functions
        public static double ACOS(double n) { return Math.Acos(n); }
        public static double ASIN(double n) { return Math.Asin(n); }
        public static double ATAN(double n) { return Math.Atan(n); }
        public static double COS (double n) { return Math.Cos(n); }
        public static double SIN (double n) { return Math.Sin(n); }
        public static double TAN (double n) { return Math.Tan(n); }
        public static double ACOS(long n) { return Math.Acos(n); }
        public static double ASIN(long n) { return Math.Asin(n); }
        public static double ATAN(long n) { return Math.Atan(n); }
        public static double COS (long n) { return Math.Cos(n); }
        public static double SIN (long n) { return Math.Sin(n); }
        public static double TAN (long n) { return Math.Tan(n); }
        public static double ACOS(int n) { return Math.Acos(n); }
        public static double ASIN(int n) { return Math.Asin(n); }
        public static double ATAN(int n) { return Math.Atan(n); }
        public static double COS (int n) { return Math.Cos(n); }
        public static double SIN (int n) { return Math.Sin(n); }
        public static double TAN (int n) { return Math.Tan(n); }
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

        #region Array management
        public static int UBOUND(object[] i) { return i.Length; }
        #endregion
    }
}
