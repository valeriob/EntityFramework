// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using JetBrains.Annotations;
using Microsoft.Data.Entity.Identity;

namespace Microsoft.Data.Entity.Oracle
{
    public class OracleValueGeneratorCache : ValueGeneratorCache
    {
        public OracleValueGeneratorCache([NotNull] OracleValueGeneratorSelector selector)
            : base(selector)
        {
        }
    }
}
