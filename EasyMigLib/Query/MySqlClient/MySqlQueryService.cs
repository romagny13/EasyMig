using System.Collections.Generic;
using EasyMigLib.Schema;

namespace EasyMigLib.Query.MySqlClient
{
    public class MySqlQueryService : QueryService
    {
        public string Engine { get; protected set; }

        public MySqlQueryService(string engine = null)
            :base("`","`")
        {
            this.SetEngine(engine);
        }

        // engine

        public bool CheckEngine(string value)
        {
            return value == "InnoDB" || value == "MyISAM";
        }

        public void SetEngine(string engine)
        {
            if (string.IsNullOrEmpty(engine))
            {
                this.Engine = "InnoDB";
            }
            else
            {
                if (!this.CheckEngine(engine)) { throw new EasyMigException("Invalid MySQL engine. Supported: MyISAM or InnoDB (by default)"); }

                this.Engine = engine;
            }
        }

        public string GetTableEngine()
        {
            if (this.Engine == "InnoDB")
            {
                return "ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci";
            }
            else if (this.Engine == "MyISAM")
            {
                return "ENGINE=MyISAM DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci";
            }
            else
            {
                return "";
            }
        }

        public override string GetColumnType(ColumnType columnType)
        {
            if (columnType is IntColumnType)
            {
                var colum = columnType as IntColumnType;
                return colum.Unsigned ? "INT UNSIGNED" : "INT";
            }
            else if (columnType is VarCharColumnType)
            {
                var column = columnType as VarCharColumnType;
                return "VARCHAR(" + column.Length + ") COLLATE utf8mb4_unicode_ci";
            }
            else if (columnType is TextColumnType)
            {
                return "TEXT COLLATE utf8mb4_unicode_ci";
            }
            else if (columnType is BitColumnType)
            {
                return "BIT";
            }
            else if (columnType is FloatColumnType)
            {
                var column = columnType as FloatColumnType;
                return column.Digits.HasValue ? "FLOAT(10," + column.Digits.Value + ")" : "FLOAT";
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
                return "CHAR(" + column.Length + ") COLLATE utf8mb4_unicode_ci";
            }
            else if (columnType is LongTextColumnType)
            {
                return "LONGTEXT COLLATE utf8mb4_unicode_ci";
            }
            else if (columnType is TinyIntColumnType)
            {
                var colum = columnType as TinyIntColumnType;
                return colum.Unsigned ? "TINYINT UNSIGNED" : "TINYINT";
            }
            else if (columnType is SmallIntColumnType)
            {
                var colum = columnType as SmallIntColumnType;
                return colum.Unsigned ? "SMALLINT UNSIGNED" : "SMALLINT";
            }
            else if (columnType is BigIntColumnType)
            {
                var colum = columnType as BigIntColumnType;
                return colum.Unsigned ? "BIGINT UNSIGNED" : "BIGINT";
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
                return "BLOB";
            }
            throw new EasyMigException("Type not supported");
        }

        public override string GetColumn(MigrationColumn column)
        {
            return this.WrapWithQuotes(column.ColumnName) + " " + this.GetColumnType(column.ColumnType)
                + (column.Nullable ? " NULL" : " NOT NULL")
                + (column.DefaultValue != null ? " DEFAULT " + this.FormatValueString(column.DefaultValue) : "")
                + (column.Unique ? " UNIQUE" : "");
        }

        public override void AddTimestamps(CreateTableSchema table)
        {
            if (table.Timestamps
               && !table.HasColumn("created_at")
               && !table.HasColumn("updated_at"))
            {
                table.AddColumn("created_at", ColumnType.Timestamp(), true, "CURRENT_TIMESTAMP");
                table.AddColumn("updated_at", ColumnType.Timestamp(), true);
            }
        }

        public override string GetCreateTable(CreateTableSchema table)
        {
            var result = new List<string>();

            this.AddTimestamps(table);

            var columns = table.GetColumns();
            foreach (var column in columns)
            {
                result.Add("\t" + this.GetColumn(column.Value));
            }

            // unique
            foreach (var column in table.columns)
            {
                if (column.Value.Unique)
                {
                    result.Add("\tUNIQUE (" + this.WrapWithQuotes(column.Key) + ")");
                }
            }

            return "CREATE TABLE " + this.FormatWithSchemaName(table.TableName) + " (\r"
                + string.Join(",\r", result)
                + "\r) " + this.GetTableEngine() + ";\r";
        }

        public override string GetModifyColumn(string tableName, MigrationColumn column)
        {
            return "ALTER TABLE " + this.FormatWithSchemaName(tableName) + " MODIFY COLUMN " + this.GetColumn(column) + ";\r";
        }

        public string GetAutoIncrement(CreateTableSchema table)
        {
            var result = "";
            foreach (var column in table.primaryKeys)
            {
                if (column.Value.AutoIncrement)
                {
                    result += "ALTER TABLE " + this.FormatWithSchemaName(table.TableName) + " MODIFY " + this.WrapWithQuotes(column.Key) + " INT UNSIGNED NOT NULL AUTO_INCREMENT;\r";
                }
            }
            return result;
        }

        public override string GetAddPrimaryKeyConstraint(CreateTableSchema table)
        {
            var primaryKeys = table.GetPrimaryKeyNames();
            if (primaryKeys.Length > 0)
            {
                var formattedPrimaryKeys = new List<string>();
                foreach (var primaryKey in primaryKeys)
                {
                    formattedPrimaryKeys.Add(this.WrapWithQuotes(primaryKey));
                }
                return "ALTER TABLE " + this.FormatWithSchemaName(table.TableName) + " ADD PRIMARY KEY (" + string.Join(",", formattedPrimaryKeys) + ");\r" + this.GetAutoIncrement(table);
            }
            else
            {
                return "";
            }
        }

        public override string GetAddForeignKeyConstraints(CreateTableSchema table)
        {
            var result = "";
            foreach (var foreignKey in table.foreignKeys)
            {
                result += "CREATE INDEX " + this.WrapWithQuotes(foreignKey.Value.ColumnName + "_index") + " ON " + this.FormatWithSchemaName(table.TableName) + " (" + this.WrapWithQuotes(foreignKey.Value.ColumnName) + ");\r";
                result += "ALTER TABLE " + this.FormatWithSchemaName(table.TableName) + " ADD FOREIGN KEY (" + this.WrapWithQuotes(foreignKey.Value.ColumnName) + ") REFERENCES " + this.FormatWithSchemaName(foreignKey.Value.TableReferenced) + "(" + this.WrapWithQuotes(foreignKey.Value.PrimaryKeyReferenced) + ");\r";
            }
            return result;
        }

        // stored procedure

        public override string GetParameter(StoredProcedureParameter parameter)
        {
            return (parameter.Direction != StoredProcedureParameterDirection.IN ? parameter.Direction.ToString() + " " : "")
                + parameter.ParameterName
                + " " + this.GetColumnType(parameter.ParameterType);
        }

        public string GetBody(string body)
        {
            return "\rBEGIN\r" + this.FormatBody(body) + "\rEND";
        }

        public override string GetCreateStoredProcedure(string procedureName, Dictionary<string, StoredProcedureParameter> parameters, string body)
        {
            return @"CREATE PROCEDURE " + this.FormatWithSchemaName(procedureName) + "(" + this.GetParameters(parameters) +")"
                   + this.GetBody(body);
        }

    }
}

