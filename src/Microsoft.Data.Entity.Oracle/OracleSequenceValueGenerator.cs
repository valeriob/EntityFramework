// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.Common;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Identity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Oracle.Utilities;
using Microsoft.Data.Entity.Utilities;

namespace Microsoft.Data.Entity.Oracle
{
    public class OracleSequenceValueGenerator : IValueGenerator
    {
        private readonly AsyncLock _lock = new AsyncLock();
        private readonly SqlStatementExecutor _executor;
        private readonly string _sequenceName;
        private readonly int _blockSize;
        private SequenceValue _currentValue = new SequenceValue(-1, 0);

        public OracleSequenceValueGenerator(
            [NotNull] SqlStatementExecutor executor,
            [NotNull] string sequenceName,
            int blockSize)
        {
            Check.NotNull(executor, "executor");
            Check.NotEmpty(sequenceName, "sequenceName");

            _executor = executor;
            _sequenceName = sequenceName;
            _blockSize = blockSize;
        }

        public virtual string SequenceName
        {
            get { return _sequenceName; }
        }

        public virtual int BlockSize
        {
            get { return _blockSize; }
        }

        public virtual object Next(
            DbContextConfiguration contextConfiguration,
            IProperty property)
        {
            Check.NotNull(contextConfiguration, "contextConfiguration");
            Check.NotNull(property, "property");

            var newValue = GetNextValue();

            // If the chosen value is outside of the current block then we need a new block.
            // It is possible that other threads will use all of the new block before this thread
            // gets a chance to use the new new value, so use a while here to do it all again.
            while (newValue.Current >= newValue.Max)
            {
                using (_lock.Lock())
                {
                    // Once inside the lock check to see if another thread already got a new block, in which
                    // case just get a value out of the new block instead of requesting one.
                    if (newValue.Max == _currentValue.Max)
                    {
                        var commandInfo = PrepareCommand(contextConfiguration);

                        var newCurrent = (long)_executor.ExecuteScalar(commandInfo.Item1, commandInfo.Item2);
                        newValue = new SequenceValue(newCurrent, newCurrent + _blockSize);
                        _currentValue = newValue;
                    }
                    else
                    {
                        newValue = GetNextValue();
                    }
                }
            }

            return Convert.ChangeType(newValue.Current, property.PropertyType);
        }

        public virtual async Task<object> NextAsync(
            DbContextConfiguration contextConfiguration,
            IProperty property,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Check.NotNull(contextConfiguration, "contextConfiguration");
            Check.NotNull(property, "property");

            var newValue = GetNextValue();

            // If the chosen value is outside of the current block then we need a new block.
            // It is possible that other threads will use all of the new block before this thread
            // gets a chance to use the new new value, so use a while here to do it all again.
            while (newValue.Current >= newValue.Max)
            {
                // Once inside the lock check to see if another thread already got a new block, in which
                // case just get a value out of the new block instead of requesting one.
                using (await _lock.LockAsync(cancellationToken))
                {
                    if (newValue.Max == _currentValue.Max)
                    {
                        var commandInfo = PrepareCommand(contextConfiguration);

                        var newCurrent = (long)await _executor.ExecuteScalarAsync(commandInfo.Item1, commandInfo.Item2, cancellationToken).ConfigureAwait(false);
                        newValue = new SequenceValue(newCurrent, newCurrent + _blockSize);
                        _currentValue = newValue;
                    }
                    else
                    {
                        newValue = GetNextValue();
                    }
                }
            }

            return Convert.ChangeType(newValue.Current, property.PropertyType);
        }

        private SequenceValue GetNextValue()
        {
            SequenceValue originalValue;
            SequenceValue newValue;
            do
            {
                originalValue = _currentValue;
                newValue = originalValue.NextValue();
            }
            while (Interlocked.CompareExchange(ref _currentValue, newValue, originalValue) != originalValue);

            return newValue;
        }

        private Tuple<DbConnection, SqlStatement> PrepareCommand(DbContextConfiguration contextConfiguration)
        {
            // TODO: Parameterize query and/or delimit identifier without using SqlServerMigrationOperationSqlGenerator
            var sql = new SqlStatement(string.Format(
                CultureInfo.InvariantCulture,
                "SELECT {0}.nextval from dual", _sequenceName));

            // TODO: Should be able to get relational connection without cast
            var connection = ((RelationalConnection)contextConfiguration.Connection).DbConnection;

            return Tuple.Create(connection, sql);
        }

        private class SequenceValue
        {
            private readonly long _current;
            private readonly long _max;

            public SequenceValue(long current, long max)
            {
                _current = current;
                _max = max;
            }

            public long Current
            {
                get { return _current; }
            }

            public long Max
            {
                get { return _max; }
            }

            public SequenceValue NextValue()
            {
                return new SequenceValue(_current + 1, _max);
            }
        }
    }
}
