// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Identity;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Oracle.Utilities;

namespace Microsoft.Data.Entity.Oracle
{
    public class SqlServerValueGeneratorSelector : ValueGeneratorSelector
    {
        private readonly SimpleValueGeneratorFactory<TemporaryValueGenerator> _tempFactory;
        private readonly SqlServerSequenceValueGeneratorFactory _sequenceFactory;
        private readonly SimpleValueGeneratorFactory<SequentialGuidValueGenerator> _sequentialGuidFactory;

        public SqlServerValueGeneratorSelector(
            [NotNull] SimpleValueGeneratorFactory<GuidValueGenerator> guidFactory,
            [NotNull] SimpleValueGeneratorFactory<TemporaryValueGenerator> tempFactory,
            [NotNull] SqlServerSequenceValueGeneratorFactory sequenceFactory,
            [NotNull] SimpleValueGeneratorFactory<SequentialGuidValueGenerator> sequentialGuidFactory)
            : base(guidFactory)
        {
            Check.NotNull(sequenceFactory, "sequenceFactory");
            Check.NotNull(sequentialGuidFactory, "sequentialGuidFactory");

            _tempFactory = tempFactory;
            _sequenceFactory = sequenceFactory;
            _sequentialGuidFactory = sequentialGuidFactory;
        }

        public override IValueGeneratorFactory Select(IProperty property)
        {
            Check.NotNull(property, "property");

            switch (property.ValueGenerationOnAdd)
            {
                case ValueGenerationOnAdd.Client:
                    if (property.PropertyType.IsInteger() && property.PropertyType != typeof(byte))
                    {
                        return _tempFactory;
                    }
                    if (property.PropertyType == typeof(Guid))
                    {
                        return _sequentialGuidFactory;
                    }
                    goto default;

                case ValueGenerationOnAdd.Server:
                    if (property.PropertyType.IsInteger())
                    {
                        return _sequenceFactory;
                    }
                    goto default;

                default:
                    return base.Select(property);
            }
        }
    }
}
