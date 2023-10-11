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
            if (value.GetType() == typeof(Guid))
            {

                parameter.Value = ((Guid)value).ToByteArray();
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
            using (MemoryStream memoryStream = new MemoryStream())
            {
                stream.CopyTo(memoryStream);

                command.AddGenericParameter(name, memoryStream.ToArray());
                memoryStream.Close();
            }
        }
    }
}
