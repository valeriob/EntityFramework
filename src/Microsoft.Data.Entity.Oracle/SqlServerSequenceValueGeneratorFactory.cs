// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Identity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations.Model;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Model;
using Microsoft.Data.Entity.Oracle.Utilities;

namespace Microsoft.Data.Entity.Oracle
{
    public class SqlServerSequenceValueGeneratorFactory : IValueGeneratorFactory
    {
        public const int DefaultBlockSize = 10;

        private readonly SqlStatementExecutor _executor;

        public SqlServerSequenceValueGeneratorFactory([NotNull] SqlStatementExecutor executor)
        {
            Check.NotNull(executor, "executor");

            _executor = executor;
        }

        public virtual int GetBlockSize([NotNull] IProperty property)
        {
            Check.NotNull(property, "property");

            // TODO: StoreSequenceBlockSize string should be in a constant somewhere--not sure where yet
            var annotatedIncrement = TryFindAnnotation(property, "StoreSequenceBlockSize");

            // TODO: Allow integer annotations
            return annotatedIncrement != null ? int.Parse(annotatedIncrement) : DefaultBlockSize;
        }

        public virtual string GetSequenceName([NotNull] IProperty property)
        {
            Check.NotNull(property, "property");

            // TODO: StoreSequenceName string should be in a constant somewhere--not sure where yet
            return TryFindAnnotation(property, "StoreSequenceName") ?? (property.EntityType.StorageName + "_Sequence");
        }

        private static string DelimitSequenceName(string sequenceName)
        {
            return new SqlServerMigrationOperationSqlGenerator(new SqlServerTypeMapper()).DelimitIdentifier(sequenceName);
        }

        public virtual IReadOnlyList<MigrationOperation> GetUpMigrationOperations([NotNull] IProperty property)
        {
            Check.NotNull(property, "property");

            return new[]
                {
                    new CreateSequenceOperation(
                        new Sequence(DelimitSequenceName(GetSequenceName(property)))
                            {
                                StartWith = 0,
                                IncrementBy = GetBlockSize(property),
                                DataType = "BIGINT"
                            })
                };
        }

        public virtual IReadOnlyList<MigrationOperation> GetDownMigrationOperations([NotNull] IProperty property)
        {
            Check.NotNull(property, "property");

            return new[]
                {
                    new DropSequenceOperation(DelimitSequenceName(GetSequenceName(property)))
                };
        }

        private static string TryFindAnnotation(IProperty property, string name)
        {
            return property[name] ?? property.EntityType[name] ?? property.EntityType.Model[name];
        }

        public virtual IValueGenerator Create(IProperty property)
        {
            Check.NotNull(property, "property");

            return new SqlServerSequenceValueGenerator(_executor, GetSequenceName(property), GetBlockSize(property));
        }

        public virtual int GetPoolSize(IProperty property)
        {
            Check.NotNull(property, "property");

            // TODO: Allow configuration without creation of derived factory type
            return 5;
        }

        public virtual string GetCacheKey(IProperty property)
        {
            Check.NotNull(property, "property");

            return GetSequenceName(property);
        }
    }
}
