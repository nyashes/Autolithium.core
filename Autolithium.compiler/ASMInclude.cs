using Autolithium.core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.compiler
{
    public static class ASMInclude
    {
        public static Assembly IncludeDLL(ModuleBuilder ASM, string Filen, string DestDir = null)
        {
            DestDir = DestDir ?? Directory.GetCurrentDirectory();
            var fstream = File.OpenRead(Filen);
            ASM.DefineManifestResource(Path.GetFileName(Filen), fstream, ResourceAttributes.Public);
            
            //r.Generate();
            return Assembly.LoadFrom(Filen);
        }
        public static ConstructorBuilder ResxLoader(TypeBuilder T)
        {
            var Method = T.DefineMethod("&=EventH", MethodAttributes.Static | MethodAttributes.Public, typeof(Assembly), new Type[] {typeof(object), typeof(ResolveEventArgs)});
            ILGenerator EH = Method.GetILGenerator();

            EH.DeclareLocal(typeof(string));
            EH.DeclareLocal(typeof(Stream));
            EH.DeclareLocal(typeof(byte[]));
            EH.DeclareLocal(typeof(Assembly));

            EH.Emit(OpCodes.Nop);
            EH.Emit(OpCodes.Ldstr, "");
            EH.Emit(OpCodes.Ldarg_1); 
            EH.Emit(OpCodes.Callvirt, typeof(ResolveEventArgs).GetMethod("get_Name"));
            EH.Emit(OpCodes.Newobj, typeof(AssemblyName).GetConstructor(new Type[] {typeof(string)}));
            EH.Emit(OpCodes.Call, typeof(AssemblyName).GetMethod("get_Name"));
            EH.Emit(OpCodes.Ldstr, ".dll");
            EH.Emit(OpCodes.Call, typeof(string).GetTypeInfo().GetMethod("Concat", new Type[] {typeof(string), typeof(string), typeof(string)}));
            EH.Emit(OpCodes.Stloc_0);
            EH.Emit(OpCodes.Ldtoken, T);
            EH.Emit(OpCodes.Call, typeof(Type).GetMethod("GetTypeFromHandle"));
            EH.Emit(OpCodes.Callvirt, typeof(Type).GetProperty("Assembly").GetGetMethod());
            EH.Emit(OpCodes.Ldloc_0); 
            EH.Emit(OpCodes.Callvirt, typeof(Assembly).GetMethod("GetManifestResourceStream", new Type[] {typeof(string)}));
            EH.Emit(OpCodes.Stloc_1);
            EH.Emit(OpCodes.Ldloc_1);
            EH.Emit(OpCodes.Callvirt, typeof(Stream).GetProperty("Length").GetGetMethod());
            EH.Emit(OpCodes.Conv_Ovf_I);
            EH.Emit(OpCodes.Newarr, typeof(byte));
            EH.Emit(OpCodes.Stloc_2);
            EH.Emit(OpCodes.Ldloc_1); 
            EH.Emit(OpCodes.Ldloc_2); 
            EH.Emit(OpCodes.Ldc_I4_0); 
            EH.Emit(OpCodes.Ldloc_2); 
            EH.Emit(OpCodes.Ldlen); 
            EH.Emit(OpCodes.Conv_I4);
            EH.Emit(OpCodes.Callvirt, typeof(Stream).GetMethod("Read", new Type[] {typeof(byte[]), typeof(int), typeof(int)}));
            EH.Emit(OpCodes.Pop);
            EH.Emit(OpCodes.Ldloc_2);
            EH.Emit(OpCodes.Call, typeof(Assembly).GetMethod("Load", new Type[] {typeof(byte[])}));
            EH.Emit(OpCodes.Stloc_3);
            var s = EH.DefineLabel();
            EH.Emit(OpCodes.Br_S, s);
            EH.MarkLabel(s);
            EH.Emit(OpCodes.Ldloc_3);
            
            EH.Emit(OpCodes.Ret);

            

            var Ret = T.DefineConstructor(MethodAttributes.Static | MethodAttributes.Private, System.Reflection.CallingConventions.Standard, new Type[]{});
            ILGenerator IL = Ret.GetILGenerator();
            IL.Emit(OpCodes.Nop);
            IL.EmitCall(OpCodes.Call, typeof(AppDomain).GetTypeInfo().GetDeclaredProperty("CurrentDomain").GetGetMethod(), null);
            IL.Emit(OpCodes.Ldnull);
            IL.Emit(OpCodes.Ldftn, Method);
            IL.Emit(OpCodes.Newobj,
                typeof(ResolveEventHandler).GetTypeInfo().GetConstructors().First());
            IL.EmitCall(OpCodes.Callvirt, typeof(AppDomain).GetTypeInfo().GetMethod("add_AssemblyResolve"), null);
            IL.Emit(OpCodes.Nop);
            IL.Emit(OpCodes.Ret);
            return Ret;

        }
        public static Assembly RequireDLL(string Filen, string DestDir = null)
        {
            DestDir = DestDir ?? Directory.GetCurrentDirectory();
            File.Copy(Filen, Path.Combine(DestDir, Path.GetFileName(Filen)), true);
            return Assembly.LoadFrom(Filen);
        }
    }
}
