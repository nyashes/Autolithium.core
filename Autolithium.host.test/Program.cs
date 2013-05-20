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
            Autolithium.host.Host.CreateScript("Test",
                @"
                global $lol = 1
                func t()
                write('hello ' & $lol)
                Pause();
                endfunc");
            Autolithium.host.Host.Scripts["Test"].Main(args);
            Autolithium.host.Host.Scripts["Test"].Functions["t"].DynamicInvoke();
            Autolithium.host.Host.Scripts["Test"].GlobalVars["lol"] = "blahhhhh!!!";
            Autolithium.host.Host.Scripts["Test"].Functions["t"].DynamicInvoke();
            
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
