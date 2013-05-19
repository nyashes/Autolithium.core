using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Autolithium.core
{
    public class ArgumentDefinition
    {
        public Type MyType = typeof(object);
        public string ArgName;
        public bool IsByRef = false;
        public Expression DefaultValue = Expression.Constant(null);
    }
    public class FunctionDefinition
    {
        public int DefinitionSignature;
        public string MyName;
        public Type ReturnType = typeof(object);
        public List<ArgumentDefinition> MyArguments = new List<ArgumentDefinition>();
        public MethodInfo Body;
        public Type Delegate;
        public Expression DelegateAccess;
        public object AditionnalInfo;
    }
}
