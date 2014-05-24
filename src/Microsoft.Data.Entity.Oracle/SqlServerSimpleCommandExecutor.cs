// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data.SqlClient;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Oracle.Utilities;

namespace Microsoft.Data.Entity.Oracle
{
    public class SqlServerSimpleCommandExecutor : IDbCommandExecutor
    {
        private const int DefaultCommandTimeout = 1;

        private readonly string _connectionString;
        private readonly int _commandTimeout;

        public SqlServerSimpleCommandExecutor([NotNull] string connectionString)
            : this(Check.NotEmpty(connectionString, "connectionString"), DefaultCommandTimeout)
        {
        }

        public SqlServerSimpleCommandExecutor([NotNull] string connectionString, int commandTimeout)
        {
            Check.NotEmpty(connectionString, "connectionString");

            _connectionString = connectionString;
            _commandTimeout = commandTimeout;
        }

        public virtual async Task<T> ExecuteScalarAsync<T>(string commandText, CancellationToken cancellationToken, params object[] parameters)
        {
            Check.NotEmpty(commandText, "commandText");
            Check.NotNull(parameters, "parameters");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync(cancellationToken).ConfigureAwait(false);

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = commandText;
                    command.CommandTimeout = _commandTimeout;

                    return (T)await command.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}
