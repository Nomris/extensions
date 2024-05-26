using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Org.Websn.Extensions
{
    public static class IDataReaderExtensions
    {
        #region No Return Values

        /// <summary>
        /// Reads all records of the <see cref="IDataReader"/>
        /// </summary>
        /// <param name="readFunction">The function that will be used to process all records</param>
        /// <param name="closeReader">Whether the <see cref="IDataReader"/> should be closed once all records where read</param>
        public static void ReadAll(this IDataReader reader, Action<IDataRecord> readFunction, bool closeReader = true)
        {
            while (reader.Read()) readFunction(reader);

            if (closeReader)
            {
                reader.Close();
                reader.Dispose();
            }
        }


        /// <summary>
        /// Reads all records of the <see cref="IDataReader"/>
        /// </summary>
        /// <param name="readFunction">The function that will be used to process all records</param>
        /// <param name="closeReader">Whether the <see cref="IDataReader"/> should be closed once all records where read</param>
        /// <param name="exc">The <see cref="Exception"/> that was thrown druing execution, <see langword="null"/> if no <see cref="Exception"/> was thrown</param>
        public static bool TryReadAll(this IDataReader reader, Action<IDataRecord> readFunction, out Exception exc, bool closeReader = true)
        {
            try
            {
                ReadAll(reader, readFunction, closeReader);

                exc = null;
                return true;
            }
            catch (Exception e)
            {
                exc = e;
                return false;
            }
        }

        #endregion

        #region Static Buffer

        /// <summary>
        /// Reads all records of the <see cref="IDataReader"/> and returns a <typeparamref name="T"/> for each entry
        /// </summary>
        /// <param name="readFunction">The function that will be used to process all records</param>
        /// <param name="closeReader">Whether the <see cref="IDataReader"/> should be closed once all records where read</param>
        public static T[] ReadAll<T>(this IDataReader reader, Func<IDataRecord, T> readFunction, bool closeReader = true)
        {
            return ReadAllEnumerable<T>(reader, readFunction, closeReader).ToArray();
        }

        /// <summary>
        /// Reads all records of the <see cref="IDataReader"/> and returns a <typeparamref name="T"/> for each entry in <paramref name="result"/>
        /// </summary>
        /// <param name="readFunction">The function that will be used to process all records</param>
        /// <param name="closeReader">Whether the <see cref="IDataReader"/> should be closed once all records where read</param>
        /// <param name="exc">The <see cref="Exception"/> that was thrown druing execution, <see langword="null"/> if no <see cref="Exception"/> was thrown</param>
        public static bool TryReadAll<T>(this IDataReader reader, Func<IDataRecord, T> readFunction, out Exception exc, out T[] result, bool closeReader = true)
        {
            bool success = TryReadAllEnumerable<T>(reader, readFunction, out exc, out IEnumerable<T> internalResult, closeReader = true);
            result = internalResult.ToArray();
            return success;
        }

        #endregion

        #region Enumrable


        /// <summary>
        /// Reads all records of the <see cref="IDataReader"/> and returns a <typeparamref name="T"/> for each entry
        /// </summary>
        /// <param name="readFunction">The function that will be used to process all records</param>
        /// <param name="closeReader">Whether the <see cref="IDataReader"/> should be closed once all records where read</param>
        public static IEnumerable<T> ReadAllEnumerable<T>(this IDataReader reader, Func<IDataRecord, T> readFunction, bool closeReader = true)
        {
            try
            {
                while (reader.Read())
                    yield return readFunction(reader);
            }
            finally
            {
                if (closeReader)
                {
                    reader.Close();
                    reader.Dispose();
                }
            }
        }

        /// <summary>
        /// Reads all records of the <see cref="IDataReader"/> and returns a <typeparamref name="T"/> for each entry in <paramref name="result"/>
        /// </summary>
        /// <param name="readFunction">The function that will be used to process all records</param>
        /// <param name="closeReader">Whether the <see cref="IDataReader"/> should be closed once all records where read</param>
        /// <param name="exc">The <see cref="Exception"/> that was thrown druing execution, <see langword="null"/> if no <see cref="Exception"/> was thrown</param>
        public static bool TryReadAllEnumerable<T>(this IDataReader reader, Func<IDataRecord, T> readFunction, out Exception exc, out IEnumerable<T> result, bool closeReader = true)
        {
            try
            {
                result = ReadAll(reader, readFunction, closeReader);
                exc = null;
                return true;
            }
            catch (Exception e)
            {
                result = null;
                exc = e;
                return false;
            }
        }

        #endregion
    }
}
