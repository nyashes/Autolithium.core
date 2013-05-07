using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autolithium.core;
using System.Reflection;
using System.Reflection.Emit;
using System.IO;

namespace Autolithium.compiler
{
    partial class Program
    {
        static void Main(string[] args)
        {
            char[] options = args.Where(x => x.StartsWith("-")).SelectMany(x => x.Substring(1).ToCharArray()).ToArray();
            args = args.Where(x => !x.StartsWith("-")).ToArray();
            if (args.Length < 2)
            {
                Console.WriteLine("Autolithium command line tool : ");
                Console.WriteLine("Autolithium.compiler.exe \"MainSource.aul\" \"DestinationFile.(exe or dll)\" [Optional] \"Destination plateform\" [Default : Win]");
                Console.WriteLine("Options: ");
                Console.WriteLine("\t- -r : run after generation");
                Console.Write("Press any key to exit...");
                Console.ReadKey();
                return;
            }
            string Script = File.ReadAllText(args[0]);
            var Compiled = AppDomain.CurrentDomain.DefineDynamicAssembly(
                new AssemblyName("Autolithium-" + Path.GetFileName(args[1])), // call it whatever you want
                AssemblyBuilderAccess.Save);

            var dm = Compiled.DefineDynamicModule("MainModule",  Path.GetFileName(args[1]));
            
            List<Assembly> Include = new List<Assembly>() { ASMInclude.IncludeDLL(dm, @"Autolithium.core.dll", "dyn_type") };
            if (args.Length < 3) { Array.Resize(ref args, 3); args[2] = "win"; }
            switch(args[2].ToLower())
            {
                case "win":
                    Include.Add(ASMInclude.IncludeDLL(dm, @"C:\Users\nem-e_000\Documents\Visual Studio 2012\Projects\Autolithium.core\Autolithium.ion\bin\Debug\Autolithium.ion.dll", "dyn_type"));
                    Include.Add(ASMInclude.RequireNetFXASM("System, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
                    Include.Add(ASMInclude.RequireNetFXASM("mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089"));
                    break;           
            }
            var Main = CompileMain(dm, Script, Include.ToArray());
            if (Main != null) Compiled.SetEntryPoint(Main);
            Compiled.Save(args[1]);
            if (options.Contains('r')) System.Diagnostics.Process.Start(args[1]);
        }
    }
}
