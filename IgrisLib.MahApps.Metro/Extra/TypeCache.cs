using System;
using System.Runtime.InteropServices;

namespace IgrisLib
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public static class TypeCache<T>
    {
        /// <summary>
        /// 
        /// </summary>
        public static int Size { get; }

        /// <summary>
        /// 
        /// </summary>
        public static Type Type { get; }

        /// <summary>
        /// 
        /// </summary>
        public static TypeCode TypeCode { get; }

        static TypeCache()
        {
            Type = typeof(T);

            if (Type.IsEnum)
            {
                Type = Type.GetEnumUnderlyingType();
            }

            Size = Type == typeof(IntPtr) ? IntPtr.Size : Type == typeof(bool) ? 1 : Marshal.SizeOf<T>();

            TypeCode = Type.GetTypeCode(Type);
        }
    }
}
