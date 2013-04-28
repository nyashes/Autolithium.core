using Autolithium.core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.compiler
{
    public static class GenerateMultiCastDelegate
    {
        public static Type CreateDelegateFor(this ModuleBuilder ASM, FunctionDefinition F)
        {
            var typeBuilder = ASM.DefineType("&=" + F.MyName, TypeAttributes.Class | TypeAttributes.Public | TypeAttributes.Sealed | TypeAttributes.AnsiClass | TypeAttributes.AutoClass, typeof(System.MulticastDelegate));
            ConstructorBuilder constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.RTSpecialName | MethodAttributes.HideBySig | MethodAttributes.Public, CallingConventions.Standard, new Type[] { typeof(object), typeof(System.IntPtr) });
            constructorBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            // Grab the parameters of the method
            Type[] paramTypes = F.MyArguments.Select(x => x.MyType).ToArray();

            // Define the Invoke method for the delegate
            var methodBuilder = typeBuilder.DefineMethod("Invoke", MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual, F.ReturnType, paramTypes);
            methodBuilder.SetImplementationFlags(MethodImplAttributes.Runtime | MethodImplAttributes.Managed);

            // bake it!
            return typeBuilder.CreateType();
        }
    }
}
