using EasyMigLib.Query;
using System.Collections.Generic;

namespace EasyMigLib.Commands
{

    public class AlterTableCommand : DatabaseCommand
    {
        internal Dictionary<string, AddColumnCommand> addColumnsCommands;
        internal Dictionary<string, ModifyColumnCommand> modifyColumnsCommands;
        internal Dictionary<string, DropColumnCommand> dropColumnsCommands;

        internal AddPrimaryKeyConstraintCommand addPrimaryKeyConstraintCommand;
        internal Dictionary<string, AddForeignKeyConstraintCommand> addForeignKeyConstraintCommands;

        public bool HasAddColumnCommands => this.addColumnsCommands.Count > 0;
        public bool HasModifyColumnCommands => this.modifyColumnsCommands.Count > 0;
        public bool HasDropColumnCommands => this.dropColumnsCommands.Count > 0;
        public bool HasForeignKeyConstraintCommands => this.addForeignKeyConstraintCommands.Count > 0;

        public string TableName { get; protected set; }

        public AlterTableCommand(string tableName)
        {
            this.addColumnsCommands = new Dictionary<string, AddColumnCommand>();
            this.modifyColumnsCommands = new Dictionary<string, ModifyColumnCommand>();
            this.dropColumnsCommands = new Dictionary<string, DropColumnCommand>();

            this.addForeignKeyConstraintCommands = new Dictionary<string, AddForeignKeyConstraintCommand>();

            this.TableName = tableName;
        }

        // add column

        public bool HasAddColumnCommand(string columnName)
        {
            return this.addColumnsCommands.ContainsKey(columnName);
        }

        public AddColumnCommand GetAddColumnCommand(string columnName)
        {
            if (!this.HasAddColumnCommand(columnName)) { throw new EasyMigException("No AddColumnCommand registered for " + columnName + " with " + this.TableName); }

            return this.addColumnsCommands[columnName];
        }

        public AlterTableCommand AddColumn(string columnName, ColumnType columnType, bool nullable = false, object defaultValue = null, bool unique = false)
        {
            if (this.HasAddColumnCommand(columnName)) { throw new EasyMigException("AddColumnCommand " + this.TableName + " with " + columnName + " already registered"); }

            var column = new MigrationColumn(columnName, columnType, nullable, defaultValue, unique);
            this.addColumnsCommands[columnName] = new AddColumnCommand(this.TableName, column);
            return this;
        }

        public AlterTableCommand AddColumn(string columnName, bool nullable = false)
        {
            return this.AddColumn(columnName, ColumnType.VarChar(), nullable);
        }

        // modify column

        public bool HasModifyColumnCommand(string columnName)
        {
            return this.modifyColumnsCommands.ContainsKey(columnName);
        }

        public ModifyColumnCommand GetModifyColumnCommand(string columnName)
        {
            if (!this.HasModifyColumnCommand(columnName)) { throw new EasyMigException("No ModifyColumnCommand registered for " + columnName + " with " + this.TableName); }

            return this.modifyColumnsCommands[columnName];
        }


        public AlterTableCommand ModifyColumn(string columnName, ColumnType columnType, bool nullable, object defaultValue, bool unique)
        {
            if (this.HasModifyColumnCommand(columnName)) { throw new EasyMigException("ModifyColumnCommand " + this.TableName + " with " + columnName + " already registered"); }

            var column = new MigrationColumn(columnName, columnType, nullable, defaultValue, unique);
            this.modifyColumnsCommands[columnName] = new ModifyColumnCommand(this.TableName, column);
            return this;
        }

        public AlterTableCommand ModifyColumn(string columnName, ColumnType columnType, bool nullable = false)
        {
            return this.ModifyColumn(columnName, columnType, nullable, null, false);
        }


        // drop column

        public bool HasDropColumnCommand(string columnName)
        {
            return this.dropColumnsCommands.ContainsKey(columnName);
        }

        public DropColumnCommand GetDropColumnCommand(string columnName)
        {
            if (!this.HasDropColumnCommand(columnName)) { throw new EasyMigException("No DropColumnCommand registered for " + columnName + " with " + this.TableName); }

            return this.dropColumnsCommands[columnName];
        }

        public AlterTableCommand DropColumn(string columnName)
        {
            if (this.HasDropColumnCommand(columnName)) { throw new EasyMigException("DropColumnCommand " + this.TableName + " with " + columnName + " already registered"); }

            this.dropColumnsCommands[columnName] = new DropColumnCommand(this.TableName, columnName);
            return this;
        }

        // primary key

        public bool HasPrimaryKeyConstraintCommand()
        {
            return this.addPrimaryKeyConstraintCommand != null;
        }

        public AddPrimaryKeyConstraintCommand GetPrimaryKeyConstraintCommand(string columnName)
        {
            if (!this.HasPrimaryKeyConstraintCommand()) { throw new EasyMigException("No AddPrimaryKeyConstraintCommand registered for " + this.TableName); }

            return this.addPrimaryKeyConstraintCommand;
        }

        public AlterTableCommand AddPrimaryKeyConstraint(params string[] primaryKeys)
        {
            if (this.HasPrimaryKeyConstraintCommand()) { throw new EasyMigException("AddPrimaryKeyConstraintCommand for " + this.TableName + " already defined"); }

            this.addPrimaryKeyConstraintCommand = new AddPrimaryKeyConstraintCommand(this.TableName, primaryKeys);
            return this;
        }

        // foreign key

        public bool HasForeignKeyConstraintCommand(string columnName)
        {
            return this.addForeignKeyConstraintCommands.ContainsKey(columnName);
        }

        public AddForeignKeyConstraintCommand GetForeignKeyConstraintCommand(string columnName)
        {
            if (!this.HasForeignKeyConstraintCommand(columnName)) { throw new EasyMigException("No AddForeignKeyConstraintCommand registered for " + columnName + " with " + this.TableName); }

            return this.addForeignKeyConstraintCommands[columnName];
        }

        public AlterTableCommand AddForeignKeyConstraint(string columnName, ColumnType columnType, string tableReferenced, string primaryKeyReferenced)
        {
            if (this.HasForeignKeyConstraintCommand(columnName)) { throw new EasyMigException("AddForeignKeyConstraintCommand already registered for " + columnName + " with " + this.TableName); }

            var column = new ForeignKeyColumn(columnName, columnType, tableReferenced, primaryKeyReferenced);
            this.addForeignKeyConstraintCommands[columnName] = new AddForeignKeyConstraintCommand(this.TableName, column);
            return this;
        }

        public AlterTableCommand AddForeignKeyConstraint(string columnName, string tableReferenced, string primaryKeyReferenced)
        {
            return this.AddForeignKeyConstraint(columnName, ColumnType.Int(true), tableReferenced, primaryKeyReferenced);
        }

        public override string GetQuery(IQueryService queryService)
        {
            var result = new List<string>();
            if (this.HasAddColumnCommands)
            {
                foreach (var command in this.addColumnsCommands)
                {
                   result.Add(command.Value.GetQuery(queryService));
                }
            }

            if (this.HasModifyColumnCommands)
            {
                foreach (var command in this.modifyColumnsCommands)
                {
                    result.Add(command.Value.GetQuery(queryService));
                }
            }

            if (this.HasDropColumnCommands)
            {
                foreach (var command in this.dropColumnsCommands)
                {
                    result.Add(command.Value.GetQuery(queryService));
                }
            }

            if (this.HasPrimaryKeyConstraintCommand())
            {
                result.Add(addPrimaryKeyConstraintCommand.GetQuery(queryService));
            }

            if (this.HasForeignKeyConstraintCommands)
            {
                foreach (var command in this.addForeignKeyConstraintCommands)
                {
                    result.Add(command.Value.GetQuery(queryService));
                }
            }

            if (result.Count > 0)
            {
                return string.Join("\r", result);
            }
            else
            {
                return "";
            }
        }
    }
}
