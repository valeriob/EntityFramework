// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Oracle.Utilities;
using Microsoft.Data.Entity.Storage;

namespace Microsoft.Data.Entity.Oracle
{
    public class OracleDataStoreSource
        : DataStoreSource<
            OracleDataStore,
            SqlServerConfigurationExtension,
            OracleDataStoreCreator,
            OracleConnection,
            OracleValueGeneratorCache>
    {
        public override bool IsAvailable(DbContextConfiguration configuration)
        {
            Check.NotNull(configuration, "configuration");

            // TODO: Consider finding connection string in config file by convention
            return IsConfigured(configuration);
        }

        public override string Name
        {
            get { return typeof(OracleDataStore).Name; }
        }
    }
}
