using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Autolithium.host.test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Autolithium.host.Host.UseAssembly(Assembly.GetAssembly(typeof(ScriptExt)));
            Autolithium.host.Host.UseType(typeof(ScriptExt));
            var s = Autolithium.host.Host.NewScript(
                @"
                global $lol = 1
                func t()
                write('hello ' & $lol)
                Pause();
                endfunc
func setLol($value)
$lol = $value
endfunc");
            s.Main(args);
            s.GlobalScope.Functions["t"].DynamicInvoke();
            s.GlobalScope.Variables["lol"] = "blahhhhh!!!";
            s.GlobalScope.Functions["t"].DynamicInvoke();
            s.GlobalScope.Functions["setLol"].DynamicInvoke(DateTime.Now);
            s.GlobalScope.Functions["t"].DynamicInvoke();
        }
        
    }
    public class ScriptExt
    {
        public static void PAUSE()
        {
            Console.Write("Tap a key to continue..."); Console.ReadKey();
            Console.WriteLine();
        }
        public static void WRITE(string S)
        {
            Console.WriteLine(S);
        }
    }
}
