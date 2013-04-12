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
@"$i = 0
$t = TimerInit()
while $i < 100000
$i = $i + 1
Wend
ConsoleWriteError(TimerDiff($t))
"
                ).CompileToMethod(method);
            dt.CreateType();

            Compiled.SetEntryPoint(method);
            Compiled.Save("dyn.exe");
        }
    }
}
