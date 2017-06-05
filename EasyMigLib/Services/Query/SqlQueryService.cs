using EasyMigLib.Commands;
using System;
using System.Collections.Generic;

namespace EasyMigLib.Services
{
    public class SqlQueryService : QueryService
    {
        public SqlQueryService(string engine = null)
        { }

        public override string FormatTableName(string value)
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
            else if (columnType is StringColumnType)
            {
                var column = columnType as StringColumnType;
                return "NVARCHAR(" + column.Length + ")";
            }
            else if (columnType is TextColumnType)
            {
                return "NVARCHAR(MAX)";
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
            return "ALTER TABLE " + this.FormatTableName(tableName) + " ALTER COLUMN " + this.GetModifedColumn(column) + ";\r";
        }

        public string SetIdentityOn(string tableName)
        {
            return "SET IDENTITY_INSERT " + this.FormatTableName(tableName) + " ON;\r";
        }

        public string SetIdentityOff(string tableName)
        {
            return "SET IDENTITY_INSERT " + this.FormatTableName(tableName) + " OFF;\r";
        }

        public bool HasIdentityPrimaryKey(CreateTableCommand createTableCommand)
        {
            foreach (var primaryKey in createTableCommand.primaryKeys)
            {
                if (primaryKey.Value.AutoIncrement)
                {
                    return true;
                }
            }
            return false;
        }

        public override string GetSeeds(CreateTableCommand createTableCommand)
        {
            var result = "";
            var hasIdentity = this.HasIdentityPrimaryKey(createTableCommand);

            if (hasIdentity)
            {
                result += this.SetIdentityOn(createTableCommand.TableName);
            }

            foreach (var seedRowCommand in createTableCommand.seedTableCommand.seedRowCommands)
            {
                result += this.GetSeedRow(seedRowCommand.TableName, seedRowCommand.columnValues);
            }

            if (hasIdentity)
            {
                result += this.SetIdentityOff(createTableCommand.TableName);
            }

            return result;
        }

    }
}

