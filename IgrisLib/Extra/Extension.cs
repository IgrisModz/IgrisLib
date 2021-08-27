using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace IgrisLib
{
    /// <summary>
    /// Extension functions class
    /// </summary>
    public class Extension
    {
        private readonly IApi api;

        /// <summary>
        /// Instance to use Native functions
        /// </summary>
        public Native ExtNative { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="api">Api to Read/Write Memory</param>
        public Extension(IApi api)
        {
            this.api = api;
            ExtNative = new Native(this);
        }

        private Extension()
        {
        }

        /// <summary>
        /// Convert the <see cref="string"/> to byte array.
        /// </summary>
        /// <param name="text">Put your text to convert!</param>
        /// <param name="encoding">Text encoding</param>
        /// <returns>The byte array converted from the text.</returns>
        public byte[] StringToByteArray(string text, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            return encoding.GetBytes(text);
        }

        /// <summary>
        /// Convert the byte array to string.
        /// </summary>
        /// <param name="bytes">Put the byte array to convert!</param>
        /// <param name="encoding">Encoding for conversion to text</param>
        /// <returns>The converted text from the byte array.</returns>
        public string ByteArrayToString(byte[] bytes, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }
            return encoding.GetString(bytes);
        }

        /// <summary>
        /// Compares two byte arrays with each other
        /// </summary>
        /// <param name="a">First byte array to compare</param>
        /// <param name="b">Second byte array to compare</param>
        /// <returns><see cref="bool">true</see> if the two byte arrays are identical</returns>
        public bool CompareByteArray(byte[] a, byte[] b)
        {
            int num = 0;
            for (int i = 0; i < a.Length; i++)
            {
                if (a[i] == b[i])
                {
                    num++;
                }
            }
            return num == a.Length;
        }

        /// <summary>
        /// Convert the <see cref="float"/> to a byte array.
        /// </summary>
        /// <param name="axis">Float to convert</param>
        /// <returns>The byte array converted from the float</returns>
        public byte[] ToHexFloat(float axis)
        {
            byte[] bytes = BitConverter.GetBytes(axis);
            Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Convert the <see cref="uint"/> to a byte array.
        /// </summary>
        /// <param name="input"><see cref="uint"/> to convert</param>
        /// <returns>The byte array converted from the <see cref="uint"/></returns>
        public byte[] UInt32Bytes(uint input)
        {
            byte[] bytes = BitConverter.GetBytes(input);
            Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>
        /// Write an integer 32 bits with Logical OR.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void Or_Int32(uint offset, int input)
        {
            WriteInt32(offset, ReadInt32(offset) | input);
        }

        /// <summary>
        /// Write an integer 32 bits with Logical AND.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void And_Int32(uint offset, int input)
        {
            WriteInt32(offset, ReadInt32(offset) & input);
        }

        #region Read
        /// <summary>
        /// Read a byte a check if his value. This return a bool according the byte detected.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <returns>The boolean of the offset.</returns>
        public bool ReadBool(uint offset)
        {
            return GetBytes(offset, 1)[0] != 0;
        }

        /// <summary>
        /// Read and return a byte.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <returns>The byte of the offset.</returns>
        public byte ReadByte(uint offset)
        {
            return GetBytes(offset, 1)[0];
        }

        /// <summary>
        /// Read and return an array of bytes.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="length">Length of memory to get.</param>
        /// <returns>The bytes of the offset.</returns>
        public byte[] ReadBytes(uint offset, int length)
        {
            return GetBytes(offset, length);
        }

        /// <summary>
        /// Read a signed byte.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <returns>The sbyte of the offset.</returns>
        public sbyte ReadSByte(uint offset)
        {
            return (sbyte)GetBytes(offset, 1)[0];
        }

        /// <summary>
        /// Read and return an array of signed bytes.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="length">Length of memory to get.</param>
        /// <returns>The sbytes of the offset.</returns>
        public sbyte[] ReadSBytes(uint offset, int length)
        {
            return (sbyte[])(Array)GetBytes(offset, length);
        }

        /// <summary>
        /// Read and return an integer 16 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <returns>The Int16 of the offset.</returns>
        public short ReadInt16(uint offset)
        {
            byte[] memory = GetBytes(offset, 2);
            Array.Reverse(memory, 0, 2);
            return BitConverter.ToInt16(memory, 0);
        }

        /// <summary>
        /// Read and return an array of integer 16 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="length">Length of memory to get.</param>
        /// <returns>The Int16 array of the offset.</returns>
        public short[] ReadInt16(uint offset, int length)
        {
            byte[] memory = GetBytes(offset, length * 2);
            Array.Reverse(memory);
            short[] numArray = new short[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToInt16(memory, (length - 1 - i) * 2);
            }
            return numArray;
        }

        /// <summary>
        /// Read and return an unsigned integer 16 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <returns>The UInt16 of the offset.</returns>
        public ushort ReadUInt16(uint offset)
        {
            byte[] memory = GetBytes(offset, 2);
            Array.Reverse(memory, 0, 2);
            return BitConverter.ToUInt16(memory, 0);
        }

        /// <summary>
        /// Read and return an array of unsigned integer 16 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="length">Length of memory to get.</param>
        /// <returns>The UInt16 array of the offset.</returns>
        public ushort[] ReadUInt16(uint offset, int length)
        {
            byte[] memory = GetBytes(offset, length * 2);
            Array.Reverse(memory);
            ushort[] numArray = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToUInt16(memory, (length - 1 - i) * 2);
            }
            return numArray;
        }

        /// <summary>
        /// Read and return an integer 32 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <returns>The Int32 of the offset.</returns>
        public int ReadInt32(uint offset)
        {
            byte[] memory = GetBytes(offset, 4);
            Array.Reverse(memory, 0, 4);
            return BitConverter.ToInt32(memory, 0);
        }

        /// <summary>
        /// Read and return an array of integer 32 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="length">Length of memory to get.</param>
        /// <returns>The Int32 array of the offset.</returns>
        public int[] ReadInt32(uint offset, int length)
        {
            byte[] memory = GetBytes(offset, length * 4);
            Array.Reverse(memory);
            int[] numArray = new int[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToInt32(memory, (length - 1 - i) * 4);
            }
            return numArray;
        }

        /// <summary>
        /// Read and return an unsigned integer 32 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <returns>The UInt32 of the offset.</returns>
        public uint ReadUInt32(uint offset)
        {
            byte[] memory = GetBytes(offset, 4);
            Array.Reverse(memory, 0, 4);
            return BitConverter.ToUInt32(memory, 0);
        }

        /// <summary>
        /// Read and return an array of unsigned integer 32 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="length">Length of memory to get.</param>
        /// <returns>The UInt32 array of the offset.</returns>
        public uint[] ReadUInt32(uint offset, int length)
        {
            byte[] memory = GetBytes(offset, length * 4);
            Array.Reverse(memory);
            uint[] numArray = new uint[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToUInt32(memory, (length - 1 - i) * 4);
            }
            return numArray;
        }

        /// <summary>
        /// Read and return an integer 64 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <returns>The Int64 of the offset.</returns>
        public long ReadInt64(uint offset)
        {
            byte[] memory = GetBytes(offset, 8);
            Array.Reverse(memory, 0, 8);
            return BitConverter.ToInt64(memory, 0);
        }

        /// <summary>
        /// Read and return an array of integer 64 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="length">Length of memory to get.</param>
        /// <returns>The Int64 array of the offset.</returns>
        public long[] ReadInt64(uint offset, int length)
        {
            byte[] memory = GetBytes(offset, length * 8);
            Array.Reverse(memory);
            long[] numArray = new long[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToInt64(memory, (length - 1 - i) * 8);
            }
            return numArray;
        }

        /// <summary>
        /// Read and return an unsigned integer 64 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <returns>The UInt64 of the offset.</returns>
        public ulong ReadUInt64(uint offset)
        {
            byte[] memory = GetBytes(offset, 8);
            Array.Reverse(memory, 0, 8);
            return BitConverter.ToUInt64(memory, 0);
        }

        /// <summary>
        /// Read and return an array of unsigned integer 64 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="length">Length of memory to get.</param>
        /// <returns>The UInt64 array of the offset.</returns>
        public ulong[] ReadUInt64(uint offset, int length)
        {
            byte[] memory = GetBytes(offset, length * 8);
            Array.Reverse(memory);
            ulong[] numArray = new ulong[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToUInt64(memory, (length - 1 - i) * 8);
            }
            return numArray;
        }

        /// <summary>
        /// Read and return a Float.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <returns>The float of the offset.</returns>
        public float ReadFloat(uint offset)
        {
            byte[] memory = GetBytes(offset, 4);
            Array.Reverse(memory, 0, 4);
            return BitConverter.ToSingle(memory, 0);
        }

        /// <summary>
        /// Read and return an array of Floats.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="length">Length of memory to get.</param>
        /// <returns>The float array of the offset.</returns>
        public float[] ReadFloats(uint offset, int length)
        {
            byte[] memory = GetBytes(offset, length * 4);
            Array.Reverse(memory);
            float[] numArray = new float[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToSingle(memory, (length - 1 - i) * 4);
            }
            return numArray;
        }

        /// <summary>
        /// Read and return a double.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <returns>The double of the offset.</returns>
        public double ReadDouble(uint offset)
        {
            byte[] memory = GetBytes(offset, 8);
            Array.Reverse(memory, 0, 8);
            return BitConverter.ToDouble(memory, 0);
        }

        /// <summary>
        /// Read and return an array of doubles.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="length">Length of memory to get.</param>
        /// <returns>The double array of the offset.</returns>
        public double[] ReadDoubles(uint offset, int length)
        {
            byte[] memory = GetBytes(offset, length * 8);
            Array.Reverse(memory);
            double[] numArray = new double[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToSingle(memory, (length - 1 - i) * 8);
            }
            return numArray;
        }

        /// <summary>
        /// Read a string very fast by buffer and stop only when a byte null is detected (0x00).
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="encoding">Encoding of the text to be obtained.</param>
        /// <returns>The string of the offset.</returns>
        public string ReadString(uint offset, Encoding encoding = null)
        {
            if (encoding is null)
            {
                encoding = Encoding.Default;
            }
            byte[] numArray = GetBytes(offset, 255);
            string str = encoding.GetString(numArray);
            if (str.Contains('\0'))
            {
                str = str.Substring(0, str.IndexOf('\0'));
            }
            return str;
        }
        #endregion

        #region Write
        /// <summary>
        /// Write a boolean.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteBool(uint offset, bool input)
        {
            SetMemory(offset, new byte[] { input ? (byte)1 : (byte)0 });
        }

        /// <summary>
        /// Write a byte.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteByte(uint offset, byte input)
        {
            SetMemory(offset, new byte[] { input });
        }

        /// <summary>
        /// Write a byte array.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteBytes(uint offset, byte[] input)
        {
            SetMemory(offset, input);
        }

        /// <summary>
        /// Write a signed byte.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteSByte(uint offset, sbyte input)
        {
            SetMemory(offset, new byte[] { (byte)input });
        }

        /// <summary>
        /// Write a sbyte array.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteSBytes(uint offset, sbyte[] input)
        {
            //int length = input.Length;
            //byte[] bytes = new byte[length];
            //for (int i = 0; i < length; i++)
            //{
            //    bytes[i] = (byte)input[i];
            //}
            //SetMemory(offset, bytes);
            SetMemory(offset, (byte[])(Array)input);
        }

        /// <summary>
        /// Write an interger 16 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteInt16(uint offset, short input)
        {
            byte[] memory = new byte[2];
            BitConverter.GetBytes(input).CopyTo(memory, 0);
            Array.Reverse(memory, 0, 2);
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write an array of interger 16 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteInt16(uint offset, short[] input)
        {
            int length = input.Length;
            byte[] memory = new byte[length * 2];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(memory, i * 2);
                Array.Reverse(memory, i * 2, 2);
            }
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write an unsigned integer 16 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteUInt16(uint offset, ushort input)
        {
            byte[] memory = new byte[2];
            BitConverter.GetBytes(input).CopyTo(memory, 0);
            Array.Reverse(memory, 0, 2);
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write an array of unsigned integer 16 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteUInt16(uint offset, ushort[] input)
        {
            int length = input.Length;
            byte[] memory = new byte[length * 2];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(memory, i * 2);
                Array.Reverse(memory, i * 2, 2);
            }
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write an integer 32 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteInt32(uint offset, int input)
        {
            byte[] memory = new byte[4];
            BitConverter.GetBytes(input).CopyTo(memory, 0);
            Array.Reverse(memory, 0, 4);
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write an array of interger 32 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteInt32(uint offset, int[] input)
        {
            int length = input.Length;
            byte[] memory = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(memory, i * 4);
                Array.Reverse(memory, i * 4, 4);
            }
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write an unsigned integer 32 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteUInt32(uint offset, uint input)
        {
            byte[] memory = new byte[4];
            BitConverter.GetBytes(input).CopyTo(memory, 0);
            Array.Reverse(memory, 0, 4);
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write an array of unsigned integer 32 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteUInt32(uint offset, uint[] input)
        {
            int length = input.Length;
            byte[] memory = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(memory, i * 4);
                Array.Reverse(memory, i * 4, 4);
            }
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write an integer 64 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteInt64(uint offset, long input)
        {
            byte[] memory = new byte[8];
            BitConverter.GetBytes(input).CopyTo(memory, 0);
            Array.Reverse(memory, 0, 8);
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write an array of interger 64 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteInt64(uint offset, long[] input)
        {
            int length = input.Length;
            byte[] memory = new byte[length * 8];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(memory, i * 8);
                Array.Reverse(memory, i * 8, 8);
            }
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write an unsigned integer 64 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteUInt64(uint offset, ulong input)
        {
            byte[] memory = new byte[8];
            BitConverter.GetBytes(input).CopyTo(memory, 0);
            Array.Reverse(memory, 0, 8);
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write an array of unsigned integer 64 bits.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteUInt64(uint offset, ulong[] input)
        {
            int length = input.Length;
            byte[] memory = new byte[length * 8];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(memory, i * 8);
                Array.Reverse(memory, i * 8, 8);
            }
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write a Float.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteFloat(uint offset, float input)
        {
            byte[] memory = new byte[4];
            BitConverter.GetBytes(input).CopyTo(memory, 0);
            Array.Reverse(memory, 0, 4);
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write an array of Floats.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteFloats(uint offset, float[] input)
        {
            int length = input.Length;
            byte[] memory = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(memory, i * 4);
                Array.Reverse(memory, i * 4, 4);
            }
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write a double.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteDouble(uint offset, double input)
        {
            byte[] memory = new byte[8];
            BitConverter.GetBytes(input).CopyTo(memory, 0);
            Array.Reverse(memory, 0, 8);
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write an array of doubles.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        public void WriteDouble(uint offset, double[] input)
        {
            int length = input.Length;
            byte[] memory = new byte[length * 8];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(memory, i * 8);
                Array.Reverse(memory, i * 8, 8);
            }
            SetMemory(offset, memory);
        }

        /// <summary>
        /// Write a string.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="input">Value to write.</param>
        /// <param name="encoding">Encoding of the text to be written.</param>
        public void WriteString(uint offset, string input, Encoding encoding = null)
        {
            if (encoding is null)
            {
                encoding = Encoding.Default;
            }
            byte[] memory = encoding.GetBytes(input);
            Array.Resize(ref memory, memory.Length + 1);
            SetMemory(offset, memory);
        }
        #endregion

        /// <summary>
        /// Write a byte array.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="buffer">byte array to write.</param>
        public void SetMemory(uint offset, byte[] buffer)
        {
            api.SetMemory(offset, buffer);
        }

        /// <summary>
        /// Read a byte array.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="buffer">byte array to return the obtained value</param>
        public void GetMemory(uint offset, byte[] buffer)
        {
            api.GetMemory(offset, buffer);
        }

        /// <summary>
        /// Read and return a byte array.
        /// </summary>
        /// <param name="offset">The option address.</param>
        /// <param name="length">Length of memory to get.</param>
        /// <returns>The bytes of the offset.</returns>
        public byte[] GetBytes(uint offset, int length)
        {
            return api.GetBytes(offset, (uint)length);
        }

        /// <summary>
        /// Class for use the native fonctions
        /// </summary>
        public class Native
        {
            /// <summary>
            /// Instance to use Extension functions
            /// </summary>
            public Extension Extension { get; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="extension"></param>
            public Native(Extension extension)
            {
                Extension = extension;
            }

            private Native()
            {
            }

            #region Read
            /// <summary>
            /// Read and return the structure.
            /// </summary>
            /// <typeparam name="T">The structure type.</typeparam>
            /// <param name="address">The structure address.</param>
            /// <returns>The structure.</returns>
            public T Read<T>(IntPtr address) where T : struct
            {
                return BytesTo<T>(Extension.ReadBytes((uint)address, TypeCache<T>.Size));
            }

            /// <summary>
            /// Read and return the structure array.
            /// </summary>
            /// <typeparam name="T">The structures type.</typeparam>
            /// <param name="address">The address of the first structure.</param>
            /// <param name="length">The number of structures to search.</param>
            /// <returns>The structure array.</returns>
            public unsafe T[] ReadArray<T>(IntPtr address, int length) where T : new()
            {
                int size = TypeCache<T>.Size * length;
                byte[] data = Extension.ReadBytes((uint)address, size);
                T[] array = new T[length];
                fixed (byte* b = data)
                {
                    for (int i = 0; i < length; i++)
                    {
                        array[i] = Marshal.PtrToStructure<T>((IntPtr)(b + (i * TypeCache<T>.Size)));
                    }
                }
                return array;
            }

            /// <summary>
            /// Reads and returns the structure array of the pointer.
            /// </summary>
            /// <typeparam name="T">The structure type.</typeparam>
            /// <param name="baseAddress">The address where the structure pointer is located.</param>
            /// <param name="offsets"></param>
            /// <returns>The structure.</returns>
            public T ReadPointer<T>(IntPtr baseAddress, params int[] offsets) where T : struct
            {
                return Read<T>(DereferencePointer(baseAddress, offsets));
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="baseAddress"></param>
            /// <param name="offsets"></param>
            /// <returns></returns>
            public IntPtr DereferencePointer(IntPtr baseAddress, params int[] offsets)
            {
                for (int i = 0; i < offsets.Length - 1; i++)
                {
                    baseAddress = Read<IntPtr>(baseAddress + offsets[i]);
                }
                return baseAddress + offsets[offsets.Length - 1];
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="classPointer"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            public IntPtr GetVirtualFunction(IntPtr classPointer, int index)
            {
                IntPtr table = Read<IntPtr>(classPointer);
                return Read<IntPtr>(table + (index * TypeCache<IntPtr>.Size));
            }
            #endregion

            #region Write
            /// <summary>
            /// Write a structure.
            /// </summary>
            /// <typeparam name="T">The structure type.</typeparam>
            /// <param name="address">The structure address.</param>
            /// <param name="data">The data of the structure</param>
            public void Write<T>(IntPtr address, T data) where T : struct
            {
                Extension.WriteBytes((uint)address, ConvertToBytes(data));
            }

            /// <summary>
            /// Write a structure array.
            /// </summary>
            /// <typeparam name="T">The structures type.</typeparam>
            /// <param name="address">The address of the first structure.</param>
            /// <param name="array">The data of the structures</param>
            public unsafe void WriteArray<T>(IntPtr address, T[] array)
            {
                byte[] buffer = new byte[TypeCache<T>.Size * array.Length];
                fixed (byte* b = buffer)
                {
                    for (int i = 0; i < array.Length; i++)
                    {
                        Marshal.StructureToPtr(array[i], (IntPtr)(b + (i * TypeCache<T>.Size)), true);
                    }
                }
                Extension.WriteBytes((uint)address, buffer);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <typeparam name="T">The structures type</typeparam>
            /// <param name="baseAddress"></param>
            /// <param name="data"></param>
            /// <param name="offsets"></param>
            public void WritePointer<T>(IntPtr baseAddress, T data, params int[] offsets) where T : struct
            {
                Write(DereferencePointer(baseAddress, offsets), data);
            }
            #endregion

            #region Static methods
            /// <summary>
            /// Read and return the size of the structure.
            /// </summary>
            /// <typeparam name="T">The structure type.</typeparam>
            /// <returns>The size of the structure.</returns>
            public static int SizeOf<T>() where T : struct
            {
                return TypeCache<T>.Size;
            }

            /// <summary>
            /// Read and return the type of the structure.
            /// </summary>
            /// <typeparam name="T">The structure type.</typeparam>
            /// <returns>The type of the structure.</returns>
            public static Type TypeOf<T>() where T : struct
            {
                return TypeCache<T>.Type;
            }

            /// <summary>
            /// Convert structure data to bytes.
            /// </summary>
            /// <typeparam name="T">The structure type.</typeparam>
            /// <param name="obj"></param>
            /// <returns>The converted bytes of the structure.</returns>
            public static unsafe byte[] ConvertToBytes<T>(T obj) where T : struct
            {
                byte[] buffer = new byte[TypeCache<T>.Size];
                fixed (byte* b = buffer)
                {
                    Marshal.StructureToPtr(obj, (IntPtr)b, true);
                }
                return buffer;
            }

            /// <summary>
            /// Convert bytes to structure data.
            /// </summary>
            /// <typeparam name="T">The structure type.</typeparam>
            /// <param name="data">The bytes to convert.</param>
            /// <returns>The structure.</returns>
            public static unsafe T BytesTo<T>(byte[] data) where T : struct
            {
                fixed (byte* b = data)
                {
                    return Marshal.PtrToStructure<T>((IntPtr)b);
                }
            }

            /// <summary>
            /// Convert bytes to structure data.
            /// </summary>
            /// <typeparam name="T">The structure type.</typeparam>
            /// <param name="data">The bytes to convert.</param>
            /// <param name="index">The starting index of the conversion.</param>
            /// <returns>The structure.</returns>
            public static unsafe T BytesTo<T>(byte[] data, int index) where T : struct
            {
                int size = Marshal.SizeOf(typeof(T));
                byte[] tmp = new byte[size];
                Array.Copy(data, index, tmp, 0, size);
                return BytesTo<T>(tmp);
            }
            #endregion
        }
    }
}
