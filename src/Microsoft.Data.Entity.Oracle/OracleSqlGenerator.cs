// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System.Text;
using Microsoft.Data.Entity.Relational;
using Microsoft.Data.Entity.Relational.Update;
using Microsoft.Data.Entity.Oracle.Utilities;

namespace Microsoft.Data.Entity.Oracle
{
    public class SqlServerSqlGenerator : SqlGenerator
    {
        public override void AppendBatchHeader(StringBuilder commandStringBuilder)
        {
            Check.NotNull(commandStringBuilder, "commandStringBuilder");

            commandStringBuilder.AppendLine("BEGIN");
            // Dummy statement due to ModificationCommandBatch structure
            commandStringBuilder.Append("NULL");

//            var batch = @"BEGIN
//                select * from table1;
//                select * from table2;
//                select * from table3;
//            END;";
        }

        public override void AppendBatchFooter(StringBuilder commandStringBuilder)
        {
            Check.NotNull(commandStringBuilder, "commandStringBuilder");
            commandStringBuilder.AppendLine();
            commandStringBuilder.Append("END;");
        }

        

        public override string QuoteIdentifier(string identifier)
        {
            Check.NotNull(identifier, "identifier");

            return "\"" + identifier.Replace("\"", "\"\"") + "\"";
        }

        protected override void AppendIdentityWhereCondition(StringBuilder commandStringBuilder, ColumnModification columnModification)
        {
            commandStringBuilder
                .Append(QuoteIdentifier(columnModification.ColumnName))
                .Append(" = ")
                .Append("scope_identity()");
        }
    }
}
