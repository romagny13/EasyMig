using EasyMigLib.Commands;
using System.Collections.Generic;
using System;

namespace EasyMigLib.Services
{
    public abstract class QueryService
    {
        public string StartQuote { get; }
        public string EndQuote { get; }

        public abstract string GetColumnType(ColumnType columnType);
        public abstract string GetColumn(MigrationColumn column);

        protected List<string> reservedWords;

        public QueryService(string startQuote = "[", string endQuote = "]")
        {
            this.StartQuote = startQuote;
            this.EndQuote = endQuote;
            this.reservedWords = this.GetReservedWords();
        }

        // reserved words

        public virtual List<string> GetReservedWords()
        {
            return new List<string> { "CURRENT_TIMESTAMP" };
        }

        public virtual bool IsReservedWord(string value)
        {
            return this.reservedWords.Contains(value);
        }

        // format

        public virtual string FormatTableName(string value)
        {
            // [table1]
            return this.StartQuote + value + this.EndQuote;
        }

        public virtual string WrapWithQuotes(string value)
        {
            // [table1] or [column1]
            return this.StartQuote + value + this.EndQuote;
        }

        // database

        public virtual string GetCreateDatabase(string databaseName)
        {
            return "CREATE DATABASE " + this.WrapWithQuotes(databaseName) + ";\r";
        }

        public virtual string GetDropDatabase(string databaseName)
        {
            return "DROP DATABASE IF EXISTS " + this.WrapWithQuotes(databaseName) + ";\r";
        }

        // table

        public virtual string GetDropTable(string tableName)
        {
            return "DROP TABLE IF EXISTS " + this.FormatTableName(tableName) + ";\r";
        }

        public virtual void AddTimestamps(CreateTableCommand table)
        {
            if (table.Timestamps
               && !table.HasColumn("created_at")
               && !table.HasColumn("updated_at"))
            {
                table.AddColumn("created_at", ColumnType.DateTime(), true, "CURRENT_TIMESTAMP");
                table.AddColumn("updated_at", ColumnType.DateTime(), true);
            }
        }

        public virtual string FormatValueString(object value)
        {
            // used with seed and default value
            if (value == null)
            {
                return "NULL";
            }
            else if (value.GetType() == typeof(string))
            {
                if (this.IsReservedWord((string)value))
                {
                    return value.ToString();
                }
                else
                {
                    // escape ' => ''
                    value = value.ToString().Replace("'", "''");
                    return "'" + value + "'";
                }
            }
            else
            {
                return value.ToString();
            }
        }

        public virtual string GetCreateTable(CreateTableCommand table)
        {
            var result = new List<string>();

            this.AddTimestamps(table);

            var columns = table.GetColumns();
            foreach (var column in columns)
            {
                result.Add("\t" + this.GetColumn(column.Value));
            }

            return "CREATE TABLE " + this.FormatTableName(table.TableName) + " (\r"
              + string.Join(",\r", result)
              + "\r);\r";
        }

        // alter table

        public virtual string GetAddColumn(string tableName, MigrationColumn column)
        {
            return "ALTER TABLE " + this.FormatTableName(tableName) + " ADD " + this.GetColumn(column) + ";\r";
        }

        public virtual string GetModifyColumn(string tableName, MigrationColumn column)
        {
            return "ALTER TABLE " + this.FormatTableName(tableName) + " ALTER COLUMN " + this.GetColumn(column) + ";\r";
        }

        public virtual string GetDropColumn(string tableName, string columnName)
        {
            return "ALTER TABLE " + this.FormatTableName(tableName) + " DROP COLUMN " + this.WrapWithQuotes(columnName) + ";\r";
        }

        public virtual string GetAddPrimaryKeyConstraint(string tableName, string[] primaryKeys)
        {
            if (primaryKeys.Length > 0)
            {
                var formattedPrimaryKeys = new List<string>();
                foreach (var primaryKey in primaryKeys)
                {
                    formattedPrimaryKeys.Add(this.WrapWithQuotes(primaryKey));
                }

                return "ALTER TABLE " + this.FormatTableName(tableName) + " ADD PRIMARY KEY (" + string.Join(",", formattedPrimaryKeys) + ");\r";
            }
            else
            {
                return "";
            }
        }

        public virtual string GetAddForeignKeyConstraint(string tableName, ForeignKeyColumn foreignKey)
        {
            return "ALTER TABLE " + this.FormatTableName(tableName)
                    + " ADD FOREIGN KEY (" + this.WrapWithQuotes(foreignKey.ColumnName)
                    + ") REFERENCES " + this.FormatTableName(foreignKey.TableReferenced)
                    + "(" + this.WrapWithQuotes(foreignKey.PrimaryKeyReferenced) + ");\r";

        }

        // at initialization

        public virtual string GetAddPrimaryKeyConstraint(CreateTableCommand createTableCommand)
        {
            if (createTableCommand.HasPrimaryKeys)
            {
                var formattedPrimaryKeys = new List<string>();
                foreach (var primaryKey in createTableCommand.primaryKeys)
                {
                    formattedPrimaryKeys.Add(this.WrapWithQuotes(primaryKey.Value.ColumnName));
                }

                return "ALTER TABLE " + this.FormatTableName(createTableCommand.TableName) + " ADD PRIMARY KEY (" + string.Join(",", formattedPrimaryKeys) + ");\r";
            }
            else
            {
                return "";
            }
        }

        public virtual string GetAddForeignKeyConstraints(CreateTableCommand createTableCommand)
        {
            var result = "";
            foreach (var foreignKey in createTableCommand.foreignKeys)
            {
                result += this.GetAddForeignKeyConstraint(createTableCommand.TableName, foreignKey.Value);
            }
            return result;
        }

        // seed

        public virtual string GetSeedColumns(Dictionary<string, object> columnValues)
        {
            var result = new List<string>();
            foreach (var columnValue in columnValues)
            {
                result.Add(this.WrapWithQuotes(columnValue.Key));
            }
            return string.Join(",", result);
        }

        public virtual string GetSeedValues(Dictionary<string, object> columnValues)
        {
            var result = new List<string>();
            foreach (var columnValue in columnValues)
            {
                result.Add(this.FormatValueString(columnValue.Value));
            }
            return string.Join(",", result);
        }

        public virtual string GetSeedRow(string tableName, Dictionary<string, object> columnValues)
        {
            return "INSERT INTO " + this.FormatTableName(tableName) + " ("
                + this.GetSeedColumns(columnValues) + ") VALUES ("
                + this.GetSeedValues(columnValues) + ");\r";
        }

        public virtual string GetSeeds(CreateTableCommand createTableCommand)
        {
            var result = new List<string>();
            foreach (var seedRowCommand in createTableCommand.seedTableCommand.seedRowCommands)
            {
                result.Add(this.GetSeedRow(seedRowCommand.TableName, seedRowCommand.columnValues));
            }
            return string.Join("\r", result);
        }
    }
}

