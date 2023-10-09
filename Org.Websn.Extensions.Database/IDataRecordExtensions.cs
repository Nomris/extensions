using System;
using System.IO;
using System.Data;

namespace Org.Websn.Extensions
{
    public static class IDataRecordExtensions
    {
        #region Named Forwarding

        public static bool GetBoolean(this IDataRecord record, string name)
        {
            return record.GetBoolean(record.GetOrdinal(name));
        }

        public static byte GetByte(this IDataRecord record, string name)
        {
            return record.GetByte(record.GetOrdinal(name));
        }

        public static long GetBytes(this IDataRecord record, string name, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            return record.GetBytes(record.GetOrdinal(name), fieldOffset, buffer, bufferoffset, length);
        }

        public static char GetChar(this IDataRecord record, string name)
        {
            return record.GetChar(record.GetOrdinal(name));
        }

        public static long GetChars(this IDataRecord record, string name, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            return record.GetChars(record.GetOrdinal(name), fieldoffset, buffer, bufferoffset, length);
        }

        public static IDataReader GetData(this IDataRecord record, string name)
        {
            return record.GetData(record.GetOrdinal(name));
        }

        public static string GetDataTypeName(this IDataRecord record, string name)
        {
            return record.GetDataTypeName(record.GetOrdinal(name));
        }

        public static DateTime GetDateTime(this IDataRecord record, string name)
        {
            return record.GetDateTime(record.GetOrdinal(name));
        }

        public static decimal GetDecimal(this IDataRecord record, string name)
        {
            return record.GetDecimal(record.GetOrdinal(name));
        }

        public static double GetDouble(this IDataRecord record, string name)
        {
            return record.GetDouble(record.GetOrdinal(name));
        }

        public static Type GetFieldType(this IDataRecord record, string name)
        {
            return record.GetFieldType(record.GetOrdinal(name));
        }

        public static float GetFloat(this IDataRecord record, string name)
        {
            return record.GetFloat(record.GetOrdinal(name));
        }

        public static Guid GetGuid(this IDataRecord record, string name)
        {
            return record.GetGuid(record.GetOrdinal(name));
        }

        public static short GetInt16(this IDataRecord record, string name)
        {
            return record.GetInt16(record.GetOrdinal(name));
        }

        public static int GetInt32(this IDataRecord record, string name)
        {
            return record.GetInt32(record.GetOrdinal(name));
        }

        public static long GetInt64(this IDataRecord record, string name)
        {
            return record.GetInt64(record.GetOrdinal(name));
        }

        public static string GetString(this IDataRecord record, string name)
        {
            return record.GetString(record.GetOrdinal(name));
        }

        public static object GetValue(this IDataRecord record, string name)
        {
            return record.GetValue(record.GetOrdinal(name));
        }

        public static bool IsDBNull(this IDataRecord record, string name)
        {
            return record.IsDBNull(record.GetOrdinal(name));
        }

        #endregion

        #region Unsigned (Signed) Direct Support

        public static sbyte GetSByte(this IDataRecord record, int i)
        {
            unchecked
            {
                return (sbyte)record.GetByte(i);
            }
        }

        public static sbyte GetSByte(this IDataRecord record, string name)
        {
            return (sbyte)record.GetValue(record.GetOrdinal(name));
        }

        public static ushort GetUInt16(this IDataRecord record, int i)
        {
            unchecked
            {
                return (ushort)record.GetValue(i);
            }
        }

        public static ushort GetUInt16(this IDataRecord record, string name)
        {
            return (ushort)record.GetValue(record.GetOrdinal(name));
        }

        public static uint GetUInt32(this IDataRecord record, int i)
        {
            unchecked
            {
                return (uint)record.GetValue(i);
            }
        }

        public static uint GetUInt32(this IDataRecord record, string name)
        {
            return (uint)record.GetValue(record.GetOrdinal(name));
        }

        public static ulong GetUInt64(this IDataRecord record, int i)
        {
            unchecked
            {
                return (ulong)record.GetValue(i);
            }
        }

        public static ulong GetUInt64(this IDataRecord record, string name)
        {
            return (ulong)record.GetValue(record.GetOrdinal(name));
        }

        #endregion

        #region Misc

        /// <inheritdoc cref="GetStream(IDataRecord, int)"/>
        public static Stream GetStream(this IDataRecord record, string name)
            => record.GetStream(record.GetOrdinal(name));

        /// <summary>
        /// Gets a <see cref="Stream"/> from a byte array from <paramref name="record"/>
        /// </summary>
        public static Stream GetStream(this IDataRecord record, int i)
        {
            byte[] buffer = new byte[16 * 1024];
            long lastRead;
            long position = 0;

            MemoryStream memoryStream = new MemoryStream();
            do
            {
                lastRead = record.GetBytes(i, position, buffer, 0, buffer.Length);

                memoryStream.Write(buffer, 0, (int)lastRead);

                position += buffer.LongLength;
            } while (lastRead == buffer.LongLength);

            memoryStream.Position = 0;

            return memoryStream;
        }

        /// <summary>
        /// Gets the <see cref="Guid"/> by reading a byte array from the <paramref name="record"/>
        /// </summary>
        public static Guid GetGuidRaw(this IDataRecord record, int i)
        {
            byte[] guidBuffer = new byte[16];
            record.GetBytes(i, 0, guidBuffer, 0, 16);
            return new Guid(guidBuffer);
        }

        #endregion
    }
}
