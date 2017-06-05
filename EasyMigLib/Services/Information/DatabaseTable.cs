using System;
using System.Collections.Generic;

namespace EasyMigLib.Services
{
    public class DatabaseTable
    {
        public string TableName { get;  }
        public Dictionary<string, object> Table { get; }
        public Dictionary<string, Dictionary<string, object>> Columns { get; }
        public Dictionary<string, Dictionary<string, object>> PrimaryKeys { get; }
        public Dictionary<string, Dictionary<string, object>> ForeignKeys { get; }

        public DatabaseTable(
            string tableName, 
            Dictionary<string, object> table, 
            Dictionary<string, Dictionary<string, object>> columns, 
            Dictionary<string, Dictionary<string, object>> primaryKeys,
            Dictionary<string, Dictionary<string, object>> foreignKeys)
        {
            this.TableName = tableName;
            this.Table = table;
            this.Columns = columns;
            this.PrimaryKeys = primaryKeys;
            this.ForeignKeys = foreignKeys;
        }

        public bool HasColumn(string columnName)
        {
            return this.Columns.ContainsKey(columnName);
        }

        public Dictionary<string, object> GetColumn(string columnName)
        {
            if(!this.HasColumn(columnName)) { throw new Exception("No column " + columnName + " found in " + this.TableName); }
            return this.Columns[columnName];
        }

        public bool IsPrimaryKey(string columnName)
        {
            return this.PrimaryKeys.ContainsKey(columnName);
        }

        public Dictionary<string, object> GetPrimaryKey(string columnName)
        {
            if (this.IsPrimaryKey(columnName))
            {
                return this.PrimaryKeys[columnName];
            }
            return null;
        }

        public bool IsForeignKey(string columnName)
        {
            return this.ForeignKeys.ContainsKey(columnName);
        }

        public Dictionary<string, object> GetForeignKey(string columnName)
        {
            if (this.IsForeignKey(columnName))
            {
                return this.ForeignKeys[columnName];
            }
            return null;
        }
    }

}
