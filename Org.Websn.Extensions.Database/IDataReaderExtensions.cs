using System;
using System.IO;
using System.Data;
using System.Linq;
using System.Collections.Generic;
using System.Text;

namespace Org.Websn.Extensions
{
    public static class IDataReaderExtensions
    {
        public static T[] ReadAll<T>(this IDataReader reader, Func<IDataRecord, T> readFunction, bool closeReader = true)
        {
            List<T> values = new List<T>();
            while (reader.Read())
                values.Add(readFunction(reader));

            if (closeReader)
            {
                reader.Close();
                reader.Dispose();
            }
            return values.ToArray();
        }

    }
}
