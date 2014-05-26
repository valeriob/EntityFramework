// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Data.Entity.ChangeTracking;
using Microsoft.Data.Entity.Relational.Utilities;

namespace Microsoft.Data.Entity.Relational.Update
{
    //public class OracleCommandBatchPreparer : CommandBatchPreparer
    //{
    //    public OracleCommandBatchPreparer([NotNull] OracleParameterNameGeneratorFactory parameterNameGeneratorFactory) : base(parameterNameGeneratorFactory)
    //    {

    //    }

    //}

    public class OracleParameterNameGeneratorFactory : ParameterNameGeneratorFactory
    {
        public OracleParameterNameGeneratorFactory()
        {

        }
        public override ParameterNameGenerator Create()
        {
            return new OracleParameterNameGenerator();
        }
    }

    public class OracleParameterNameGenerator : ParameterNameGenerator
    {
        private int _count;

        public override string GenerateNext()
        {
            return ":p" + _count++;
        }
    }
}
