using EasyMigLib.Schema;
using System.Collections.Generic;

namespace EasyMigLib.Query.SqlClient
{
    public class SqlQueryService : QueryService
    {
        public SqlQueryService()
        {
            // this.SetDefaultDelimiter("\rGO\r");
        }

        public override string FormatWithSchemaName(string value)
        {
            // [dbo].[table1]
            return this.StartQuote + "dbo" + this.EndQuote + "." + this.StartQuote + value + this.EndQuote;
        }

        public override string GetColumnType(ColumnType columnType)
        {
            if (columnType is IntColumnType)
            {
                return "INT";
            }
            else if (columnType is VarCharColumnType)
            {
                var column = columnType as VarCharColumnType;
                return "NVARCHAR(" + column.Length + ")";
            }
            else if (columnType is TextColumnType)
            {
                return "NVARCHAR(MAX)";
            }
            else if (columnType is BitColumnType)
            {
                return "BIT";
            }
            else if (columnType is FloatColumnType)
            {
                var column = columnType as FloatColumnType;
                return column.Digits.HasValue ? "DECIMAL(18," + column.Digits.Value + ")" : "FLOAT";
            }
            else if (columnType is DateTimeColumnType)
            {
                return "DATETIME";
            }
            else if (columnType is TimestampColumnType)
            {
                return "TIMESTAMP";
            }
            else if (columnType is CharColumnType)
            {
                var column = columnType as CharColumnType;
                return "NCHAR(" + column.Length + ")";
            }
            else if (columnType is LongTextColumnType)
            {
                return "NTEXT";
            }
            else if (columnType is TinyIntColumnType)
            {
                return "TINYINT";
            }
            else if (columnType is SmallIntColumnType)
            {
                return "SMALLINT";
            }
            else if (columnType is BigIntColumnType)
            {
                return "BIGINT";
            }
            else if (columnType is DateColumnType)
            {
                return "DATE";
            }
            else if (columnType is TimeColumnType)
            {
                return "TIME";
            }
            else if (columnType is BlobColumnType)
            {
                return "VARBINARY(MAX)";
            }
            else if (columnType is CustomColumnType)
            {
                return ((CustomColumnType)columnType).Name;
            }
            throw new EasyMigException("Type not supported");
        }

        public override string GetColumn(MigrationColumn column)
        {
            return this.WrapWithQuotes(column.ColumnName) + " " + this.GetColumnType(column.ColumnType)
                  + (column.Nullable ? " NULL" : " NOT NULL")
                  + (column is PrimaryKeyColumn && ((PrimaryKeyColumn)column).AutoIncrement ? " IDENTITY(1,1)" : "")
                  + (column.Unique? " UNIQUE" : "")
                  + (column.DefaultValue != null ? " DEFAULT " + this.FormatValueString(column.DefaultValue) : "");
        }

        public string GetModifedColumn(MigrationColumn column)
        {
            return this.WrapWithQuotes(column.ColumnName) + " " + this.GetColumnType(column.ColumnType) 
                + (column.Nullable ? " NULL" : " NOT NULL");
        }

        public override string GetModifyColumn(string tableName, MigrationColumn column)
        {
            return "ALTER TABLE " + this.FormatWithSchemaName(tableName) + " ALTER COLUMN " + this.GetModifedColumn(column) + this.GetDefaultDelimiter();
        }

        public bool HasIdentityPrimaryKey(CreateTableSchema table)
        {
            foreach (var primaryKey in table.primaryKeys)
            {
                if (primaryKey.Value.AutoIncrement)
                {
                    return true;
                }
            }
            return false;
        }

        public override string GetSeeds(CreateTableSchema table)
        {
            var result = "";
            foreach (var seedRowCommand in table.seedTable.rows)
            {
                result += this.GetSeedRow(seedRowCommand.TableName, seedRowCommand.columnValues);
            }
            return result;
        }

        public string GetBody(string body)
        {
            return "\rAS\rBEGIN\r" + this.FormatBody(body) + "\rEND";
        }

        public override string GetCreateStoredProcedure(string procedureName, Dictionary<string, StoredProcedureParameter> parameters, string body)
        {
            return "CREATE PROCEDURE " + this.FormatWithSchemaName(procedureName) + " "
                 + this.GetParameters(parameters)
                 + this.GetBody(body) + this.GetDefaultDelimiter();
        }

    }
}

