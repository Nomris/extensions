using System;
using System.IO;
using System.Text;

namespace Org.Websn.Extensions
{
    public static class StreamExtensions
    {
        #region Reading

        /// <summary>
        /// Read's a byte array of <paramref name="length"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static byte[] ReadArray(this Stream stream, int length)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            byte[] buffer = new byte[length];
            int remainder = length;
            while (remainder > 0) remainder -= stream.Read(buffer, length - remainder, remainder);
            return buffer;
        }

        /// <summary>
        /// Read's an array of <paramref name="length"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static object[] ReadArray(this Stream stream, int length, Func<Stream, object> parser)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            object[] objs = new object[length];

            for (int i = 0; i < length; i++)
                objs[i] = parser(stream);
            return objs;
        }

        /// <summary>
        /// Read's a <see cref="sbyte"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static sbyte ReadSByte(this Stream stream)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            return (sbyte)stream.ReadByte();
        }

        /// <summary>
        /// Read's a <see cref="ushort"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static ushort ReadUInt16(this Stream stream, bool? reverseOrder = null)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = stream.ReadArray(sizeof(ushort));
            if (reverseOrder.Value) Array.Reverse(buffer);
            return BitConverter.ToUInt16(buffer, 0);
        }

        /// <summary>
        /// Read's a <see cref="short"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static short ReadInt16(this Stream stream, bool? reverseOrder = null)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = stream.ReadArray(sizeof(short));
            if (reverseOrder.Value) Array.Reverse(buffer);
            return BitConverter.ToInt16(buffer, 0);
        }

        /// <summary>
        /// Read's a <see cref="uint"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static uint ReadUInt32(this Stream stream, bool? reverseOrder = null)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = stream.ReadArray(sizeof(uint));
            if (reverseOrder.Value) Array.Reverse(buffer);
            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>
        /// Read's a <see cref="int"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static int ReadInt32(this Stream stream, bool? reverseOrder = null)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = stream.ReadArray(sizeof(int));
            if (reverseOrder.Value) Array.Reverse(buffer);
            return BitConverter.ToInt32(buffer, 0);
        }

        /// <summary>
        /// Read's a <see cref="ulong"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static ulong ReadUInt64(this Stream stream, bool? reverseOrder = null)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = stream.ReadArray(sizeof(ulong));
            if (reverseOrder.Value) Array.Reverse(buffer);
            return BitConverter.ToUInt64(buffer, 0);
        }

        /// <summary>
        /// Read's a <see cref="long"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static long ReadInt64(this Stream stream, bool? reverseOrder = null)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = stream.ReadArray(sizeof(long));
            if (reverseOrder.Value) Array.Reverse(buffer);
            return BitConverter.ToInt64(buffer, 0);
        }

        /// <summary>
        /// Read's a <see cref="float"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static float ReadSingle(this Stream stream, bool? reverseOrder = null)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = stream.ReadArray(sizeof(float));
            if (reverseOrder.Value) Array.Reverse(buffer);
            return BitConverter.ToSingle(buffer, 0);
        }

        /// <summary>
        /// Read's a <see cref="double"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static double ReadDouble(this Stream stream, bool? reverseOrder = null)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = stream.ReadArray(sizeof(double));
            if (reverseOrder.Value) Array.Reverse(buffer);
            return BitConverter.ToDouble(buffer, 0);
        }

        /// <summary>
        /// Read's a <see cref="bool"/>, with  <see langword="sizeof"/>(<see langword="bool"/>)
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static bool ReadBool(this Stream stream)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            byte[] buffer = stream.ReadArray(sizeof(bool));
            return BitConverter.ToBoolean(buffer, 0);
        }

        /// <summary>
        /// Read's a <see cref="string"/>
        /// </summary>
        /// <param name="encoding">
        ///     The <see cref="Encoding"/> that is to be used, must be <see cref="Encoding.IsSingleByte"/> = <see langword="true"/> if <paramref name="zeroTerminate"/> = <see langword="true"/>
        /// </param>
        /// <param name="zeroTerminate">Stop reading the stream once an null char is encountered, requires <see cref="Encoding.IsSingleByte"/> = <see langword="true"/></param>
        /// <param name="length">The length of the string in bytes, is ignored if <paramref name="zeroTerminate"/> = <see langword="true"/></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public static string ReadString(this Stream stream, Encoding encoding = null, bool zeroTerminate = true, int? length = null)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            encoding = encoding ?? Encoding.ASCII;

            if (zeroTerminate)
            {
                string buffer = "";
                if (!encoding.IsSingleByte) throw new ArgumentException("Zero termination only aviable in single byte encoding", nameof(encoding));
                byte c;
                if (length.HasValue)
                {
                    while ((c = (byte)stream.ReadByte()) != '\0' && --length > 0)
                        buffer += (char)c;

                    while (--length > 0) stream.ReadByte();
                }
                else while ((c = (byte)stream.ReadByte()) != '\0')
                        buffer += (char)c;

                return buffer;
            }

            if (!length.HasValue) throw new ArgumentNullException(nameof(length), "No length has been provided");
            if (length <= 0) return "";

            return encoding.GetString(stream.ReadArray(length.Value));
        }

        /// <summary>
        /// Read's a <see cref="Guid"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static Guid ReadGuid(this Stream stream)
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            return new Guid(stream.ReadArray(16));
        }

        /// <summary>
        /// Read's a <typeparamref name="T"/>
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public static T ReadEnum<T>(this Stream stream, bool? reverseOrder = null) where T : Enum
        {
            if (!stream.CanRead) throw new ArgumentException("The provided stream must be readable", nameof(stream));

            Type type = typeof(T);
            Type baseType = type.GetEnumUnderlyingType();
            if (baseType == typeof(byte))
                return (T)Enum.ToObject(type, (byte)stream.ReadByte());
            if (baseType == typeof(sbyte))
                return (T)Enum.ToObject(type, stream.ReadSByte());
            if (baseType == typeof(ushort))
                return (T)Enum.ToObject(type, stream.ReadUInt16(reverseOrder));
            if (baseType == typeof(short))
                return (T)Enum.ToObject(type, stream.ReadInt16(reverseOrder));
            if (baseType == typeof(uint))
                return (T)Enum.ToObject(type, stream.ReadUInt32(reverseOrder));
            if (baseType == typeof(int))
                return (T)Enum.ToObject(type, stream.ReadInt32(reverseOrder));
            if (baseType == typeof(ulong))
                return (T)Enum.ToObject(type, stream.ReadUInt64(reverseOrder));
            if (baseType == typeof(long))
                return (T)Enum.ToObject(type, stream.ReadInt64(reverseOrder));

            throw new InvalidCastException("Unknown Number Value");
        }

        #endregion

        #region Writer

        /// <summary>
        /// Write's the <paramref name="data"/> byte array
        /// </summary>
        /// <param name="offset">Where to start writing to the <see cref="Stream"/></param>
        /// <exception cref="ArgumentException"></exception>
        public static void WriteFixed(this Stream stream, byte[] data, long? size, int offset = 0)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            if (!size.HasValue)
            {
                stream.Write(data);
                return;
            }

            byte[] buffer = new byte[size.Value];
            Array.Copy(data, buffer, Math.Min(buffer.LongLength, data.LongLength));
            stream.Write(buffer, offset, buffer.Length);
        }

        /// <summary>
        /// Write's the <paramref name="buffer"/> byte array
        /// </summary>
        /// <param name="offset">Where to start writing to the <see cref="Stream"/></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Write(this Stream stream, byte[] buffer, int offset = 0)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            stream.Write(buffer, offset, buffer.Length);
        }

        /// <summary>
        /// Write's the <paramref name="data"/> byte
        /// </summary>
        /// <param name="offset">Where to start writing to the <see cref="Stream"/></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Write(this Stream stream, byte data)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            stream.Write(new byte[] { data });
        }

        /// <summary>
        /// Write's the <paramref name="data"/> sbyte
        /// </summary>
        /// <param name="offset">Where to start writing to the <see cref="Stream"/></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Write(this Stream stream, sbyte data)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            stream.Write(new byte[] { (byte)data });
        }

        /// <summary>
        /// Write's the <paramref name="value"/> byte
        /// </summary>
        /// <param name="offset">Where to start writing to the <see cref="Stream"/></param>
        /// <exception cref="ArgumentException"></exception>
        public static void Write(this Stream stream, ushort value, bool? reverseOrder = null)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = BitConverter.GetBytes(value);
            if (reverseOrder.Value) Array.Reverse(buffer);
            stream.Write(buffer);
        }

        public static void Write(this Stream stream, short value, bool? reverseOrder = null)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = BitConverter.GetBytes(value);
            if (reverseOrder.Value) Array.Reverse(buffer);
            stream.Write(buffer);
        }

        public static void Write(this Stream stream, uint value, bool? reverseOrder = null)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = BitConverter.GetBytes(value);
            if (reverseOrder.Value) Array.Reverse(buffer);
            stream.Write(buffer);
        }

        public static void Write(this Stream stream, int value, bool? reverseOrder = null)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = BitConverter.GetBytes(value);
            if (reverseOrder.Value) Array.Reverse(buffer);
            stream.Write(buffer);
        }

        public static void Write(this Stream stream, ulong value, bool? reverseOrder = null)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = BitConverter.GetBytes(value);
            if (reverseOrder.Value) Array.Reverse(buffer);
            stream.Write(buffer);
        }

        public static void Write(this Stream stream, long value, bool? reverseOrder = null)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = BitConverter.GetBytes(value);
            if (reverseOrder.Value) Array.Reverse(buffer);
            stream.Write(buffer);
        }

        public static void Write(this Stream stream, float value, bool? reverseOrder = null)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = BitConverter.GetBytes(value);
            if (reverseOrder.Value) Array.Reverse(buffer);
            stream.Write(buffer);
        }

        public static void Write(this Stream stream, double value, bool? reverseOrder = null)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            if (!reverseOrder.HasValue) reverseOrder = BitConverter.IsLittleEndian;
            byte[] buffer = BitConverter.GetBytes(value);
            if (reverseOrder.Value) Array.Reverse(buffer);
            stream.Write(buffer);
        }

        public static void Write(this Stream stream, bool value)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            stream.Write(BitConverter.GetBytes(value));
        }

        public static void Write(this Stream stream, string s, Encoding encoding = null, bool prependLength = false, int prependSize = sizeof(int))
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            encoding = encoding ?? Encoding.ASCII;
            byte[] rawStr = encoding.GetBytes(s);

            if (prependLength)
            {
                byte[] buffer = new byte[prependSize];
                Array.Copy(BitConverter.GetBytes(rawStr.Length), buffer, prependSize);
                if (BitConverter.IsLittleEndian) Array.Reverse(buffer);
                stream.Write(buffer);
            }

            stream.Write(rawStr);

            if (encoding.IsSingleByte && !prependLength) stream.WriteByte(0);
        }

        public static void Write(this Stream stream, Guid guid)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            stream.Write(guid.ToByteArray());
        }

        public static void Write(this Stream stream, IStreamable streamable)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            streamable.ToStream(stream);
        }

        public static void Write<T>(this Stream stream, T @enum, bool? reverseOrder = null) where T : Enum
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            Type baseType = typeof(T).GetEnumUnderlyingType();
            if (baseType == typeof(byte))
            {
                stream.Write((byte)(object)@enum);
                return;
            }
            if (baseType == typeof(sbyte))
            {
                stream.Write((sbyte)(object)@enum);
                return;
            }
            if (baseType == typeof(ushort))
            {
                stream.Write((ushort)(object)@enum, reverseOrder);
                return;
            }
            if (baseType == typeof(short))
            {
                stream.Write((short)(object)@enum, reverseOrder);
                return;
            }
            if (baseType == typeof(uint))
            {
                stream.Write((uint)(object)@enum, reverseOrder);
                return;
            }
            if (baseType == typeof(int))
            {
                stream.Write((int)(object)@enum, reverseOrder);
                return;
            }
            if (baseType == typeof(ulong))
            {
                stream.Write((ulong)(object)@enum, reverseOrder);
                return;
            }
            if (baseType == typeof(long))
            {
                stream.Write((long)(object)@enum, reverseOrder);
                return;
            }
            throw new InvalidCastException("Unknown Number Value");
        }

        public static StreamPackage OpenPackage(this Stream stream)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            return new StreamPackage(stream);
        }

        public static void ClosePackage(this Stream stream, StreamPackage package)
        {
            if (!stream.CanWrite) throw new ArgumentException("The provided stream must be writeable", nameof(stream));

            package.Submit(stream);
        }

        #endregion
    }
}
