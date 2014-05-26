// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Linq;
using Microsoft.Data.Entity.Identity;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Storage;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.DependencyInjection.Fallback;
using Xunit;

namespace Microsoft.Data.Entity.Oracle.Tests
{
    public class SqlServerEntityServicesBuilderExtensionsTest
    {
        [Fact]
        public void Can_get_default_services()
        {
            var services = new ServiceCollection();
            services.AddEntityFramework().AddOracle();

            // Relational
            Assert.True(services.Any(sd => sd.ServiceType == typeof(DatabaseBuilder)));
            Assert.True(services.Any(sd => sd.ServiceType == typeof(RelationalObjectArrayValueReaderFactory)));
            Assert.True(services.Any(sd => sd.ServiceType == typeof(RelationalTypedValueReaderFactory)));
            Assert.True(services.Any(sd => sd.ServiceType == typeof(CommandBatchPreparer)));

            // SQL Server dingletones
            Assert.True(services.Any(sd => sd.ServiceType == typeof(DataStoreSource)));
            Assert.True(services.Any(sd => sd.ServiceType == typeof(SqlServerSqlGenerator)));
            Assert.True(services.Any(sd => sd.ServiceType == typeof(SqlStatementExecutor)));

            // SQL Server scoped
            Assert.True(services.Any(sd => sd.ServiceType == typeof(OracleDataStore)));
            Assert.True(services.Any(sd => sd.ServiceType == typeof(OracleConnection)));
            Assert.True(services.Any(sd => sd.ServiceType == typeof(OracleBatchExecutor)));
            Assert.True(services.Any(sd => sd.ServiceType == typeof(ModelDiffer)));
            Assert.True(services.Any(sd => sd.ServiceType == typeof(OracleMigrationOperationSqlGenerator)));
            Assert.True(services.Any(sd => sd.ServiceType == typeof(OracleDataStoreCreator)));
        }

        [Fact]
        public void Services_wire_up_correctly()
        {
            var services = new ServiceCollection();
            services.AddEntityFramework().AddOracle();
            var serviceProvider = services.BuildServiceProvider();

            var context = new DbContext(
                serviceProvider,
                new DbContextOptions().UseOracle("goo").BuildConfiguration());

            var scopedProvider = context.Configuration.Services.ServiceProvider;

            var databaseBuilder = scopedProvider.GetService<DatabaseBuilder>();
            var arrayReaderFactory = scopedProvider.GetService<RelationalObjectArrayValueReaderFactory>();
            var typedReaderFactory = scopedProvider.GetService<RelationalTypedValueReaderFactory>();
            var batchPreparer = scopedProvider.GetService<CommandBatchPreparer>();

            var dataStoreSource = scopedProvider.GetService<DataStoreSource>();
            var sqlServerSqlGenerator = scopedProvider.GetService<SqlServerSqlGenerator>();
            var sqlStatementExecutor = scopedProvider.GetService<SqlStatementExecutor>();

            var sqlServerDataStore = scopedProvider.GetService<OracleDataStore>();
            var sqlServerConnection = scopedProvider.GetService<OracleConnection>();
            var sqlServerBatchExecutor = scopedProvider.GetService<OracleBatchExecutor>();
            var modelDiffer = scopedProvider.GetService<ModelDiffer>();
            var sqlServerMigrationOperationSqlGenerator = scopedProvider.GetService<OracleMigrationOperationSqlGenerator>();
            var sqlServerDataStoreCreator = scopedProvider.GetService<OracleDataStoreCreator>();

            Assert.NotNull(databaseBuilder);
            Assert.NotNull(arrayReaderFactory);
            Assert.NotNull(typedReaderFactory);
            Assert.NotNull(batchPreparer);

            Assert.NotNull(dataStoreSource);
            Assert.NotNull(sqlServerSqlGenerator);
            Assert.NotNull(sqlStatementExecutor);

            Assert.NotNull(sqlServerDataStore);
            Assert.NotNull(sqlServerConnection);
            Assert.NotNull(sqlServerBatchExecutor);
            Assert.NotNull(modelDiffer);
            Assert.NotNull(sqlServerMigrationOperationSqlGenerator);
            Assert.NotNull(sqlServerDataStoreCreator);

            context.Dispose();

            context = new DbContext(
                serviceProvider,
                new DbContextOptions().UseOracle("goo").BuildConfiguration());

            scopedProvider = context.Configuration.Services.ServiceProvider;

            // Dingletons
            Assert.Same(databaseBuilder, scopedProvider.GetService<DatabaseBuilder>());
            Assert.Same(arrayReaderFactory, scopedProvider.GetService<RelationalObjectArrayValueReaderFactory>());
            Assert.Same(typedReaderFactory, scopedProvider.GetService<RelationalTypedValueReaderFactory>());
            Assert.Same(batchPreparer, scopedProvider.GetService<CommandBatchPreparer>());

            Assert.Same(dataStoreSource, scopedProvider.GetService<DataStoreSource>());
            Assert.Same(sqlServerSqlGenerator, scopedProvider.GetService<SqlServerSqlGenerator>());
            Assert.Same(sqlStatementExecutor, scopedProvider.GetService<SqlStatementExecutor>());

            // Scoped
            Assert.NotSame(sqlServerDataStore, scopedProvider.GetService<OracleDataStore>());
            Assert.NotSame(sqlServerConnection, scopedProvider.GetService<OracleConnection>());
            Assert.NotSame(sqlServerBatchExecutor, scopedProvider.GetService<OracleBatchExecutor>());
            Assert.NotSame(modelDiffer, scopedProvider.GetService<ModelDiffer>());
            Assert.NotSame(sqlServerMigrationOperationSqlGenerator, scopedProvider.GetService<OracleMigrationOperationSqlGenerator>());
            Assert.NotSame(sqlServerDataStoreCreator, scopedProvider.GetService<OracleDataStoreCreator>());

            context.Dispose();
        }
    }
}
