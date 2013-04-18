using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autolithium.core;
using System.Reflection;
using System.Reflection.Emit;

namespace Autolithium.compiler
{
    partial class Program
    {
        static void Main(string[] args)
        {
            string Script =
@"$a[2]
$a[0] = 1
$a[1] = ''
$t = TimerInit()
while $a[0]::int < 10000
$a[0]::int +=1
sin($a[0]::int)
$a[1] = $a[1]::string & ' ' & $a[0]::int
wend
;ConsoleWriteerror($a[1])
alert(TimerDiff($t))
lol()
plop($t)
func lol()
alert('kikoo')
endfunc
func plop($test)
return 'plop ' & $test
endfunc
";
            var rgen = new Random();
            var Compiled = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Autolithium"), // call it whatever you want
                AssemblyBuilderAccess.Save);

            var dm = Compiled.DefineDynamicModule("MainModule", "dyn.exe");
            
            var dt = dm.DefineType("dyn_type");
            var Loader = ASMInclude.ResxLoader(dt);
            ReadMethodsDefinition(dt, Script);
            var Main = CompileMain(dt, Script,
                ASMInclude.IncludeDLL(dm, @"C:\Users\nem-e_000\Documents\Visual Studio 2012\Projects\Autolithium.core\Autolithium.core\bin\Debug\Autolithium.core.dll", "dyn_type"),
                ASMInclude.IncludeDLL(dm, @"C:\Users\nem-e_000\Documents\Visual Studio 2012\Projects\Autolithium.core\Autolithium.ion\bin\Debug\Autolithium.ion.dll", "dyn_type"));
            CreateMethods(dt, Script);
            dt.CreateType();
            Compiled.SetEntryPoint(Main);
            Compiled.Save("dyn.exe");
        }
    }
}
