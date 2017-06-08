using System;
using System.Collections.Generic;

namespace EasyMigLib.Commands
{
    public class CommandContainer
    {
        internal Dictionary<string, CreateDatabaseCommand> createDatabaseCommands;
        internal Dictionary<string, DropDatabaseCommand> dropDatabaseCommands;

        internal Dictionary<string, CreateTableCommand> createTableCommands;
        internal Dictionary<string, AlterTableCommand> alterTableCommands;
        internal Dictionary<string, DropTableCommand> dropTableCommands;

        internal Dictionary<string, CreateStoredProcedureCommand> createStoredProcedureCommands;
        internal Dictionary<string, DropStoredProcedureCommand> dropStoredProcedureCommands;

        internal Dictionary<string, SeedTableCommand> seedTableCommands;

        public bool HasCreateDatabaseCommands => this.createDatabaseCommands.Count > 0;
        public bool HasDropDatabaseCommands => this.dropDatabaseCommands.Count > 0;
        public bool HasCreateTableCommands => this.createTableCommands.Count > 0;
        public bool HasAlterTableCommands => this.alterTableCommands.Count > 0;
        public bool HasDropTableCommands => this.dropTableCommands.Count > 0;
        public bool HasCreateStoredProcedureCommands => this.createStoredProcedureCommands.Count > 0;
        public bool HasDropStoredProcedureCommands => this.dropStoredProcedureCommands.Count > 0;
        public bool HasSeedCommands => this.seedTableCommands.Count > 0;

        public CommandContainer()
        {

            this.createDatabaseCommands = new Dictionary<string, CreateDatabaseCommand>();
            this.dropDatabaseCommands = new Dictionary<string, DropDatabaseCommand>();

            this.createTableCommands = new Dictionary<string, CreateTableCommand>();
            this.alterTableCommands = new Dictionary<string, AlterTableCommand>();
            this.dropTableCommands = new Dictionary<string, DropTableCommand>();

            this.createStoredProcedureCommands = new Dictionary<string, CreateStoredProcedureCommand>();
            this.dropStoredProcedureCommands = new Dictionary<string, DropStoredProcedureCommand>();

            this.seedTableCommands = new Dictionary<string, SeedTableCommand>();
        }

        // create database 

        public bool HasCreateDatabaseCommand(string databaseName)
        {
            return this.createDatabaseCommands.ContainsKey(databaseName);
        }

        public CreateDatabaseCommand GetCreateDatabaseCommand(string databaseName)
        {
            if (!this.HasCreateDatabaseCommand(databaseName)) { throw new Exception("No " + databaseName + " registered"); }

            return this.createDatabaseCommands[databaseName];
        }

        public CreateDatabaseCommand CreateDatabase(string databaseName)
        {
            if (this.HasCreateDatabaseCommand(databaseName)) { throw new Exception(databaseName + " already registered"); }

            var command = new CreateDatabaseCommand(databaseName);
            this.createDatabaseCommands[databaseName] = command;
            return command;
        }

        // drop database

        public bool HasDropDatabaseCommand(string databaseName)
        {
            return this.dropDatabaseCommands.ContainsKey(databaseName);
        }

        public DropDatabaseCommand GetDropDatabaseCommand(string databaseName)
        {
            if (!this.HasDropDatabaseCommand(databaseName)) { throw new Exception("No DropDatabaseCommand " + databaseName + " registered"); }

            return this.dropDatabaseCommands[databaseName];
        }

        public DropDatabaseCommand DropDatabase(string databaseName)
        {
            if (this.HasDropDatabaseCommand(databaseName)) { throw new Exception("DropDatabaseCommand " + databaseName + " already registered"); }

            var command = new DropDatabaseCommand(databaseName);
            this.dropDatabaseCommands[databaseName] = command;
            return command;
        }

        // create table

        public bool HasCreateTableCommand(string tableName)
        {
            return this.createTableCommands.ContainsKey(tableName);
        }

        public CreateTableCommand GetCreateTableCommand(string tableName)
        {
            if (!this.HasCreateTableCommand(tableName)) { throw new Exception("No CreateTableCommand " + tableName + " registered"); }

            return this.createTableCommands[tableName];
        }

        public CreateTableCommand CreateTable(string tableName)
        {
            if (this.HasCreateTableCommand(tableName)) { throw new Exception("CreateTableCommand " + tableName + " already registered"); }

            var command = new CreateTableCommand(tableName);
            this.createTableCommands[tableName] = command;
            return command;
        }

        // alter table

        public bool HasAlterTableCommand(string tableName)
        {
            return this.alterTableCommands.ContainsKey(tableName);
        }

        public AlterTableCommand GetAlterTableCommand(string tableName)
        {
            if (!this.HasAlterTableCommand(tableName)) { throw new Exception("No AlterTableCommand " + tableName + " registered"); }

            return this.alterTableCommands[tableName];
        }

        public AlterTableCommand AlterTable(string tableName)
        {
            if (this.HasAlterTableCommand(tableName))
            {
                return this.GetAlterTableCommand(tableName);
            }
            else
            {
                var command = new AlterTableCommand(tableName);
                this.alterTableCommands[tableName] = command;
                return command;
            }
        }


        // drop table 

        public bool HasDropTableCommand(string tableName)
        {
            return this.dropTableCommands.ContainsKey(tableName);
        }

        public DropTableCommand GetDropTableCommand(string tableName)
        {
            if (!this.HasDropTableCommand(tableName)) { throw new Exception("No DropTableCommand " + tableName + " registered"); }

            return this.dropTableCommands[tableName];
        }

        public DropTableCommand DropTable(string tableName)
        {
            if (this.HasDropTableCommand(tableName)) { throw new Exception("DropTableCommand " + tableName + " already registered"); }

            var command = new DropTableCommand(tableName);
            this.dropTableCommands[tableName] = command;
            return command;
        }

        // create stored procedure

        public bool HasCreateStoredProcedureCommand(string procedureName)
        {
            return this.createStoredProcedureCommands.ContainsKey(procedureName);
        }

        public CreateStoredProcedureCommand GetCreateStoredProcedureCommand(string procedureName)
        {
            if (!this.HasCreateStoredProcedureCommand(procedureName)) { throw new Exception("No CreateProcedureCommand " + procedureName + " registered"); }

            return this.createStoredProcedureCommands[procedureName];
        }

        public CreateStoredProcedureCommand CreateStoredProcedure(string procedureName)
        {
            if (this.HasCreateStoredProcedureCommand(procedureName)) { throw new Exception("CreateProcedureCommand " + procedureName + " already registered"); }

            var command = new CreateStoredProcedureCommand(procedureName);
            this.createStoredProcedureCommands[procedureName] = command;
            return command;
        }

        // drop stored procedure

        public bool HasDropStoredProcedureCommand(string procedureName)
        {
            return this.dropStoredProcedureCommands.ContainsKey(procedureName);
        }

        public DropStoredProcedureCommand GetDropStoredProcedureCommand(string procedureName)
        {
            if (!this.HasDropStoredProcedureCommand(procedureName)) { throw new Exception("No DropProcedureCommand " + procedureName + " registered"); }

            return this.dropStoredProcedureCommands[procedureName];
        }

        public DropStoredProcedureCommand DropStoredProcedure(string procedureName)
        {
            if (this.HasDropStoredProcedureCommand(procedureName)) { throw new Exception("DropProcedureCommand " + procedureName + " already registered"); }

            var command = new DropStoredProcedureCommand(procedureName);
            this.dropStoredProcedureCommands[procedureName] = command;
            return command;
        }

        // seed

        public bool HasSeedTable(string tableName)
        {
            return this.seedTableCommands.ContainsKey(tableName);
        }

        public SeedTableCommand GetSeedTable(string tableName)
        {
            if (!this.HasSeedTable(tableName)) { throw new Exception("No Seed Table registered for " + tableName); }

            return this.seedTableCommands[tableName];
        }

        public SeedTableCommand SeedTable(string tableName)
        {
            if (this.HasSeedTable(tableName))
            {
                return this.GetSeedTable(tableName);
            }
            else
            {
                var seedTable = new SeedTableCommand(tableName);
                this.seedTableCommands[tableName] = seedTable;
                return seedTable;
            }
        }

        // clear

        public void ClearSeeders()
        {
            this.seedTableCommands.Clear();
        }

        public void ClearMigrations()
        {
            this.createDatabaseCommands.Clear();
            this.dropDatabaseCommands.Clear();

            this.createTableCommands.Clear();
            this.alterTableCommands.Clear();
            this.dropTableCommands.Clear();

            this.createStoredProcedureCommands.Clear();
            this.dropStoredProcedureCommands.Clear();
        }

    }
}
