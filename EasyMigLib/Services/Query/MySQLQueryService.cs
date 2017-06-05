﻿using EasyMigLib.Commands;
using System.Collections.Generic;
using System;

namespace EasyMigLib.Services
{
    public class MySQLQueryService : QueryService
    {
        public string Engine { get; protected set; }

        public MySQLQueryService(string engine = null)
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
                if (!this.CheckEngine(engine)) { throw new Exception("Invalid MySQL engine. Supported: MyISAM or InnoDB (by default)"); }

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
            else if (columnType is StringColumnType)
            {
                var column = columnType as StringColumnType;
                return "VARCHAR(" + column.Length + ") COLLATE utf8mb4_unicode_ci";
            }
            else if (columnType is TextColumnType)
            {
                return "TEXT COLLATE utf8mb4_unicode_ci";
            }
            else if (columnType is BooleanColumnType)
            {
                return "BIT";
            }
            else if (columnType is FloatColumnType)
            {
                return "FLOAT";
            }
            else if (columnType is DateTimeColumnType)
            {
                return "DATETIME";
            }
            else if (columnType is TimestampColumnType)
            {
                return "TIMESTAMP";
            }
            throw new Exception("Type not supported");
        }

        public override string GetColumn(MigrationColumn column)
        {
            return this.WrapWithQuotes(column.ColumnName) + " " + this.GetColumnType(column.ColumnType)
                + (column.Nullable ? " NULL" : " NOT NULL")
                + (column.DefaultValue != null ? " DEFAULT " + this.FormatValueString(column.DefaultValue) : "");
        }

        public override void AddTimestamps(CreateTableCommand table)
        {
            if (table.Timestamps
               && !table.HasColumn("created_at")
               && !table.HasColumn("updated_at"))
            {
                table.AddColumn("created_at", ColumnType.Timestamp(), true, "CURRENT_TIMESTAMP");
                table.AddColumn("updated_at", ColumnType.Timestamp(), true);
            }
        }

        public override string GetCreateTable(CreateTableCommand table)
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

            return "CREATE TABLE " + this.FormatTableName(table.TableName) + " (\r"
                + string.Join(",\r", result)
                + "\r) " + this.GetTableEngine() + ";\r";
        }

        public override string GetModifyColumn(string tableName, MigrationColumn column)
        {
            return "ALTER TABLE " + this.FormatTableName(tableName) + " MODIFY COLUMN " + this.GetColumn(column) + ";\r";
        }

        public string GetAutoIncrement(CreateTableCommand createTableCommand)
        {
            var result = "";
            foreach (var column in createTableCommand.primaryKeys)
            {
                if (column.Value.AutoIncrement)
                {
                    result += "ALTER TABLE " + this.FormatTableName(createTableCommand.TableName) + " MODIFY " + this.WrapWithQuotes(column.Key) + " INT UNSIGNED NOT NULL AUTO_INCREMENT;\r";
                }
            }
            return result;
        }

        public override string GetAddPrimaryKeyConstraint(CreateTableCommand createTableCommand)
        {
            var primaryKeys = createTableCommand.GetPrimaryKeyNames();
            if (primaryKeys.Length > 0)
            {
                var formattedPrimaryKeys = new List<string>();
                foreach (var primaryKey in primaryKeys)
                {
                    formattedPrimaryKeys.Add(this.WrapWithQuotes(primaryKey));
                }
                return "ALTER TABLE " + this.FormatTableName(createTableCommand.TableName) + " ADD PRIMARY KEY (" + string.Join(",", formattedPrimaryKeys) + ");\r" + this.GetAutoIncrement(createTableCommand);
            }
            else
            {
                return "";
            }
        }

        public override string GetAddForeignKeyConstraints(CreateTableCommand createTableCommand)
        {
            var result = "";
            foreach (var foreignKey in createTableCommand.foreignKeys)
            {
                result += "CREATE INDEX " + this.WrapWithQuotes(foreignKey.Value.ColumnName + "_index") + " ON " + this.FormatTableName(createTableCommand.TableName) + " (" + this.WrapWithQuotes(foreignKey.Value.ColumnName) + ");\r";
                result += "ALTER TABLE " + this.FormatTableName(createTableCommand.TableName) + " ADD FOREIGN KEY (" + this.WrapWithQuotes(foreignKey.Value.ColumnName) + ") REFERENCES " + this.FormatTableName(foreignKey.Value.TableReferenced) + "(" + this.WrapWithQuotes(foreignKey.Value.PrimaryKeyReferenced) + ");\r";
            }
            return result;
        }
     
    }
}
