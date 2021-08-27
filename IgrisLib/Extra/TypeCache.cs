using System;
using System.Runtime.InteropServices;

namespace IgrisLib
{
    public static class TypeCache<T>
    {
        public static readonly int Size;
        public static readonly Type Type;
        public static readonly TypeCode TypeCode;

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
