using System;
using System.Data;
using System.Collections.Generic;

/* Unmerged change from project 'Org.Websn.Extensions.Database (net7.0)'
Before:
using System.Text;
After:
using System.Text;
using Extensions;
using Org.Websn.Extensions;
*/
using System.Text;

namespace Org.Websn.Extensions
{
    public static class IDbConnectionExtensions
    {
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
        /// <param name="connection">The connection on which the <paramref name="commandOperator"/> should be executed</param>
        /// <param name="commandOperator">The operation that is to be made on the <paramref name="connection"/></param>
        /// <param name="openConnection">Wether the <paramref name="connection"/> should be opened. (Set false if the <paramref name="connection"/> is already open)</param>
        public static void WrapCommand(this IDbConnection connection, Action<IDbCommand> commandOperator, bool openConnection = true)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            if (openConnection && connection.IsConnectionOpen())
                throw new InvalidOperationException("Cannot open connection that is already open");

            try
            {
                if (openConnection) connection.Open();

                commandOperator(connection.CreateCommand());
            }
            finally
            {
                if (openConnection)
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
        /// <param name="connection">The connection on which the <paramref name="commandOperator"/> should be executed</param>
        /// <param name="commandOperator">The operation that is to be made on the <paramref name="connection"/></param>
        /// <param name="openConnetcion">Wether the <paramref name="connection"/> should be opened. (Set false if the <paramref name="connection"/> is already open)</param>
        /// <param name="exc">The <see cref="Exception"/> if any was thrown, else it will be <see langword="null"/></param>
        /// <returns></returns>
        public static bool TryWrapCommand(this IDbConnection connection, Action<IDbCommand> commandOperator, out Exception exc, bool openConnetcion = true)
        {
            try
            {
                connection.WrapCommand(commandOperator, openConnetcion);

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
        /// <param name="connection">The connection on which the <paramref name="commandOperator"/> should be executed</param>
        /// <param name="commandOperator">The operation that is to be made on the <paramref name="connection"/></param>
        /// <param name="openConnection">Wether the <paramref name="connection"/> should be opened. (Set false if the <paramref name="connection"/> is already open)</param>
        public static TResult WrapCommand<TResult>(this IDbConnection connection, Func<IDbCommand, TResult> commandOperator, bool openConnection = true)
        {
            if (connection == null) throw new ArgumentNullException(nameof(connection));

            if (openConnection && connection.IsConnectionOpen())
                throw new InvalidOperationException("Cannot open connection that is already open");

            try
            {
                if (openConnection) connection.Open();

                return commandOperator(connection.CreateCommand());

            }
            finally
            {
                if (openConnection)
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
        /// <param name="connection">The connection on which the <paramref name="commandOperator"/> should be executed</param>
        /// <param name="commandOperator">The operation that is to be made on the <paramref name="connection"/></param>
        /// <param name="openConnetcion">Wether the <paramref name="connection"/> should be opened. (Set false if the <paramref name="connection"/> is already open)</param>
        /// <param name="exc">The <see cref="Exception"/> if any was thrown, else it will be <see langword="null"/></param>
        /// <param name="result">The <typeparamref name="TResult"/></param>
        /// <param name="default">The default value that should be returend if the operation faild</param>
        /// <returns></returns>
        public static bool TryWrapCommand<TResult>(this IDbConnection connection, Func<IDbCommand, TResult> commandOperator, out TResult result, TResult @default, out Exception exc, bool openConnetcion = true)
        {
            try
            {
                result = connection.WrapCommand(commandOperator, openConnetcion);

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
    }
}
