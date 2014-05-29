// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Oracle;
using Microsoft.Data.Entity.Relational.Update;

namespace Microsoft.Data.Entity.Oracle
{
    public class OracleDataStore : RelationalDataStore
    {
        public OracleDataStore(
            [NotNull] DbContextConfiguration configuration,
            [NotNull] OracleConnection connection,
            [NotNull] CommandBatchPreparer batchPreparer,
            [NotNull] OracleBatchExecutor batchExecutor)
            : base(configuration, connection, batchPreparer, batchExecutor)
        {
        }

        protected override RelationalValueReaderFactory ValueReaderFactory
        {
            get { return new OracleObjectArrayValueReaderFactory(); }
        }
    }
}
