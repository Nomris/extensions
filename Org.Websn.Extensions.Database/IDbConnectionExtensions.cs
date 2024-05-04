using System;
using System.Data;
using System.Linq;


namespace Org.Websn.Extensions
{
    public static class IDbConnectionExtensions
    {
        /// <summary>
        /// Test if the <paramref name="connection"/> is currently open, including being bussy.
        /// </summary>
        /// <returns>Returns <see langword="true"/> if the connection is curently open, <see langword="false"/> otherwise</returns>
        public static bool IsConnectionOpen(this IDbConnection connection)
        {
            if (connection == null) return false;
            if (connection.State == ConnectionState.Closed) return false;
            if (connection.State == ConnectionState.Broken) return false;

            return true;
        }

        /// <summary>
        /// Executes the provided <paramref name="commandOperator"/>.
        /// </summary>
        /// <param name="commandOperator">The operation that is to be made on the <paramref name="connection"/></param>
        /// <param name="openConnection">Wether the <paramref name="connection"/> should be opened. Set <see langword="false"/> if the <paramref name="connection"/> is already open. Set <see langword="null"/> to auto determin</param>
        public static void WrapCommand(this IDbConnection connection, Action<IDbCommand> commandOperator, bool? openConnection = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (!openConnection.HasValue) openConnection = !IsConnectionOpen(connection);

            if (openConnection.Value && connection.IsConnectionOpen())
                throw new InvalidOperationException("Cannot open connection that is already open");

            try
            {
                if (openConnection.Value) connection.Open();

                commandOperator(connection.CreateCommand());
            }
            finally
            {
                if (openConnection.Value)
                    try
                    {
                        connection.Close();
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("Faild to close the connetcion");
                        Console.Error.WriteLine(exc);
                    }
            }
        }

        /// <summary>
        /// Executes the provided <paramref name="commandOperator"/>. Returns <see langword="null"/> on success. And an <see cref="Exception"/> if any occurred
        /// </summary>
        /// <param name="commandOperator">The operation that is to be made on the <paramref name="connection"/></param>
        /// <param name="openConnection">Wether the <paramref name="connection"/> should be opened. Set <see langword="false"/> if the <paramref name="connection"/> is already open. Set <see langword="null"/> to auto determin</param>
        /// <param name="exc">The <see cref="Exception"/> if any was thrown, else it will be <see langword="null"/></param>
        /// <returns>Returns <see langword="true"/> if the command completed successfully, <see langword="false"/> otherwise</returns>
        public static bool TryWrapCommand(this IDbConnection connection, Action<IDbCommand> commandOperator, out Exception exc, bool? openConnection = null)
        {
            try
            {
                WrapCommand(connection, commandOperator, openConnection);

                exc = null;
                return true;
            }
            catch (Exception _exc)
            {
                exc = _exc;
                return false;
            }
        }

        /// <summary>
        /// Executes the provided <paramref name="commandOperator"/>.
        /// </summary>
        /// <param name="commandOperator">The operation that is to be made on the <paramref name="connection"/></param>
        /// <param name="openConnection">Wether the <paramref name="connection"/> should be opened. Set <see langword="false"/> if the <paramref name="connection"/> is already open. Set <see langword="null"/> to auto determin</param>
        public static TResult WrapCommand<TResult>(this IDbConnection connection, Func<IDbCommand, TResult> commandOperator, bool? openConnection = null)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (!openConnection.HasValue)
                openConnection = !IsConnectionOpen(connection);

            if (openConnection.Value && connection.IsConnectionOpen())
                throw new InvalidOperationException("Cannot open connection that is already open");

            try
            {
                if (openConnection.Value) connection.Open();

                return commandOperator(connection.CreateCommand());

            }
            finally
            {
                if (openConnection.Value)
                    try
                    {
                        connection.Close();
                    }
                    catch (Exception exc)
                    {
                        Console.WriteLine("Faild to close the connetcion");
                        Console.Error.WriteLine(exc);
                    }
            }
        }

        /// <summary>
        /// Executes the provided <paramref name="commandOperator"/>. Returns <see langword="null"/> on success. And an <see cref="Exception"/> if any occurred
        /// </summary>
        /// <param name="commandOperator">The operation that is to be made on the <paramref name="connection"/></param>
        /// <param name="openConnection">Wether the <paramref name="connection"/> should be opened. Set <see langword="false"/> if the <paramref name="connection"/> is already open. Set <see langword="null"/> to auto determin</param>
        /// <param name="exc">The <see cref="Exception"/> if any was thrown, else it will be <see langword="null"/></param>
        /// <param name="result">The <typeparamref name="TResult"/></param>
        /// <param name="default">The default value that should be returend if the operation faild</param>
        /// <returns>Returns <see langword="true"/> if the command completed successfully, <see langword="false"/> otherwise</returns>
        public static bool TryWrapCommand<TResult>(this IDbConnection connection, Func<IDbCommand, TResult> commandOperator, out TResult result, TResult @default, out Exception exc, bool? openConnection = null)
        {
            try
            {
                result = WrapCommand(connection, commandOperator, openConnection);

                exc = null;
                return true;
            }
            catch (Exception _exc)
            {
                exc = _exc;
                result = @default;
                return false;
            }
        }

        /// <summary>
        /// Executes the provided <paramref name="commandText"/> and returns a <see cref="IDataReader"/> as a result of the query
        /// </summary>
        /// <param name="commandText">The Command that is to be executed, use <see cref="string.Format"/> style inserstion of <paramref name="args"/></param>
        /// <param name="openConnection">Wether the <paramref name="connection"/> should be opened. Set <see langword="false"/> if the <paramref name="connection"/> is already open. Set <see langword="null"/> to auto determin</param>
        /// <param name="args">The Arguments</param>
        /// <returns>The <see cref="IDataReader"/> that represents the <paramref name="commandText"/></returns>
        public static IDataReader WrapCommandAsReader(this IDbConnection connection, string commandText, bool? openConnection, params object[] args)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));
            if (!openConnection.HasValue)
                openConnection = !IsConnectionOpen(connection);

            if (openConnection.Value && connection.IsConnectionOpen())
                throw new InvalidOperationException("Cannot open connection that is already open");

            try
            {
                if (openConnection.Value) connection.Open();

                IDbCommand command = connection.CreateCommand();
                int i = 0;
                command.CommandText = string.Format(commandText, args.Select(arg => $"@_{i++}").ToArray());
                i = -1;
                while (++i < args.Length)
                    command.AddGenericParameter($"_{i}", args[i]);

                return new DataReaderProxy(command.ExecuteReader(), () =>
                {
                    command.Dispose();
                    if (openConnection.Value)
                        try
                        {
                            connection.Close();
                        }
                        catch (Exception exc)
                        {
                            Console.WriteLine("Faild to close the connetcion");
                            Console.Error.WriteLine(exc);
                        }
                });

            }
            finally
            {
                
            }
        }

        /// <summary>
        /// Executes the provided <paramref name="commandText"/> and returns a <see cref="IDataReader"/> as a result of the query. And an <see cref="Exception"/> if any occurred
        /// </summary>
        /// <param name="commandText">The Command that is to be executed, use <see cref="string.Format"/> style inserstion of <paramref name="args"/></param>
        /// <param name="openConnection">Wether the <paramref name="connection"/> should be opened. Set <see langword="false"/> if the <paramref name="connection"/> is already open. Set <see langword="null"/> to auto determin</param>
        /// <param name="args">The Arguments</param>
        /// <param name="result">The <see cref="IDataReader"/> that represents the <paramref name="commandText"/></param>
        /// <param name="exc">The <see cref="Exception"/> if any was thrown, else it will be <see langword="null"/></param>
        /// <returns>Returns <see langword="true"/> if the command completed successfully, <see langword="false"/> otherwise</returns>
        public static bool TryWrapCommandAsReader(this IDbConnection connection, string commandText, out IDataReader result, out Exception exc, bool? openConnection, params object[] args)
        {
            try
            {
                result = WrapCommandAsReader(connection, commandText, openConnection, args);

                exc = null;
                return true;
            }
            catch (Exception _exc)
            {
                exc = _exc;
                result = null;
                return false;
            }
        }
    }
}
