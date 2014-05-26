// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Identity;
using Microsoft.Data.Entity.Migrations;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Oracle;
using Microsoft.Data.Entity.Oracle.Utilities;
using Microsoft.Data.Entity.Storage;
using Microsoft.Data.Entity.Relational.Update;

// ReSharper disable once CheckNamespace
namespace Microsoft.Framework.DependencyInjection
{
    public static class SqlServerEntityServicesBuilderExtensions
    {
        public static EntityServicesBuilder AddOracle([NotNull] this EntityServicesBuilder builder)
        {
            Check.NotNull(builder, "builder");

            builder
                //.AddRelational()
                .ServiceCollection
                .AddSingleton<DatabaseBuilder>()
                .AddSingleton<RelationalObjectArrayValueReaderFactory>()
                .AddSingleton<RelationalTypedValueReaderFactory>()
                .AddSingleton(typeof(Microsoft.Data.Entity.Relational.Update.ParameterNameGeneratorFactory), typeof(Microsoft.Data.Entity.Relational.Update.OracleParameterNameGeneratorFactory))
                .AddSingleton<CommandBatchPreparer>()

                .AddSingleton<DataStoreSource, SqlServerDataStoreSource>()
                .AddSingleton<SqlServerValueGeneratorCache>()
                .AddSingleton<SqlServerValueGeneratorSelector>()
                .AddSingleton<SimpleValueGeneratorFactory<SequentialGuidValueGenerator>>()
                .AddSingleton<SqlServerSequenceValueGeneratorFactory>()
                .AddSingleton<SqlServerSqlGenerator>()
                .AddSingleton<SqlStatementExecutor>()
                .AddSingleton<SqlServerTypeMapper>()
                .AddScoped<SqlServerDataStore>()
                .AddScoped<OracleConnection>()
                .AddScoped<SqlServerBatchExecutor>()
                .AddScoped<ModelDiffer, ModelDiffer>()
                .AddScoped<SqlServerMigrationOperationSqlGenerator>()
                .AddScoped<SqlServerDataStoreCreator>();


            return builder;
        }
    }
}
