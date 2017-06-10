using System.Collections.Generic;

namespace EasyMigLib.Schema
{
    public class AlterTableSchema
    {
        public string TableName { get; }

        internal Dictionary<string, MigrationColumn> columnsToAdd;
        internal Dictionary<string, MigrationColumn> columnsToModify;
        internal List<string> columnsToDrop;

        internal PrimaryKeyConstraint primaryKeyConstraint;
        internal Dictionary<string, ForeignKeyConstraint> foreignKeyConstraints;

        public bool HasColumnsToAdd => this.ColumnsToAddCount > 0;
        public bool HasColumnsToModify => this.ColumnsToModifyCount > 0;
        public bool HasColumnsToDrop => this.ColumnsToDropCount > 0;
        public bool HasPrimaryKeyConstraint => this.primaryKeyConstraint != null;
        public bool HasForeignKeyConstraints => this.ForeignKeyConstraintsCount > 0;

        public int ColumnsToAddCount => this.columnsToAdd.Count;
        public int ColumnsToModifyCount => this.columnsToModify.Count;
        public int ColumnsToDropCount => this.columnsToDrop.Count;
        public int ForeignKeyConstraintsCount => this.foreignKeyConstraints.Count;

        public AlterTableSchema(string tableName)
        {
            this.TableName = tableName;

            this.columnsToAdd = new Dictionary<string, MigrationColumn>();
            this.columnsToModify = new Dictionary<string, MigrationColumn>();
            this.columnsToDrop = new List<string>();

            this.foreignKeyConstraints = new Dictionary<string, ForeignKeyConstraint>();
        }

        // add column

        public bool HasColumnToAdd(string columnName)
        {
            return this.columnsToAdd.ContainsKey(columnName);
        }

        public MigrationColumn GetColumnToAdd(string columnName)
        {
            if (!this.HasColumnToAdd(columnName)) { throw new EasyMigException("No column " + columnName + " to add registered for " + this.TableName); }

            return this.columnsToAdd[columnName];
        }

        public AlterTableSchema AddColumn(string columnName, ColumnType columnType, bool nullable = false, object defaultValue = null, bool unique = false)
        {
            if (this.HasColumnToAdd(columnName)) { throw new EasyMigException("Column to add to " + columnName + " already registered"); }

            this.columnsToAdd[columnName] = new MigrationColumn(this.TableName, columnName, columnType, nullable, defaultValue, unique);
            return this;
        }

        public AlterTableSchema AddColumn(string columnName, bool nullable = false)
        {
            return this.AddColumn(columnName, ColumnType.VarChar(), nullable);
        }

        // modify column

        public bool HasColumnToModify(string columnName)
        {
            return this.columnsToModify.ContainsKey(columnName);
        }

        public MigrationColumn GetColumnToModify(string columnName)
        {
            if (!this.HasColumnToModify(columnName)) { throw new EasyMigException("No column " + columnName + " to modify registered for " + this.TableName); }

            return this.columnsToModify[columnName];
        }


        public AlterTableSchema ModifyColumn(string columnName, ColumnType columnType, bool nullable, object defaultValue, bool unique)
        {
            if (this.HasColumnToModify(columnName)) { throw new EasyMigException("Column to modify " + columnName +  " already registered"); }

            this.columnsToModify[columnName] = new MigrationColumn(this.TableName, columnName, columnType, nullable, defaultValue, unique);
            return this;
        }

        public AlterTableSchema ModifyColumn(string columnName, ColumnType columnType, bool nullable = false)
        {
            return this.ModifyColumn(columnName, columnType, nullable, null, false);
        }

        // drop column

        public bool HasColumnToDrop(string columnName)
        {
            return this.columnsToDrop.Contains(columnName);
        }

        public AlterTableSchema DropColumn(string columnName)
        {
            if (!this.HasColumnToDrop(columnName))
            {
                this.columnsToDrop.Add(columnName);
            }
            return this;
        }

        // primary key

        public PrimaryKeyConstraint GetPrimaryKeyConstraint()
        {
            return this.primaryKeyConstraint;
        }

        public AlterTableSchema AddPrimaryKeyConstraint(params string[] primaryKeys)
        {
            if (this.HasPrimaryKeyConstraint) { throw new EasyMigException("Primary key constraint already defined for " + this.TableName); }

            this.primaryKeyConstraint = new PrimaryKeyConstraint(this.TableName, primaryKeys);
            return this;
        }

        // foreign key

        public bool HasForeignKeyConstraint(string columnName)
        {
            return this.foreignKeyConstraints.ContainsKey(columnName);
        }

        public ForeignKeyConstraint GetForeignKeyConstraint(string columnName)
        {
            if (!this.HasForeignKeyConstraint(columnName)) { throw new EasyMigException("No foreign key constraint defined for " + columnName + " on " + this.TableName); }

            return this.foreignKeyConstraints[columnName];
        }

        public AlterTableSchema AddForeignKeyConstraint(string columnName, ColumnType columnType, string tableReferenced, string primaryKeyReferenced)
        {
            if (this.HasForeignKeyConstraint(columnName)) { throw new EasyMigException("A foreign key constraint already defined for " + columnName + " on " + this.TableName); }

            this.foreignKeyConstraints[columnName] = new ForeignKeyConstraint(this.TableName, columnName, columnType, tableReferenced, primaryKeyReferenced);
            return this;
        }

        public AlterTableSchema AddForeignKeyConstraint(string columnName, string tableReferenced, string primaryKeyReferenced)
        {
            return this.AddForeignKeyConstraint(columnName, ColumnType.Int(true), tableReferenced, primaryKeyReferenced);
        }
    }
}
