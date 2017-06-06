using EasyMigLib.Services;
using System;
using System.Collections.Generic;

namespace EasyMigLib.Commands
{

    public class CreateTableCommand
    {
        internal Dictionary<string, PrimaryKeyColumn> primaryKeys;
        internal Dictionary<string, ForeignKeyColumn> foreignKeys;
        internal Dictionary<string, MigrationColumn> columns;
        internal SeedTableCommand seedTableCommand;

        public string TableName { get; protected set; }
        public bool Timestamps { get; protected set; }

        public bool HasPrimaryKeys => this.primaryKeys.Count > 0;
        public bool HasForeignKeys => this.foreignKeys.Count > 0;
        public bool HasColumns => this.columns.Count > 0 || this.HasPrimaryKeys || this.HasForeignKeys;
        public int SeedRowCount => this.seedTableCommand.RowCount;
        public bool HasSeeds => this.SeedRowCount > 0;

        public CreateTableCommand(string tableName)
        {
            this.primaryKeys = new Dictionary<string, PrimaryKeyColumn>();
            this.foreignKeys = new Dictionary<string, ForeignKeyColumn>();
            this.columns = new Dictionary<string, MigrationColumn>();
            this.TableName = tableName;
            this.seedTableCommand = new SeedTableCommand(this.TableName);
        }

        // primary key

        public bool HasPrimaryKey(string columnName)
        {
            return this.primaryKeys.ContainsKey(columnName);
        }

        public PrimaryKeyColumn GetPrimaryKey(string columnName)
        {
            if (!this.HasPrimaryKey(columnName)) { throw new Exception("Primary key " + columnName + "  not registered for " + this.TableName); }

            return this.primaryKeys[columnName];
        }

        public CreateTableCommand AddPrimaryKey(string columnName, ColumnType columnType, bool autoIncrement = false)
        {
            if (this.HasColumn(columnName)) { throw new Exception("Column " + columnName + " already registered for " + this.TableName); }
            if (columnType.GetType() != typeof(IntColumnType) && autoIncrement) { throw new Exception("Invalid type. Column " + columnName + " cannot be auto incremetented"); }

            this.primaryKeys[columnName] = new PrimaryKeyColumn(columnName, columnType, autoIncrement);
            return this;
        }

        public CreateTableCommand AddPrimaryKey(string columnName)
        {
            return this.AddPrimaryKey(columnName, new IntColumnType(true), true);
        }

        // foreign key

        public bool HasForeignKey(string columnName)
        {
            return this.foreignKeys.ContainsKey(columnName);
        }

        public ForeignKeyColumn GetForeignKey(string columnName)
        {
            if (!this.HasForeignKey(columnName)) { throw new Exception("Foreign key " + columnName + "  not registered for " + this.TableName); }

            return this.foreignKeys[columnName];
        }

        public CreateTableCommand AddForeignKey(string columnName, ColumnType columnType, string tableReferenced, string primaryKeyReferenced, bool nullable = false, object defaultValue = null)
        {
            if (this.HasColumn(columnName)) { throw new Exception("Column " + columnName + " already registered for " + this.TableName); }
            if (!columnType.CheckDefaultValue(defaultValue)) { throw new Exception("Invalid default for " + columnName + " with " + this.TableName); }

            this.foreignKeys[columnName] = new ForeignKeyColumn(columnName, columnType, tableReferenced, primaryKeyReferenced, nullable, defaultValue);
            return this;
        }

        public CreateTableCommand AddForeignKey(string columnName, string tableReferenced, string primaryKeyReferenced, bool nullable = false)
        {
            return this.AddForeignKey(columnName, ColumnType.Int(true), tableReferenced, primaryKeyReferenced, nullable);
        }

        // column

        public bool HasColumn(string columnName)
        {
            return this.columns.ContainsKey(columnName)
                || this.HasPrimaryKey(columnName)
                || this.HasForeignKey(columnName);
        }

        public MigrationColumn GetColumn(string columnName)
        {
            if (!this.HasColumn(columnName)) { throw new Exception("Column " + columnName + " not registered for " + this.TableName); }

            if (this.HasPrimaryKey(columnName))
            {
                return this.GetPrimaryKey(columnName);
            }
            else if (this.HasForeignKey(columnName))
            {
                return this.GetForeignKey(columnName);
            }
            else
            {
                return this.columns[columnName];
            }
        }

        public CreateTableCommand AddColumn(string columnName, ColumnType columnType, bool nullable = false, object defaultValue = null, bool unique = false)
        {
            if (this.HasColumn(columnName)) { throw new Exception("Column " + columnName + " already registered for " + this.TableName); }
            if (!columnType.CheckDefaultValue(defaultValue)) { throw new Exception("Invalid default for " + columnName + " with " + this.TableName); }

            this.columns[columnName] = new MigrationColumn(columnName, columnType, nullable, defaultValue, unique);
            return this;
        }

        public CreateTableCommand AddColumn(string columnName, bool nullable = false)
        {
            return this.AddColumn(columnName, new VarCharColumnType(), nullable);
        }

        public CreateTableCommand AddTimestamps()
        {
            this.Timestamps = true;
            return this;
        }

        public Dictionary<string, MigrationColumn> GetColumns()
        {
            var result = new Dictionary<string, MigrationColumn>();
            foreach (var column in this.primaryKeys)
            {
                result[column.Key] = column.Value;
            }
            foreach (var column in this.columns)
            {
                result[column.Key] = column.Value;
            }
            foreach (var column in this.foreignKeys)
            {
                result[column.Key] = column.Value;
            }
            return result;
        }

        public string GetQuery(QueryService queryService)
        {
            return queryService.GetCreateTable(this);
        }

        // seed on init (auto increment off)

        public CreateTableCommand Insert(Dictionary<string, object> columnValues)
        {
            this.seedTableCommand.Insert(columnValues);
            return this;
        }

        public CreateTableCommand Insert(SeedData seedData)
        {
            return this.Insert(seedData.container);
        }

        public Dictionary<string, object> GetSeedRow(int index)
        {
            return this.seedTableCommand.GetRow(index);
        }

        public string[] GetPrimaryKeyNames()
        {
            var result = new List<string>();
            foreach (var primaryKey in this.primaryKeys)
            {
                result.Add(primaryKey.Value.ColumnName);
            }
            return result.ToArray();
        }

        public List<string> GetReferencedTableList()
        {
            var result = new List<string>();
            foreach (var foreignKey in this.foreignKeys)
            {
                result.Add(foreignKey.Value.TableReferenced);
            }
            return result;
        }
    }
}
