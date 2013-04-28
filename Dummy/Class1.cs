using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Dummy
{
    internal class CacheDict<TKey, TValue>
    {
        internal bool TryGetValue(MethodBase method, out ParameterInfo[] parameters)
        {
            throw new NotImplementedException();
        }
        internal ParameterInfo[] this [MethodBase b]
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }

    public static class Class1
    {
        internal static bool CanCache(this Type t)
        {
            return false;
        }

        private static readonly CacheDict<MethodBase, ParameterInfo[]> _ParamInfoCache;
        internal static ParameterInfo[] GetParametersCached(this MethodBase method)
        {

            var ASM = Assembly.LoadFrom(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            try
            {
                var Meth = ASM.GetType("GetParamCachedCBContainer").GetMethod("EXT");
                return (ParameterInfo[])Meth.Invoke(null, new object[] {method});
            }
            catch
            {
                ParameterInfo[] parameters;
                lock (_ParamInfoCache)
                {
                    if (!_ParamInfoCache.TryGetValue(method, out parameters))
                    {
                        parameters = method.GetParameters();
                        Type declaringType = method.DeclaringType;
                        if ((declaringType != null) && declaringType.CanCache())
                        {
                            _ParamInfoCache[method] = parameters;
                        }
                    }
                }
                return parameters;
            }
        }
    }
}
