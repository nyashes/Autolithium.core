using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autolithium.core;
using System.Reflection;
using System.Reflection.Emit;

namespace Autolithium.compiler
{
    class Program
    {
        static void Main(string[] args)
        {
            var rgen = new Random().Next(int.MaxValue);
            var Compiled = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Autolithium"), // call it whatever you want
                AssemblyBuilderAccess.Save);

            var dm = Compiled.DefineDynamicModule("MainModule", "dyn.exe");
            var dt = dm.DefineType("dyn_type");
            var method = dt.DefineMethod(
                "Autolithium-" + new Random().Next(int.MaxValue),
                MethodAttributes.Public | MethodAttributes.Static);
            LiParser.Parse(
@"$a[2]
$a[0] = 1
$a[1] = ''
$t = TimerInit()
while $a[0] < 10000
$a[0] +=1
sin($a[0])
$a[1] = $a[1] & ' ' & $a[0] ; ==> objArray[0] = ((string) objArray[1]) + ' ' + Convert.ToString((int) objArray[0], CultureInfo.InvariantCulture); fix it
wend
ConsoleWriteerror($a[1])
ConsoleWriteerror(TimerDiff($t))
"
                /**/
                ).CompileToMethod(method);
            dt.CreateType();

            Compiled.SetEntryPoint(method);
            Compiled.Save("dyn.exe");
        }
    }
}
