// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Relational.Update;

namespace Microsoft.Data.Entity.Oracle
{
    public class OracleBatchExecutor : BatchExecutor
    {
        public OracleBatchExecutor(
            [NotNull] SqlServerSqlGenerator sqlGenerator,
            [NotNull] OracleConnection connection,
            [NotNull] OracleTypeMapper parameterFactory)
            : base(sqlGenerator, connection, parameterFactory)
        {
        }
    }
}
