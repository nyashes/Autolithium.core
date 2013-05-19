/*Copyright or © or Copr. THOUVENIN Alexandre (2013)

nem-e5i5software@live.fr

This software is a computer program whose purpose is to [describe
functionalities and technical features of your software].

This software is governed by the CeCILL-C license under French law and
abiding by the rules of distribution of free software.  You can  use, 
modify and/ or redistribute the software under the terms of the CeCILL-C
license as circulated by CEA, CNRS and INRIA at the following URL
"http://www.cecill.info". 

As a counterpart to the access to the source code and  rights to copy,
modify and redistribute granted by the license, users are provided only
with a limited warranty  and the software's author,  the holder of the
economic rights,  and the successive licensors  have only  limited
liability. 

In this respect, the user's attention is drawn to the risks associated
with loading,  using,  modifying and/or developing or reproducing the
software by the user in light of its specific status of free software,
that may mean  that it is complicated to manipulate,  and  that  also
therefore means  that it is reserved for developers  and  experienced
professionals having in-depth computer knowledge. Users are therefore
encouraged to load and test the software's suitability as regards their
requirements in conditions enabling the security of their systems and/or 
data to be ensured and,  more generally, to use and operate it in the 
same conditions as regards security. 

The fact that you are presently reading this means that you have had
knowledge of the CeCILL-C license and that you accept its terms.*/

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
