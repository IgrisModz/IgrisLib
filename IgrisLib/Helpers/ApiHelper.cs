using System;
using System.Collections.Generic;
using System.Linq;

namespace IgrisLib.Helpers
{
    internal static class ApiHelper
    {
        private static IEnumerable<Type> apis;

        internal static IEnumerable<Type> Apis
        {
            get
            {
                return apis = apis ?? AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(x => x.GetTypes())
                        .Where(x => typeof(IApi).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
            }
        }
    }
}
