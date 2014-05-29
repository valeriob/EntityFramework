using Microsoft.Data.Entity.Relational.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Data.Entity.Relational.Model.Oracle
{
    class OracleRelationalTypeMapping : RelationalTypeMapping
    {
        public OracleRelationalTypeMapping(string storeTypeName, DbType storeType) : base(storeTypeName, storeType)
        {
        }
        protected override void ConfigureParameter(System.Data.Common.DbParameter parameter, Update.ColumnModification columnModification)
        {
            base.ConfigureParameter(parameter, columnModification);
            if (columnModification.Property.PropertyType == typeof(Guid))
            {
                object value = columnModification.StateEntry[columnModification.Property];
                if (value == null)
                    parameter.Value = DBNull.Value;
                else
                    parameter.Value = ((Guid)value).ToByteArray();
            }
        }
    }
}
