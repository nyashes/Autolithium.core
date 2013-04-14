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
@"$i = 1
$t = TimerInit()
while $i < 10000
	$j = 2
	while $j < $i
		if int($i / $j) = $i / $j then ExitLoop
		$j += 1
	WEnd
	if $j >= $i then ConsoleWriteerror($i)
	$i += 2
WEnd
ConsoleWriteerror(TimerDiff($t))
"
                ).CompileToMethod(method);
            dt.CreateType();

            Compiled.SetEntryPoint(method);
            Compiled.Save("dyn.exe");
        }
    }
}
