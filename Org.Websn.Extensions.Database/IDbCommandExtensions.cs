using System;
using System.IO;
using System.Data;

namespace Org.Websn.Extensions
{
    public static class IDbCommandExtensions
    {
        /// <summary>
        /// Addes or sets the paremter <paramref name="name"/>
        /// </summary>
        /// <param name="command">The <see cref="IDbCommand"/> that the parameter is to be set on</param>
        /// <param name="name">The name of the parameter</param>
        /// <param name="value">The value that the parameter should have</param>
        public static void AddGenericParameter(this IDbCommand command, string name, object value)
        {
            if (value == null) value = DBNull.Value;

            IDataParameter parameter = command.Parameters.Contains(name) ? (IDataParameter)command.Parameters[name] : command.CreateParameter();
            parameter.ParameterName = name;

            Type type = value.GetType();

            if (type == typeof(Guid))
            {
                parameter.Value = ((Guid)value).ToByteArray();
            }
            if (typeof(Stream).IsAssignableFrom(type))
            {
                Stream stream = (Stream)value;
                long pos = stream.Position; // Get current position
                stream.Position = 0;
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);

                    parameter.Value = memoryStream.ToArray();
                    memoryStream.Close();
                }
                stream.Position = pos; // Reset post to original postion
            }
            else if (type.IsEnum)
            {
                type = type.GetEnumUnderlyingType();

                if (type == typeof(byte)) parameter.Value = (byte)value;
                if (type == typeof(sbyte)) parameter.Value = (sbyte)value;
                if (type == typeof(ushort)) parameter.Value = (ushort)value;
                if (type == typeof(short)) parameter.Value = (short)value;
                if (type == typeof(uint)) parameter.Value = (uint)value;
                if (type == typeof(int)) parameter.Value = (int)value;
                if (type == typeof(ulong)) parameter.Value = (ulong)value;
                if (type == typeof(long)) parameter.Value = (long)value;
            }
            else
                parameter.Value = value;

            if (command.Parameters.Contains(name)) return;
            command.Parameters.Add(parameter);
        }

        /// <summary>
        /// Addes or sets a <see cref="Stream"/> as a parameter
        /// </summary>
        /// <param name="command">The <see cref="IDbCommand"/> that the parameter is to be set on</param>
        /// <param name="name">The name of the parameter</param>
        /// <param name="stream">The value that the parameter should have</param>
        public static void AddStream(this IDbCommand command, string name, Stream stream)
        {
            long pos = stream.Position; // Get current position
            stream.Position = 0;
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                command.AddGenericParameter(name, memoryStream.ToArray());
                memoryStream.Close();
            }
            stream.Position = pos; // Reset post to original postion
        }
    }
}
