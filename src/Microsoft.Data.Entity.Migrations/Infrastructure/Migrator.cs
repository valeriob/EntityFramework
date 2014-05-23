// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Migrations.Utilities;
using Microsoft.Data.Entity.Relational;

namespace Microsoft.Data.Entity.Migrations.Infrastructure
{
    public class Migrator
    {
        private readonly DbContextConfiguration _contextConfiguration;
        private readonly HistoryRepository _historyRepository;
        private readonly MigrationAssembly _migrationAssembly;
        private readonly DatabaseBuilder _databaseBuilder;
        private readonly IMigrationOperationSqlGeneratorFactory _ddlSqlGeneratorFactory;
        private readonly SqlGenerator _dmlSqlGenerator;
        private readonly SqlStatementExecutor _sqlExecutor;        

        public Migrator(
            [NotNull] DbContextConfiguration contextConfiguration,
            [NotNull] HistoryRepository historyRepository,
            [NotNull] MigrationAssembly migrationAssembly,
            [NotNull] DatabaseBuilder databaseBuilder,
            [NotNull] IMigrationOperationSqlGeneratorFactory ddlSqlGeneratorFactory,
            [NotNull] SqlGenerator dmlSqlGenerator,
            [NotNull] SqlStatementExecutor sqlExecutor)
        {
            Check.NotNull(contextConfiguration, "contextConfiguration");
            Check.NotNull(historyRepository, "historyRepository");
            Check.NotNull(migrationAssembly, "migrationAssembly");
            Check.NotNull(databaseBuilder, "databaseBuilder");
            Check.NotNull(ddlSqlGeneratorFactory, "ddlSqlGeneratorFactory");
            Check.NotNull(dmlSqlGenerator, "dmlSqlGenerator");
            Check.NotNull(sqlExecutor, "sqlExecutor");

            _contextConfiguration = contextConfiguration;
            _historyRepository = historyRepository;
            _migrationAssembly = migrationAssembly;
            _databaseBuilder = databaseBuilder;
            _ddlSqlGeneratorFactory = ddlSqlGeneratorFactory;
            _dmlSqlGenerator = dmlSqlGenerator;
            _sqlExecutor = sqlExecutor;
        }

        protected virtual DbContextConfiguration ContextConfiguration
        {
            get { return _contextConfiguration; }
        }

        protected virtual HistoryRepository HistoryRepository
        {
            get { return _historyRepository; }
        }

        protected virtual MigrationAssembly MigrationAssembly
        {
            get { return _migrationAssembly; }
        }

        protected virtual DatabaseBuilder DatabaseBuilder
        {
            get { return _databaseBuilder; }
        }

        protected virtual IMigrationOperationSqlGeneratorFactory DdlSqlGeneratorFactory
        {
            get { return _ddlSqlGeneratorFactory; }
        }

        protected virtual SqlGenerator DmlSqlGenerator
        {
            get { return _dmlSqlGenerator; }
        }

        protected virtual SqlStatementExecutor SqlExecutor
        {
            get { return _sqlExecutor; }
        }

        public virtual IReadOnlyList<IMigrationMetadata> GetLocalMigrations()
        {
            return MigrationAssembly.Migrations;
        }

        public virtual IReadOnlyList<IMigrationMetadata> GetDatabaseMigrations()
        {
            return HistoryRepository.Migrations;
        }

        public virtual void UpdateDatabase()
        {
            UpdateDatabase(GenerateUpdateDatabaseSql());
        }

        public virtual void UpdateDatabase([NotNull] string targetMigrationName)
        {
            Check.NotEmpty(targetMigrationName, "targetMigrationName");

            UpdateDatabase(GenerateUpdateDatabaseSql(targetMigrationName));
        }

        protected virtual void UpdateDatabase(IReadOnlyList<SqlStatement> sqlStatements)
        {
            var dbConnection = ((RelationalConnection)ContextConfiguration.Connection).DbConnection;

            SqlExecutor.ExecuteNonQuery(dbConnection, sqlStatements);
        }

        public virtual IReadOnlyList<SqlStatement> GenerateUpdateDatabaseSql()
        {
            var localMigrations = GetLocalMigrations();
            var databaseMigrations = GetDatabaseMigrations();

            if (!ValidateMigrations(localMigrations, databaseMigrations))
            {
                throw new InvalidOperationException(Strings.InconsistentMigrations);
            }

            return 
                GenerateUpdateDatabaseSql(
                    localMigrations
                        .Skip(databaseMigrations.Count)
                        .ToArray(), 
                    downgrade: false);
        }

        public virtual IReadOnlyList<SqlStatement> GenerateUpdateDatabaseSql([NotNull] string targetMigrationName)
        {
            Check.NotEmpty(targetMigrationName, "targetMigrationName");

            var localMigrations = GetLocalMigrations();
            var databaseMigrations = GetDatabaseMigrations();

            if (!ValidateMigrations(localMigrations, databaseMigrations))
            {
                throw new InvalidOperationException(Strings.InconsistentMigrations);
            }

            var index = localMigrations.IndexOf(m => m.Name == targetMigrationName);
            if (index < 0)
            {
                throw new InvalidOperationException(Strings.FormatTargetMigrationNotFound(targetMigrationName));
            }

            if (index < databaseMigrations.Count)
            {
                return 
                    GenerateUpdateDatabaseSql(
                        localMigrations
                            .Skip(index + 1)
                            .Take(databaseMigrations.Count - index - 1)
                            .Reverse()
                            .ToArray(),
                        downgrade: true);
            }

            return 
                GenerateUpdateDatabaseSql(
                    localMigrations
                        .Skip(databaseMigrations.Count)
                        .Take(index - databaseMigrations.Count + 1)
                        .ToArray(), 
                    downgrade: false);
        }

        protected virtual IReadOnlyList<SqlStatement> GenerateUpdateDatabaseSql(
            IReadOnlyList<IMigrationMetadata> migrations, bool downgrade)
        {
            var sqlStatements = new List<SqlStatement>();

            foreach (var migration in migrations)
            {
                var database = DatabaseBuilder.GetDatabase(migration.TargetModel);
                var ddlSqlGenerator = DdlSqlGeneratorFactory.Create(database);

                sqlStatements.AddRange(
                    ddlSqlGenerator.Generate(
                        downgrade
                            ? migration.DowngradeOperations
                            : migration.UpgradeOperations,
                        generateIdempotentSql: true));

                sqlStatements.AddRange(
                    downgrade 
                        ? HistoryRepository.GenerateDeleteMigrationSql(migration, DmlSqlGenerator)
                        : HistoryRepository.GenerateInsertMigrationSql(migration, DmlSqlGenerator));
            }

            return sqlStatements;
        }

        protected virtual bool ValidateMigrations(
            IReadOnlyList<IMigrationMetadata> localMigrations,
            IReadOnlyList<IMigrationMetadata> databaseMigrations)
        {
            // TODO: Consider doing exhaustive validation.

            return
                databaseMigrations.Count == 0
                || localMigrations.Count >= databaseMigrations.Count
                && localMigrations[databaseMigrations.Count - 1].Name
                    == databaseMigrations[databaseMigrations.Count - 1].Name;
        }
    }
}
