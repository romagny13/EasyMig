using EasyMigLib.Commands;
using EasyMigLib.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyMigLib
{
    public class CommandContainer
    {
        protected IMigrationAssemblyService assemblyService;
        protected IDbService dbService;

        // keys => table name

        internal Dictionary<string, CreateDatabaseCommand> createDatabaseCommands;
        internal Dictionary<string, DropDatabaseCommand> dropDatabaseCommands;

        internal Dictionary<string, CreateTableCommand> createTableCommands;
        internal Dictionary<string, AlterTableCommand> alterTableCommands;
        internal Dictionary<string, DropTableCommand> dropTableCommands;

        internal Dictionary<string, SeedTableCommand> seedTableCommands;

        public bool HasCreateDatabaseCommands => this.createDatabaseCommands.Count > 0;
        public bool HasDropDatabaseCommands => this.dropDatabaseCommands.Count > 0;
        public bool HasCreateTableCommands => this.createTableCommands.Count > 0;
        public bool HasAlterTableCommands => this.alterTableCommands.Count > 0;
        public bool HasDropTableCommands => this.dropTableCommands.Count > 0;
        public bool HasSeedCommands => this.seedTableCommands.Count > 0;

        public CommandContainer()
            :this(new MigrationAssemblyService(), new DbService())
        {  }

        public CommandContainer(IMigrationAssemblyService assemblyService, IDbService dbService)
        {
            this.assemblyService = assemblyService;
            this.dbService = dbService;

            this.createDatabaseCommands = new Dictionary<string, CreateDatabaseCommand>();
            this.dropDatabaseCommands = new Dictionary<string, DropDatabaseCommand>();

            this.createTableCommands = new Dictionary<string, CreateTableCommand>();
            this.alterTableCommands = new Dictionary<string, AlterTableCommand>();
            this.dropTableCommands = new Dictionary<string, DropTableCommand>();

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


        // seed

        public bool HasSeedTable(string tableName)
        {
            return this.seedTableCommands.ContainsKey(tableName);
        }

        public SeedTableCommand GetSeedTable(string tableName)
        {
            if (!this.HasSeedTable(tableName)) { throw new Exception("No Seed Table registered for " +  tableName); }

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


        // execute

        public string GetSeedQuery(string providerName, string engine = null)
        {
            var queryService = QueryServiceFactories.GetService(providerName, engine);

            var result = new List<string>();
            foreach (var seedTableCommand in this.seedTableCommands)
            {
               result.Add(seedTableCommand.Value.GetQuery(queryService));
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
        }

        public void DoSeedAll(List<RecognizedMigrationFile> recognizedTypes,  string connectionString, string providerName, string engine = null)
        {
            try
            {
                this.dbService.CreateConnection(connectionString, providerName);
                this.dbService.Open();

                var groups = this.assemblyService.Group(recognizedTypes);

                foreach (var group in groups)
                {
                    var last = group.Value.Last();

                    this.assemblyService.RunSeeder(last);

                    var query = this.GetSeedQuery(providerName, engine);

                    this.dbService.Execute(query);
                    this.ClearSeeders();
                }

                this.dbService.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ClearSeeders();
            }
        }

        public void DoSeedForAssembly(string assemblyPath, string connectionString, string providerName, string engine = null)
        {
            var recognizedTypes = this.assemblyService.FindTypes<Seeder>(assemblyPath);
            if (recognizedTypes.Count > 0)
            {
                this.DoSeedAll(recognizedTypes, connectionString, providerName, engine);
            }
        }

        public void DoSeedForTypes(Type[] assemblyTypes, string connectionString, string providerName, string engine = null)
        {
            var recognizedTypes = this.assemblyService.FindTypes<Seeder>(assemblyTypes);
            if (recognizedTypes.Count > 0)
            {
                this.DoSeedAll(recognizedTypes, connectionString, providerName, engine);
            }
        }

        public void DoSeedFromMemory(string connectionString, string providerName, string engine = null)
        {
            this.dbService.CreateConnection(connectionString, providerName);
            this.dbService.Open();

            var query = this.GetSeedQuery(providerName, engine);

            this.dbService.Execute(query);
            this.dbService.Close();

            this.ClearSeeders();
        }

        public void DoSeedOnlyFor(RecognizedMigrationFile recognizedType,string connectionString, string providerName, string engine = null)
        {
            this.dbService.CreateConnection(connectionString, providerName);
            this.dbService.Open();

            this.assemblyService.RunSeeder(recognizedType);

            var query = this.GetSeedQuery(providerName, engine);

            this.dbService.Execute(query);
            this.ClearSeeders();

            this.dbService.Close();
        }

        public void DoSeedOnlyFor(string seederFileName, Type[] assemblyTypes, string connectionString, string providerName, string engine = null)
        {
            var recognizedType = this.assemblyService.FindType<Seeder>(assemblyTypes, seederFileName);
            if (recognizedType != null)
            {
                this.DoSeedOnlyFor(recognizedType, connectionString, providerName, engine);
            }
        }

        public void DoSeedOnlyFor(string seederFileName, string assemblyPath, string connectionString, string providerName, string engine = null)
        {
            var recognizedType = this.assemblyService.FindType<Seeder>(assemblyPath, seederFileName);
            if(recognizedType != null)
            {
                this.DoSeedOnlyFor(recognizedType, connectionString, providerName, engine);
            }
        }

        // do migrations

        public string[] GetDropTableList(Dictionary<string, CreateTableCommand>  createTableCommands)
        {
            var result = new List<string>();
            foreach (var table in createTableCommands)
            {
                if (table.Value.HasForeignKeys)
                {
                    var tableReferencedList = table.Value.GetReferencedTableList();
                    foreach (var tableReferenced in tableReferencedList)
                    {
                        if (!result.Contains(tableReferenced))
                        {
                            result.Add(tableReferenced);
                        }

                        // sort
                        var indexOfTableReferenced = result.IndexOf(tableReferenced);
                        var index = result.IndexOf(table.Key);
                        if (index == -1)
                        {
                            result.Insert(indexOfTableReferenced, table.Key);
                        }
                        else if (index > indexOfTableReferenced)
                        {
                            result.RemoveAt(index);
                            // insert before table referenced
                            result.Insert(indexOfTableReferenced, table.Key);
                        }
                    }
                }
            }

            // tables with only primary keys
            foreach (var table in createTableCommands)
            {
                if (!table.Value.HasForeignKeys && !result.Contains(table.Key))
                {
                    result.Add(table.Key);
                }
            }
            return result.ToArray();
        }

        public string GetMigrationQuery(string providerName, string engine = null)
        {
            var queryService = QueryServiceFactories.GetService(providerName, engine);

            var result = new List<string>();

            // drop database

            if (this.HasDropDatabaseCommands)
            {
                foreach (var command in this.dropDatabaseCommands)
                {
                    result.Add(command.Value.GetQuery(queryService));
                }
            }

            // create database

            if (this.HasCreateDatabaseCommands)
            {
                foreach (var command in this.createDatabaseCommands)
                {
                   result.Add(command.Value.GetQuery(queryService));
                }
            }


            if (this.HasCreateTableCommands)
            {
                // drop tables

                var dropTables = GetDropTableList(this.createTableCommands);
                foreach (var tableName in dropTables)
                {
                    result.Add(queryService.GetDropTable(tableName));
                }

                // create tables

                foreach (var command in this.createTableCommands)
                {
                    result.Add(command.Value.GetQuery(queryService));
                }

                // insert rows (insert_identitfy off)

                foreach (var command in this.createTableCommands)
                {
                    if (command.Value.HasSeeds)
                    {
                        result.Add(queryService.GetSeeds(command.Value));
                    }
                }

                // primary keys

                foreach (var command in this.createTableCommands)
                {
                    if (command.Value.HasPrimaryKeys)
                    {
                        result.Add(queryService.GetAddPrimaryKeyConstraint(command.Value));
                    }
                }

                // foreign keys

                foreach (var command in this.createTableCommands)
                {
                    if (command.Value.HasForeignKeys)
                    {
                        result.Add(queryService.GetAddForeignKeyConstraints(command.Value));
                    }
                }
            }

            // alter tables

            if (this.HasDropTableCommands)
            {
                foreach (var command in this.dropTableCommands)
                {
                    result.Add(command.Value.GetQuery(queryService));
                }
            }

            if (this.HasAlterTableCommands)
            {
                foreach (var command in this.alterTableCommands)
                {
                    result.Add(command.Value.GetQuery(queryService));
                }
            }

            return string.Join("\r", result);
        }

        public void UpdateDatabase(string connectionString, string providerName, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            this.dbService.CreateConnection(connectionString, providerName);
            this.dbService.Open();

            var query = this.GetMigrationQuery(providerName, engine);

            this.dbService.Execute(query);
            this.ClearMigrations();

            this.dbService.Close();
        }

        public void DoMigrations(List<RecognizedMigrationFile> recognizedTypes, string connectionString, string providerName, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            try
            {
                this.dbService.CreateConnection(connectionString, providerName);
                this.dbService.Open();

                var groups = this.assemblyService.Group(recognizedTypes);

                foreach (var group in groups)
                {
                    var last = group.Value.Last();

                    this.assemblyService.RunMigration(last, direction);

                    var query = this.GetMigrationQuery(providerName, engine);

                    this.dbService.Execute(query);
                    this.ClearMigrations();
                }

                this.dbService.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.ClearMigrations();
            }
        }

        public void DoMigrationsForTypes(Type[] assemblyTypes, string connectionString, string providerName, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedTypes = this.assemblyService.FindTypes<Migration>(assemblyTypes);
            if (recognizedTypes.Count > 0)
            {
                this.DoMigrations(recognizedTypes, connectionString, providerName, engine, direction);
            }
        }

        public void DoMigrationsForAssembly(string assemblyPath, string connectionString, string providerName, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedTypes = this.assemblyService.FindTypes<Migration>(assemblyPath);
            if (recognizedTypes.Count > 0)
            {
                this.DoMigrations(recognizedTypes, connectionString, providerName, engine, direction);
            }
        }

        public void RunMigrationAndUpdateDatabase(RecognizedMigrationFile recognizedType, string connectionString, string providerName, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            this.assemblyService.RunMigration(recognizedType, direction);

            this.UpdateDatabase(connectionString, providerName, engine, direction);
        }

        public void DoMigrationOnlyFor(string migrationFileName, Type[] assemblyTypes, string connectionString, string providerName, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedType = this.assemblyService.FindType<Migration>(assemblyTypes, migrationFileName);
            if (recognizedType != null)
            {
                this.RunMigrationAndUpdateDatabase(recognizedType, connectionString, providerName, engine, direction);
            }
        }

        public void DoMigrationOnlyFor(string migrationFileName, string assemblyPath, string connectionString, string providerName, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedType = this.assemblyService.FindType<Migration>(assemblyPath, migrationFileName);
            if (recognizedType != null)
            {
                this.RunMigrationAndUpdateDatabase(recognizedType, connectionString, providerName, engine, direction);
            }
        }

        // execute query

        public int ExecuteQuery(string query, string connectionString, string providerName)
        {
            int rowsAffected = 0;
            dbService.CreateConnection(connectionString, providerName);
            dbService.Open();
            rowsAffected = dbService.Execute(query);
            dbService.Close();
            return rowsAffected;
        }

        // scripts

        public void CreateMigrationsScript(string assemblyPath, string providerName, string fileName, string engine = null)
        {
            try
            {
                var recognizedTypes = this.assemblyService.FindTypes<Migration>(assemblyPath);
                if (recognizedTypes.Count == 0) { throw new Exception("No Migration found"); }

                var groups = this.assemblyService.Group(recognizedTypes);

                foreach (var group in groups)
                {
                    var last = group.Value.Last();

                    this.assemblyService.RunMigration(last);
                }

                var query = GetMigrationQuery(providerName, engine);

                File.WriteAllText(fileName, query);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ClearMigrations();
            }
          
        }

        public void CreateSeedScript(string assemblyPath, string providerName, string fileName, string engine = null)
        {
            try
            {
                var recognizedTypes = this.assemblyService.FindTypes<Seeder>(assemblyPath);
                if (recognizedTypes.Count == 0) { throw new Exception("No Seeder found"); }

                var groups = this.assemblyService.Group(recognizedTypes);

                foreach (var group in groups)
                {
                    var last = group.Value.Last();

                    this.assemblyService.RunSeeder(last);
                }

                var query = GetSeedQuery(providerName, engine);

                File.WriteAllText(fileName, query);

                this.ClearSeeders();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.ClearMigrations();
            }

        }
    }

}
