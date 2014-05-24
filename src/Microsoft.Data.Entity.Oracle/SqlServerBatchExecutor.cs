// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Update;

namespace Microsoft.Data.Entity.Oracle
{
    public class SqlServerBatchExecutor : BatchExecutor
    {
        public SqlServerBatchExecutor(
            [NotNull] SqlServerSqlGenerator sqlGenerator,
            [NotNull] OracleConnection connection,
            [NotNull] SqlServerTypeMapper parameterFactory)
            : base(sqlGenerator, connection, parameterFactory)
        {
        }
    }
}
