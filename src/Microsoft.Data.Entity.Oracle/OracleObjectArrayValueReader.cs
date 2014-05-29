// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Data.Common;
using JetBrains.Annotations;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Relational.Utilities;

namespace Microsoft.Data.Entity.Relational.Oracle
{
    public class OracleObjectArrayValueReader : IValueReader
    {
        object[] _valueBuffer;

        public OracleObjectArrayValueReader([NotNull] DbDataReader dataReader)
        {
            _valueBuffer = CreateBuffer(dataReader);
        }

        private static object[] CreateBuffer(DbDataReader dataReader)
        {
            var values = new object[dataReader.FieldCount];

            dataReader.GetValues(values);

            for (var i = 0; i < values.Length; i++)
            {
                if (ReferenceEquals(values[i], DBNull.Value))
                {
                    values[i] = null;
                }
            }

            return values;
        }

        public virtual bool IsNull(int index)
        {
            return _valueBuffer[index] == null;
        }

        public virtual T ReadValue<T>(int index)
        {
            if (typeof(T) == typeof(bool))
                return (T)GetBoolean(index);

            if (typeof(T) == typeof(Guid))
                return (T)GetGuid(index);
            if (typeof(T) == typeof(byte))
                return (T)GetByte(index);
            if (typeof(T) == typeof(char))
                return (T)GetChar(index);
            if (typeof(T) == typeof(double))
                return (T)GetDouble(index);

            return (T)_valueBuffer[index];
        }

        object GetBoolean(int index)
        {
            return (short)_valueBuffer[index] == 1;
        }


        object GetGuid(int index)
        {
            return new Guid((byte[])_valueBuffer[index]);
        }
        object GetByte(int index)
        {
            return Convert.ToByte(_valueBuffer[index]);
        }
        object GetChar(int index)
        {
            return Convert.ToChar(_valueBuffer[index]);
        }
        object GetDouble(int index)
        {
            return Convert.ToDouble(_valueBuffer[index]);
        }

        public virtual int Count
        {
            get { return _valueBuffer.Length; }
        }
    }
}
