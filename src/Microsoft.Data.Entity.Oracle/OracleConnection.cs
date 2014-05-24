// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data.Common;
using System.Data.SqlClient;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using OracleManagedConnection = Oracle.ManagedDataAccess.Client.OracleConnection;

namespace Microsoft.Data.Entity.Oracle
{
    public class OracleConnection : RelationalConnection
    {
        public OracleConnection([NotNull] DbContextConfiguration configuration)
            : base(configuration)
        {
        }

        protected override DbConnection CreateDbConnection()
        {
            // TODO: Consider using DbProviderFactory to create connection instance
            return new OracleManagedConnection(ConnectionString);
        }

        public virtual OracleManagedConnection CreateMasterConnection()
        {
            var builder = new SqlConnectionStringBuilder { ConnectionString = ConnectionString };
            builder.InitialCatalog = "master";
            return new OracleManagedConnection(builder.ConnectionString);
        }
    }
}
