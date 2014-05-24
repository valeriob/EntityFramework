// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data.SqlClient;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Xunit;
using OracleManagedConnection = Oracle.ManagedDataAccess.Client.OracleConnection;

namespace Microsoft.Data.Entity.Oracle.Tests
{
    public class OracleConnectionTest
    {
        [Fact]
        public void Creates_Oracle_connection_string()
        {
            using (var connection = new OracleConnection(CreateConfiguration()))
            {
                Assert.IsType<OracleManagedConnection>(connection.DbConnection);
            }
        }

        [Fact]
        public void Can_create_master_connection_string()
        {
            using (var connection = new OracleConnection(CreateConfiguration()))
            {
                using (var master = connection.CreateMasterConnection())
                {
                    Assert.Equal(@"USER ID=onwmstcc2;PASSWORD=onwmstcc2;DATA SOURCE=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS =(PROTOCOL=TCP)(HOST=vborioni-onit.cloudapp.net)(PORT=11521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=orcl)))", master.ConnectionString);
                }
            }
        }

        public static DbContextConfiguration CreateConfiguration()
        {
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddEntityFramework().AddOracle();
            return new DbContext(serviceCollection.BuildServiceProvider(),
                new DbContextOptions()
                    .UseOracle("USER ID=onwmstcc2;PASSWORD=onwmstcc2;DATA SOURCE=(DESCRIPTION=(ADDRESS_LIST=(ADDRESS =(PROTOCOL=TCP)(HOST=vborioni-onit.cloudapp.net)(PORT=11521)))(CONNECT_DATA=(SERVER=DEDICATED)(SID=orcl)))")
                    .BuildConfiguration()).Configuration;
        }
    }
}
