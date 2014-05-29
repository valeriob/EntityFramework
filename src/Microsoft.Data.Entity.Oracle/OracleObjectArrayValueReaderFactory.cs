// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Data.Common;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Utilities;

namespace Microsoft.Data.Entity.Relational.Oracle
{
    public class OracleObjectArrayValueReaderFactory : RelationalObjectArrayValueReaderFactory
    {
        public override IValueReader Create(DbDataReader dataReader)
        {
            //Check.NotNull(dataReader, "dataReader");

            return new OracleObjectArrayValueReader(dataReader);
        }
    }
}
