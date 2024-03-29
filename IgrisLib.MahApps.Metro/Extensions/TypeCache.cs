﻿using System;
using System.Runtime.InteropServices;

namespace IgrisLib
{
    /// <summary>
    /// Classe for gets the structure info.
    /// </summary>
    /// <typeparam name="T">The structure type.</typeparam>
    public static class TypeCache<T>
    {
        /// <summary>
        /// Get the size of the structure.
        /// </summary>
        public static int Size { get; }

        /// <summary>
        /// Get the type of the structure.
        /// </summary>
        public static Type Type { get; }

        /// <summary>
        /// Get the typeCode of the structure.
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
