using System;
using System.Linq;
using System.Text;

namespace IgrisLib
{
    /// <summary>
    /// Select the endian type
    /// </summary>
    public enum EndianType
    {
        /// <summary>
        /// 
        /// </summary>
        LittleEndian,
        /// <summary>
        /// 
        /// </summary>
        BigEndian
    }

    /// <summary>
    /// 
    /// </summary>
    public class ArrayBuilder
    {
        private readonly byte[] buffer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="BytesArray"></param>
        public ArrayBuilder(byte[] BytesArray)
        {
            buffer = BytesArray;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arraySize"></param>
        public ArrayBuilder(int arraySize)
        {
            buffer = new byte[arraySize];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public byte[] ToArray()
        {
            return buffer;
        }

        /// <summary>
        /// Enter into all functions "Reader".
        /// </summary>
        public ArrayReader Read => new ArrayReader(buffer);

        /// <summary>
        /// Enter into all functions "Writer".
        /// </summary>
        public ArrayWriter Write => new ArrayWriter(buffer);

        /// <summary>
        /// 
        /// </summary>
        public class ArrayReader
        {
            private readonly byte[] buffer;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="BytesArray"></param>
            public ArrayReader(byte[] BytesArray)
            {
                buffer = BytesArray;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <returns></returns>
            public sbyte GetSByte(int pos)
            {
                return (sbyte)buffer[pos];
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <returns></returns>
            public byte GetByte(int pos)
            {
                return buffer[pos];
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <returns></returns>
            public char GetChar(int pos)
            {
                string s = buffer[pos].ToString();
                char b = s[0];
                return b;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <returns></returns>
            public bool GetBool(int pos)
            {
                return buffer[pos] != 0;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="Type"></param>
            /// <returns></returns>
            public short GetInt16(int pos, EndianType Type = EndianType.BigEndian)
            {
                byte[] b = new byte[2];
                for (int i = 0; i < 2; i++)
                {
                    b[i] = buffer[pos + i];
                }

                if (Type == EndianType.BigEndian)
                {
                    Array.Reverse(b, 0, 2);
                }

                return BitConverter.ToInt16(b, 0);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="Type"></param>
            /// <returns></returns>
            public int GetInt32(int pos, EndianType Type = EndianType.BigEndian)
            {
                byte[] b = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    b[i] = buffer[pos + i];
                }

                if (Type == EndianType.BigEndian)
                {
                    Array.Reverse(b, 0, 4);
                }

                return BitConverter.ToInt32(b, 0);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="Type"></param>
            /// <returns></returns>
            public long GetInt64(int pos, EndianType Type = EndianType.BigEndian)
            {
                byte[] b = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    b[i] = buffer[pos + i];
                }

                if (Type == EndianType.BigEndian)
                {
                    Array.Reverse(b, 0, 8);
                }

                return BitConverter.ToInt64(b, 0);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="Type"></param>
            /// <returns></returns>
            public ushort GetUInt16(int pos, EndianType Type = EndianType.BigEndian)
            {
                byte[] b = new byte[2];
                for (int i = 0; i < 2; i++)
                {
                    b[i] = buffer[pos + i];
                }

                if (Type == EndianType.BigEndian)
                {
                    Array.Reverse(b, 0, 2);
                }

                return BitConverter.ToUInt16(b, 0);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="Type"></param>
            /// <returns></returns>
            public uint GetUInt32(int pos, EndianType Type = EndianType.BigEndian)
            {
                byte[] b = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    b[i] = buffer[pos + i];
                }

                if (Type == EndianType.BigEndian)
                {
                    Array.Reverse(b, 0, 4);
                }

                return BitConverter.ToUInt32(b, 0);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="Type"></param>
            /// <returns></returns>
            public ulong GetUInt64(int pos, EndianType Type = EndianType.BigEndian)
            {
                byte[] b = new byte[8];
                for (int i = 0; i < 8; i++)
                {
                    b[i] = buffer[pos + i];
                }

                if (Type == EndianType.BigEndian)
                {
                    Array.Reverse(b, 0, 8);
                }

                return BitConverter.ToUInt64(b, 0);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="length"></param>
            /// <returns></returns>
            public byte[] GetBytes(int pos, int length)
            {
                byte[] b = new byte[length];
                for (int i = 0; i < length; i++)
                {
                    b[i] = buffer[pos + i];
                }

                return b;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <returns></returns>
            public string GetString(int pos)
            {
                int strlen = 0;
                while (true)
                {
                    if ((pos + strlen) == buffer.Length)
                    {
                        break;
                    }

                    if (buffer[pos + strlen] != 0)
                    {
                        strlen++;
                    }
                    else
                    {
                        break;
                    }
                }
                return Encoding.UTF8.GetString(
                    buffer.ToList()
                    .GetRange(pos, strlen)
                    .ToArray());
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <returns></returns>
            public float GetFloat(int pos)
            {
                byte[] b = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    b[i] = buffer[pos + i];
                }

                Array.Reverse(b, 0, 4);
                return BitConverter.ToSingle(b, 0);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public class ArrayWriter
        {
            private readonly byte[] buffer;

            /// <summary>
            /// 
            /// </summary>
            /// <param name="BytesArray"></param>
            public ArrayWriter(byte[] BytesArray)
            {
                buffer = BytesArray;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="value"></param>
            public void SetSByte(int pos, sbyte value)
            {
                buffer[pos] = (byte)value;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="value"></param>
            public void SetByte(int pos, byte value)
            {
                buffer[pos] = value;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="value"></param>
            public void SetChar(int pos, char value)
            {
                byte[] b = Encoding.UTF8.GetBytes(value.ToString());
                buffer[pos] = b[0];
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="value"></param>
            public void SetBool(int pos, bool value)
            {
                byte[] b = new byte[1];
                b[0] = value ? (byte)1 : (byte)0;
                buffer[pos] = b[0];
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="value"></param>
            /// <param name="Type"></param>
            public void SetInt16(int pos, short value, EndianType Type = EndianType.BigEndian)
            {
                byte[] b = BitConverter.GetBytes(value);
                if (Type == EndianType.BigEndian)
                {
                    Array.Reverse(b, 0, 2);
                }

                for (int i = 0; i < 2; i++)
                {
                    buffer[i + pos] = b[i];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="value"></param>
            /// <param name="Type"></param>
            public void SetInt32(int pos, int value, EndianType Type = EndianType.BigEndian)
            {
                byte[] b = BitConverter.GetBytes(value);
                if (Type == EndianType.BigEndian)
                {
                    Array.Reverse(b, 0, 4);
                }

                for (int i = 0; i < 4; i++)
                {
                    buffer[i + pos] = b[i];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="value"></param>
            /// <param name="Type"></param>
            public void SetInt64(int pos, long value, EndianType Type = EndianType.BigEndian)
            {
                byte[] b = BitConverter.GetBytes(value);
                if (Type == EndianType.BigEndian)
                {
                    Array.Reverse(b, 0, 8);
                }

                for (int i = 0; i < 8; i++)
                {
                    buffer[i + pos] = b[i];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="value"></param>
            /// <param name="Type"></param>
            public void SetUInt16(int pos, ushort value, EndianType Type = EndianType.BigEndian)
            {
                byte[] b = BitConverter.GetBytes(value);
                if (Type == EndianType.BigEndian)
                {
                    Array.Reverse(b, 0, 2);
                }

                for (int i = 0; i < 2; i++)
                {
                    buffer[i + pos] = b[i];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="value"></param>
            /// <param name="Type"></param>
            public void SetUInt32(int pos, uint value, EndianType Type = EndianType.BigEndian)
            {
                byte[] b = BitConverter.GetBytes(value);
                if (Type == EndianType.BigEndian)
                {
                    Array.Reverse(b, 0, 4);
                }

                for (int i = 0; i < 4; i++)
                {
                    buffer[i + pos] = b[i];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="value"></param>
            /// <param name="Type"></param>
            public void SetUInt64(int pos, ulong value, EndianType Type = EndianType.BigEndian)
            {
                byte[] b = BitConverter.GetBytes(value);
                if (Type == EndianType.BigEndian)
                {
                    Array.Reverse(b, 0, 8);
                }

                for (int i = 0; i < 8; i++)
                {
                    buffer[i + pos] = b[i];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="value"></param>
            public void SetBytes(int pos, byte[] value)
            {
                int valueSize = value.Length;
                for (int i = 0; i < valueSize; i++)
                {
                    buffer[i + pos] = value[i];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="value"></param>
            public void SetString(int pos, string value)
            {
                byte[] b = Encoding.UTF8.GetBytes($"{value}\0");
                for (int i = 0; i < b.Length; i++)
                {
                    buffer[i + pos] = b[i];
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="pos"></param>
            /// <param name="value"></param>
            public void SetFloat(int pos, float value)
            {
                byte[] b = BitConverter.GetBytes(value);
                Array.Reverse(b, 0, 4);
                for (int i = 0; i < 4; i++)
                {
                    buffer[i + pos] = b[i];
                }
            }
        }
    }
}
