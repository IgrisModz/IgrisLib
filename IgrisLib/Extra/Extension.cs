using System;
using System.Linq;
using System.Text;

namespace IgrisLib
{
    public class Extension
    {
        private readonly IApi api;
        public Extension(IApi api)
        {
            this.api = api;
        }

        private Extension()
        {
        }

        public byte[] Ascii2Hex(string input)
        {
            return Encoding.Default.GetBytes(input);
        }

        public string ByteArrayToString(byte[] bytes)
        {
            ASCIIEncoding encoding = new ASCIIEncoding();
            return encoding.GetString(bytes);
        }

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
            return (num == a.Length);
        }

        public void Or_Int32(uint offset, int input)
        {
            int num = ReadInt32(offset) | input;
            WriteInt32(offset, num);
        }

        public string Hex2Ascii(byte[] buffer)
        {
            return Encoding.Default.GetString(buffer);
        }

        public byte[] ToHexFloat(float Axis, bool reverse = true)
        {
            byte[] bytes = BitConverter.GetBytes(Axis);
            if (reverse)
                Array.Reverse(bytes);
            return bytes;
        }

        public byte[] UintBytes(uint Input, bool reverse = true)
        {
            byte[] bytes = BitConverter.GetBytes(Input);
            if (reverse)
                Array.Reverse(bytes);
            return bytes;
        }

        /// <summary>Read a signed byte.</summary>
        public sbyte ReadSByte(uint offset)
        {
            byte[] buffer = GetBytes(offset, 1);
            return (sbyte)buffer[0];
        }

        /// <summary>Read and return an array of signed bytes.</summary>
        public sbyte[] ReadSBytes(uint offset, uint length)
        {
            byte[] buffer = GetBytes(offset, length);
            return (sbyte[])(Array)buffer;
        }

        /// <summary>Read a byte a check if his value. This return a bool according the byte detected.</summary>
        public bool ReadBool(uint offset)
        {
            byte[] buffer = new byte[1];
            GetMem(offset, buffer);
            return buffer[0] != 0;
        }

        /// <summary>Read and return an integer 16 bits.</summary>
        public short ReadInt16(uint offset, bool reverse = true)
        {
            byte[] buffer = GetBytes(offset, 2);
            if (reverse)
                Array.Reverse(buffer, 0, 2);
            return BitConverter.ToInt16(buffer, 0);
        }

        /// <summary>Read and return an array of integer 16 bits.</summary>
        public short[] ReadInt16(uint offset, int length, bool reverse = true)
        {
            byte[] memory = GetBytes(offset, (uint)length * 4);
            if (reverse)
                Array.Reverse(memory);
            short[] numArray = new short[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToInt16(memory, ((length - 1) - i) * 4);
            }
            return numArray;
        }

        /// <summary>Read and return an integer 32 bits.</summary>
        public int ReadInt32(uint offset, bool reverse = true)
        {
            byte[] buffer = GetBytes(offset, 4);
            if (reverse)
                Array.Reverse(buffer, 0, 4);
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>Read and return an array of integer 32 bits.</summary>
        public int[] ReadInt32(uint offset, int length, bool reverse = true)
        {
            byte[] memory = GetBytes(offset, (uint)length * 4);
            if (reverse)
                Array.Reverse(memory);
            int[] numArray = new int[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToInt32(memory, ((length - 1) - i) * 4);
            }
            return numArray;
        }

        /// <summary>Read and return an integer 64 bits.</summary>
        public long ReadInt64(uint offset, bool reverse = true)
        {
            byte[] buffer = GetBytes(offset, 8);
            if (reverse)
                Array.Reverse(buffer, 0, 8);
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>Read and return an array of integer 64 bits.</summary>
        public long[] ReadInt64(uint offset, int length, bool reverse = true)
        {
            byte[] memory = GetBytes(offset, (uint)length * 8);
            if (reverse)
                Array.Reverse(memory);
            long[] numArray = new long[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToInt64(memory, ((length - 1) - i) * 8);
            }
            return numArray;
        }

        /// <summary>Read and return a byte.</summary>
        public byte ReadByte(uint offset)
        {
            byte[] buffer = GetBytes(offset, 1);
            return buffer[0];
        }

        /// <summary>Read and return an array of bytes.</summary>
        public byte[] ReadBytes(uint offset, uint length)
        {
            byte[] buffer = GetBytes(offset, length);
            return buffer;
        }

        /// <summary>Read and return an unsigned integer 16 bits.</summary>
        public ushort ReadUInt16(uint offset, bool reverse = true)
        {
            byte[] buffer = GetBytes(offset, 2);
            if (reverse)
                Array.Reverse(buffer, 0, 2);
            return BitConverter.ToUInt16(buffer, 0);
        }

        /// <summary>Read and return an array of unsigned integer 16 bits.</summary>
        public ushort[] ReadUInt16(uint offset, int length, bool reverse = true)
        {
            byte[] memory = GetBytes(offset, (uint)length * 2);
            if (reverse)
                Array.Reverse(memory);
            ushort[] numArray = new ushort[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToUInt16(memory, ((length - 1) - i) * 2);
            }
            return numArray;
        }

        /// <summary>Read and return an unsigned integer 32 bits.</summary>
        public uint ReadUInt32(uint offset, bool reverse = true)
        {
            byte[] buffer = GetBytes(offset, 4);
            if (reverse)
                Array.Reverse(buffer, 0, 4);
            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>Read and return an array of unsigned integer 32 bits.</summary>
        public uint[] ReadUInt32(uint offset, int length, bool reverse = true)
        {
            byte[] memory = GetBytes(offset, (uint)length * 4);
            if (reverse)
                Array.Reverse(memory);
            uint[] numArray = new uint[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToUInt32(memory, ((length - 1) - i) * 4);
            }
            return numArray;
        }

        /// <summary>Read and return an unsigned integer 64 bits.</summary>
        public ulong ReadUInt64(uint offset, bool reverse = true)
        {
            byte[] buffer = GetBytes(offset, 8);
            if (reverse)
                Array.Reverse(buffer, 0, 8);
            return BitConverter.ToUInt64(buffer, 0);
        }

        /// <summary>Read and return an array of unsigned integer 64 bits.</summary>
        public ulong[] ReadUInt64(uint offset, int length, bool reverse = true)
        {
            byte[] memory = GetBytes(offset, (uint)length * 8);
            if (reverse)
                Array.Reverse(memory);
            ulong[] numArray = new ulong[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToUInt64(memory, ((length - 1) - i) * 8);
            }
            return numArray;
        }

        /// <summary>Read and return a Float.</summary>
        public float ReadFloat(uint offset, bool reverse = true)
        {
            byte[] buffer = GetBytes(offset, 4);
            if (reverse)
                Array.Reverse(buffer, 0, 4);
            return BitConverter.ToSingle(buffer, 0);
        }

        /// <summary>Read and return an array of Floats.</summary>
        public float[] ReadFloats(uint offset, int arrayLength = 3, bool reverse = true)
        {
            float[] vec = new float[arrayLength];
            for (int i = 0; i < arrayLength; i++)
            {
                byte[] buffer = GetBytes(offset + ((uint)i * 4), 4);
                if (reverse)
                    Array.Reverse(buffer, 0, 4);
                vec[i] = BitConverter.ToSingle(buffer, 0);
            }
            return vec;
        }

        /// <summary>Read and return a double.</summary>
        public double ReadDouble(uint offset, bool reverse = true)
        {
            byte[] buffer = GetBytes(offset, 8);
            if (reverse)
                Array.Reverse(buffer, 0, 8);
            return BitConverter.ToDouble(buffer, 0);
        }

        /// <summary>Read and return an array of doubles.</summary>
        public double[] ReadDoubles(uint offset, int length, bool reverse = true)
        {
            byte[] memory = GetBytes(offset, (uint)length * 8);
            if (reverse)
                Array.Reverse(memory);
            double[] numArray = new double[length];
            for (int i = 0; i < length; i++)
            {
                numArray[i] = BitConverter.ToSingle(memory, ((length - 1) - i) * 8);
            }
            return numArray;
        }

        /// <summary>Read a string very fast by buffer and stop only when a byte null is detected (0x00).</summary>
        public string ReadString(uint offset)
        {
            uint blocksize = 40;
            uint scalesize = 0;
            string str = string.Empty;

            while (!str.Contains('\0'))
            {
                byte[] buffer = ReadBytes(offset + scalesize, blocksize);
                str += Encoding.Default.GetString(buffer);
                scalesize += blocksize;
            }

            return str.Substring(0, str.IndexOf('\0'));
        }

        /// <summary>Write a signed byte.</summary>
        public void WriteSByte(uint offset, sbyte input)
        {
            byte[] buff = new byte[1];
            buff[0] = (byte)input;
            SetMem(offset, buff);
        }

        /*public void WriteSBytes(uint offset, sbyte[] input)
        {
            int length = input.Length;
            byte[] bytes = new byte[length];
            for (int i = 0; i < length; i++)
            {
                bytes[i] = (byte)input[i];
            }
            SetMem(offset, bytes);
        }*/

        /// <summary>Write a sbyte array.</summary>
        public void WriteSBytes(uint offset, sbyte[] input)
        {
            byte[] buff = (byte[])(Array)input;
            SetMem(offset, buff);
        }

        /// <summary>Write a boolean.</summary>
        public void WriteBool(uint offset, bool input)
        {
            byte[] buff = new byte[1];
            buff[0] = input ? (byte)1 : (byte)0;
            SetMem(offset, buff);
        }

        /// <summary>Write an interger 16 bits.</summary>
        public void WriteInt16(uint offset, short input, bool reverse = true)
        {
            byte[] buff = new byte[2];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 2);
            SetMem(offset, buff);
        }

        /// <summary>Write an array of interger 16 bits.</summary>
        public void WriteInt16(uint offset, short[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 2];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                SetMem(offset + ((uint)i * 2), buff);
            }
        }

        /// <summary>Write an integer 32 bits.</summary>
        public void WriteInt32(uint offset, int input, bool reverse = true)
        {
            byte[] buff = new byte[4];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 4);
            SetMem(offset, buff);
        }

        /// <summary>Write an array of interger 32 bits.</summary>
        public void WriteInt32(uint offset, int[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                SetMem(offset + ((uint)i * 4), buff);
            }
        }

        /// <summary>Write an integer 64 bits.</summary>
        public void WriteInt64(uint offset, long input, bool reverse = true)
        {
            byte[] buff = new byte[8];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 8);
            SetMem(offset, buff);
        }

        /// <summary>Write an array of interger 64 bits.</summary>
        public void WriteInt64(uint offset, long[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 8];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                SetMem(offset + ((uint)i * 8), buff);
            }
        }

        /// <summary>Write a byte.</summary>
        public void WriteByte(uint offset, byte input)
        {
            byte[] buff = new byte[1];
            buff[0] = input;
            SetMem(offset, buff);
        }

        /// <summary>Write a byte array.</summary>
        public void WriteBytes(uint offset, byte[] input)
        {
            byte[] buff = input;
            SetMem(offset, buff);
        }

        /// <summary>Write a string.</summary>
        public void WriteString(uint offset, string input)
        {
            byte[] buff = Encoding.Default.GetBytes(input);
            Array.Resize(ref buff, buff.Length + 1);
            SetMem(offset, buff);
        }

        /// <summary>Write an unsigned integer 16 bits.</summary>
        public void WriteUInt16(uint offset, ushort input, bool reverse = true)
        {
            byte[] buff = new byte[2];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 2);
            SetMem(offset, buff);
        }

        /// <summary>Write an array of unsigned integer 16 bits.</summary>
        public void WriteUInt16(uint offset, ushort[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 2];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                SetMem(offset + ((uint)i * 2), buff);
            }
        }

        /// <summary>Write an unsigned integer 32 bits.</summary>
        public void WriteUInt32(uint offset, uint input, bool reverse = true)
        {
            byte[] buff = new byte[4];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 4);
                SetMem(offset, buff);
        }

        /// <summary>Write an array of unsigned integer 32 bits.</summary>
        public void WriteUInt32(uint offset, uint[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 4];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                SetMem(offset + ((uint)i * 4), buff);
            }
        }

        /// <summary>Write an unsigned integer 64 bits.</summary>
        public void WriteUInt64(uint offset, ulong input, bool reverse = true)
        {
            byte[] buff = new byte[8];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 8);
            SetMem(offset, buff);
        }

        /// <summary>Write an array of unsigned integer 64 bits.</summary>
        public void WriteUInt64(uint offset, ulong[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 8];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                SetMem(offset + ((uint)i * 8), buff);
            }
        }

        /// <summary>Write a Float.</summary>
        public void WriteFloat(uint offset, float input, bool reverse = true)
        {
            byte[] buff = new byte[4];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 4);
            SetMem(offset, buff);
        }

        /// <summary>Write an array of Floats.</summary>
        public void WriteFloats(uint offset, float[] input, bool reverse = true)
        {
            byte[] buff = new byte[4];
            for (int i = 0; i < input.Length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff, 0, 4);
                SetMem(offset + ((uint)i * 4), buff);
            }
        }

        /// <summary>Write a double.</summary>
        public void WriteDouble(uint offset, double input, bool reverse = true)
        {
            byte[] buff = new byte[8];
            BitConverter.GetBytes(input).CopyTo(buff, 0);
            if (reverse)
                Array.Reverse(buff, 0, 8);
            SetMem(offset, buff);
        }

        /// <summary>Write an array of doubles.</summary>
        public void WriteDouble(uint offset, double[] input, bool reverse = true)
        {
            int length = input.Length;
            byte[] buff = new byte[length * 8];
            for (int i = 0; i < length; i++)
            {
                BitConverter.GetBytes(input[i]).CopyTo(buff, 0);
                if (reverse)
                    Array.Reverse(buff);
                SetMem(offset + ((uint)i * 8), buff);
            }
        }

        private void SetMem(uint offset, byte[] buffer)
        {
            api.SetMemory(offset, buffer);
        }

        private void GetMem(uint offset, byte[] buffer)
        {
            api.GetMemory(offset, buffer);
        }

        private byte[] GetBytes(uint offset, uint length)
        {
            byte[] buffer = api.GetBytes(offset, length);
            return buffer;
        }
    }
}
