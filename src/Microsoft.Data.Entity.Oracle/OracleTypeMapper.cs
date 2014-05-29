// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data;
using System.Linq;
using Microsoft.Data.Entity.Relational.Model;
using Microsoft.Data.Entity.Relational.Model.Oracle;

namespace Microsoft.Data.Entity.Oracle
{
    public class OracleTypeMapper : RelationalTypeMapper
    {
        // This dictionary is for invariant mappings from a sealed CLR type to a single
        // store type. If the CLR type is unsealed or if the mapping varies based on how the
        // type is used (e.g. in keys), then add custom mapping below.
        private readonly Tuple<Type, RelationalTypeMapping>[] _simpleMappings =
            {
                Tuple.Create(typeof(int), new RelationalTypeMapping("NUMBER(9)", DbType.Int32)),
                Tuple.Create(typeof(DateTime), new RelationalTypeMapping("TIMESTAMP(6)", DbType.DateTime)),
                Tuple.Create(typeof(Guid), (RelationalTypeMapping)new OracleRelationalTypeMapping("RAW(16)", DbType.Binary)),
                Tuple.Create(typeof(bool), new RelationalTypeMapping("NUMBER(1)", DbType.Int16)),
                Tuple.Create(typeof(byte), new RelationalTypeMapping("NUMBER(3)", DbType.Byte)),
                Tuple.Create(typeof(double), new RelationalTypeMapping("FLOAT", DbType.Double)),
             //   Tuple.Create(typeof(decimal), new RelationalTypeMapping("NUMBER", DbType.Decimal)),
                Tuple.Create(typeof(DateTimeOffset), new RelationalTypeMapping("TIMESTAMP(6) with time zone", DbType.DateTimeOffset)),
                Tuple.Create(typeof(char), new RelationalTypeMapping("CHAR", DbType.String)),
                Tuple.Create(typeof(sbyte), new RelationalTypeMapping("smallint", DbType.SByte)),
                Tuple.Create(typeof(ushort), new RelationalTypeMapping("int", DbType.UInt16)),
                Tuple.Create(typeof(uint), new RelationalTypeMapping("bigint", DbType.UInt32)),
                Tuple.Create(typeof(ulong), new RelationalTypeMapping("numeric(20, 0)", DbType.UInt64)),
            };

        private readonly RelationalTypeMapping _nonKeyStringMapping
            = new RelationalTypeMapping("nvarchar(max)", DbType.String);

        // TODO: It may be possible to increase 128 to 900, at least for SQL Server
        private readonly RelationalTypeMapping _keyStringMapping
            = new RelationalSizedTypeMapping("nvarchar2(128)", DbType.String, 128);

        private readonly RelationalTypeMapping _nonKeyByteArrayMapping
            = new RelationalTypeMapping("varbinary(max)", DbType.Binary);

        // TODO: It may be possible to increase 128 to 900, at least for SQL Server
        private readonly RelationalTypeMapping _keyByteArrayMapping
            = new RelationalSizedTypeMapping("varbinary(128)", DbType.Binary, 128);

        public override RelationalTypeMapping GetTypeMapping(
            string specifiedType, string storageName, Type propertyType, bool isKey, bool isConcurrencyToken)
        {
            var mapping = _simpleMappings.FirstOrDefault(m => m.Item1 == propertyType);
            if (mapping != null)
            {
                return mapping.Item2;
            }

            if (propertyType == typeof(string))
            {
                if (isKey)
                {
                    return _keyStringMapping;
                }
                return _nonKeyStringMapping;
            }

            if (propertyType == typeof(byte[]))
            {
                if (isKey)
                {
                    return _keyByteArrayMapping;
                }

                if (!isConcurrencyToken)
                {
                    return _nonKeyByteArrayMapping;
                }
            }

            return base.GetTypeMapping(specifiedType, storageName, propertyType, isKey, isConcurrencyToken);
        }
    }
}
