using EasyMigLib.Commands;
using System.Collections.Generic;
using System;

namespace EasyMigLib.Query
{
    public abstract class QueryService : IQueryService
    {
        public string StartQuote { get; }
        public string EndQuote { get; }
        public string Delimiter { get; protected set;  }

        public abstract string GetColumnType(ColumnType columnType);
        public abstract string GetColumn(MigrationColumn column);
        public abstract string GetCreateStoredProcedure(string procedureName, Dictionary<string, DatabaseParameter> parameters, string body);

        protected List<string> reservedWords;

        public QueryService(string startQuote = "[", string endQuote = "]")
        {
            this.StartQuote = startQuote;
            this.EndQuote = endQuote;
            this.reservedWords = this.GetReservedWords();
            this.Delimiter = ";\r";
        }

        public void SetDefaultDelimiter(string delimiter)
        {
            this.Delimiter = delimiter;
        }

        public virtual string GetDefaultDelimiter()
        {
            return this.Delimiter;
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

        public virtual string FormatWithSchemaName(string value)
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
            return "CREATE DATABASE " + this.WrapWithQuotes(databaseName) + this.GetDefaultDelimiter();
        }

        public virtual string GetUseDatabase(string databaseName)
        {
            return "USE " + this.WrapWithQuotes(databaseName) + this.GetDefaultDelimiter();
        }

        public virtual string GetCreateAndUseDatabase(string databaseName)
        {
            return this.GetCreateDatabase(databaseName)
                + this.GetUseDatabase(databaseName); 
        }

        public virtual string GetDropDatabase(string databaseName)
        {
            return "DROP DATABASE IF EXISTS " + this.WrapWithQuotes(databaseName) + this.GetDefaultDelimiter();
        }

        // table

        public virtual string GetDropTable(string tableName)
        {
            return "DROP TABLE IF EXISTS " + this.FormatWithSchemaName(tableName) + this.GetDefaultDelimiter();
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

            return "CREATE TABLE " + this.FormatWithSchemaName(table.TableName) + " (\r"
              + string.Join(",\r", result)
              + "\r)" + this.GetDefaultDelimiter();
        }

        // alter table

        public virtual string GetAddColumn(string tableName, MigrationColumn column)
        {
            return "ALTER TABLE " + this.FormatWithSchemaName(tableName) + " ADD " + this.GetColumn(column) + this.GetDefaultDelimiter();
        }

        public virtual string GetModifyColumn(string tableName, MigrationColumn column)
        {
            return "ALTER TABLE " + this.FormatWithSchemaName(tableName) + " ALTER COLUMN " + this.GetColumn(column) + this.GetDefaultDelimiter();
        }

        public virtual string GetDropColumn(string tableName, string columnName)
        {
            return "ALTER TABLE " + this.FormatWithSchemaName(tableName) + " DROP COLUMN " + this.WrapWithQuotes(columnName) + this.GetDefaultDelimiter();
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

                return "ALTER TABLE " + this.FormatWithSchemaName(tableName) + " ADD PRIMARY KEY (" + string.Join(",", formattedPrimaryKeys) + ")" + this.GetDefaultDelimiter();
            }
            else
            {
                return "";
            }
        }

        public virtual string GetAddForeignKeyConstraint(string tableName, ForeignKeyColumn foreignKey)
        {
            return "ALTER TABLE " + this.FormatWithSchemaName(tableName)
                    + " ADD FOREIGN KEY (" + this.WrapWithQuotes(foreignKey.ColumnName)
                    + ") REFERENCES " + this.FormatWithSchemaName(foreignKey.TableReferenced)
                    + "(" + this.WrapWithQuotes(foreignKey.PrimaryKeyReferenced) + ")" + this.GetDefaultDelimiter();

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

                return "ALTER TABLE " + this.FormatWithSchemaName(createTableCommand.TableName) + " ADD PRIMARY KEY (" + string.Join(",", formattedPrimaryKeys) + ")" + this.GetDefaultDelimiter();
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
            return "INSERT INTO " + this.FormatWithSchemaName(tableName) + " ("
                + this.GetSeedColumns(columnValues) + ") VALUES ("
                + this.GetSeedValues(columnValues) + ")" + this.GetDefaultDelimiter();
        }

        public virtual string GetSeeds(CreateTableCommand createTableCommand)
        {
            var result = "";
            foreach (var seedRowCommand in createTableCommand.seedTableCommand.seedRowCommands)
            {
                result += this.GetSeedRow(seedRowCommand.TableName, seedRowCommand.columnValues);
            }
            return result;
        }

        // stored procedure

        public virtual string GetParameter(DatabaseParameter parameter)
        {
            return parameter.ParameterName + " " + this.GetColumnType(parameter.ParameterType)
             + (parameter.DefaultValue != null ? "=" + parameter.DefaultValue : "")
             + (parameter.Direction != DatabaseParameterDirection.IN ? " " + parameter.Direction.ToString() : "");
        }

        public virtual string GetParameters(Dictionary<string, DatabaseParameter> parameters)
        {
            var result = new List<string>();
            foreach (var parameter in parameters)
            {
                result.Add(this.GetParameter(parameter.Value));
            }
            return string.Join(",", result);
        }

        public virtual string GetDropStoredProcedure(string procedureName, bool delimiter = true)
        {
            return "DROP PROCEDURE IF EXISTS " + this.FormatWithSchemaName(procedureName) 
                + (delimiter ? this.GetDefaultDelimiter() : "");
        }

        public virtual string FormatBody(string body)
        {
            if (!string.IsNullOrWhiteSpace(body))
            {
                return body.EndsWith(";") ? body : body + ";";
            }
            return "";
        }
    }
}

