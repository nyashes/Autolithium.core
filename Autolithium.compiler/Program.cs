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
@"$t = TimerInit()
for $i = 1 to 10000 step 2
	for $j = 2 to $i
		if int($i / $j) = $i / $j then ExitLoop
	next
	if $j >= $i then ConsoleWriteerror($i)
next
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
