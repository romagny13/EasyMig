using EasyMigLib.Commands;
using System.Collections.Generic;
using EasyMigLib.MigrationReflection;
using EasyMigLib.Db.Common;
using EasyMigLib.Query;
using System;
using System.Linq;
using System.IO;
using EasyMigLib.Query.SqlClient;

namespace EasyMigLib.SqlClientAttachedDbFile
{
    public class SqlFileSchema
    {
        protected IMigrationAssemblyService assemblyService;
        protected IQueryService queryService;
        protected IDbService dbService;

        protected CommandContainer container;

        public string ProviderName { get; }

        public SqlFileSchema(CommandContainer container)
            : this(new MigrationAssemblyService(), new SqlQueryService(), new DbService(), container)
        { }

        public SqlFileSchema(
            IMigrationAssemblyService assemblyService,
            IQueryService queryService,
            IDbService dbService,
            CommandContainer container)
        {
            this.assemblyService = assemblyService;
            this.queryService = queryService;
            this.dbService = dbService;

            this.container = container;

            this.ProviderName = "System.Data.SqlClient";
        }

        // query

        public string GetSeedQuery()
        {
            var result = new List<string>();
            foreach (var seedTableCommand in container.seedTableCommands)
            {
                result.Add(seedTableCommand.Value.GetQuery(this.queryService));
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

        public string[] GetDropTableList(Dictionary<string, CreateTableCommand> createTableCommands)
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

        public string GetMigrationQuery()
        {
            var result = new List<string>();

            if (container.HasCreateTableCommands)
            {
                // drop tables

                var dropTables = this.GetDropTableList(container.createTableCommands);
                foreach (var tableName in dropTables)
                {
                    result.Add(queryService.GetDropTable(tableName));
                }

                // create tables

                foreach (var command in container.createTableCommands)
                {
                    result.Add(command.Value.GetQuery(queryService));
                }

                // insert rows (insert_identitfy off)

                foreach (var command in container.createTableCommands)
                {
                    if (command.Value.HasSeeds)
                    {
                        result.Add(queryService.GetSeeds(command.Value));
                    }
                }

                // primary keys

                foreach (var command in container.createTableCommands)
                {
                    if (command.Value.HasPrimaryKeys)
                    {
                        result.Add(queryService.GetAddPrimaryKeyConstraint(command.Value));
                    }
                }

                // foreign keys

                foreach (var command in container.createTableCommands)
                {
                    if (command.Value.HasForeignKeys)
                    {
                        result.Add(queryService.GetAddForeignKeyConstraints(command.Value));
                    }
                }
            }

            // alter tables

            if (container.HasDropTableCommands)
            {
                foreach (var command in container.dropTableCommands)
                {
                    result.Add(command.Value.GetQuery(queryService));
                }
            }

            if (container.HasAlterTableCommands)
            {
                foreach (var command in container.alterTableCommands)
                {
                    result.Add(command.Value.GetQuery(queryService));
                }
            }

            return string.Join("\r", result);
        }

        public string GetStoredProcedureQuery()
        {

            if (container.HasCreateStoredProcedureCommands)
            {
                var result = new List<string>();

                foreach (var command in container.createStoredProcedureCommands)
                {
                    // drop if exists
                    result.Add(this.queryService.GetDropStoredProcedure(command.Value.ProcedureName, false) + "\rGO");
                    // create
                    result.Add(command.Value.GetQuery(queryService) + "\rGO");
                }

                return string.Join("\r", result);
            }
            return "";
        }

        // seed all

        public void DoSeedAll(List<RecognizedMigrationFile> recognizedTypes, string connectionString)
        {
            try
            {
                this.dbService.CreateConnection(connectionString, this.ProviderName);
                this.dbService.Open();

                var groups = this.assemblyService.Group(recognizedTypes);

                foreach (var group in groups)
                {
                    var last = group.Value.Last();

                    this.assemblyService.RunSeeder(last);

                    var query = this.GetSeedQuery();
                    this.dbService.Execute(query);

                    container.ClearSeeders();
                }

                this.dbService.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                container.ClearSeeders();
            }
        }

        public void DoSeedForAssembly(string assemblyPath, string connectionString)
        {
            var recognizedTypes = this.assemblyService.FindTypes<Seeder>(assemblyPath);
            if (recognizedTypes.Count > 0)
            {
                this.DoSeedAll(recognizedTypes, connectionString);
            }
        }

        public void DoSeedForTypes(Type[] assemblyTypes, string connectionString)
        {
            var recognizedTypes = this.assemblyService.FindTypes<Seeder>(assemblyTypes);
            if (recognizedTypes.Count > 0)
            {
                this.DoSeedAll(recognizedTypes, connectionString);
            }
        }

        // seed one

        public void DoSeedOne(RecognizedMigrationFile recognizedType, string connectionString)
        {
            try
            {
                this.dbService.CreateConnection(connectionString, this.ProviderName);
                this.dbService.Open();

                this.assemblyService.RunSeeder(recognizedType);

                var query = this.GetSeedQuery();
                this.dbService.Execute(query);

                container.ClearSeeders();

                this.dbService.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                container.ClearSeeders();
            }
        }

        public void DoSeedOnlyFor(string seederFileName, Type[] assemblyTypes, string connectionString)
        {
            var recognizedType = this.assemblyService.FindType<Seeder>(assemblyTypes, seederFileName);
            if (recognizedType != null)
            {
                this.DoSeedOne(recognizedType, connectionString);
            }
        }

        public void DoSeedOnlyFor(string seederFileName, string assemblyPath, string connectionString)
        {
            var recognizedType = this.assemblyService.FindType<Seeder>(assemblyPath, seederFileName);
            if (recognizedType != null)
            {
                this.DoSeedOne(recognizedType, connectionString);
            }
        }

        // seed from memory

        public void DoSeedFromMemory(string connectionString)
        {

            try
            {
                // from memory , no group
                this.dbService.CreateConnection(connectionString, this.ProviderName);
                this.dbService.Open();

                var query = this.GetSeedQuery();

                this.dbService.Execute(query);
                this.dbService.Close();

                container.ClearSeeders();
            }
            catch (Exception)
            {
                throw;
            }
        }

        // migrations

        public void DoMigrations(List<RecognizedMigrationFile> recognizedTypes, string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            try
            {
                this.dbService.CreateConnection(connectionString, this.ProviderName);
                this.dbService.Open();

                var groups = this.assemblyService.Group(recognizedTypes);

                foreach (var group in groups)
                {
                    var last = group.Value.Last();

                    this.assemblyService.RunMigration(last, direction);

                    var query = this.GetMigrationQuery();
                    this.dbService.Execute(query);

                    this.DropProcedures(connectionString);
                    this.CreateProcedures(connectionString);

                    container.ClearMigrations();
                }

                this.dbService.Close();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                container.ClearMigrations();
            }
        }

        public void DoMigrationsForTypes(Type[] assemblyTypes, string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedTypes = this.assemblyService.FindTypes<Migration>(assemblyTypes);
            if (recognizedTypes.Count > 0)
            {
                this.DoMigrations(recognizedTypes, connectionString, direction);
            }
        }

        public void DoMigrationsForAssembly(string assemblyPath, string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedTypes = this.assemblyService.FindTypes<Migration>(assemblyPath);
            if (recognizedTypes.Count > 0)
            {
                this.DoMigrations(recognizedTypes, connectionString, direction);
            }
        }

        public void DropProcedures(string connectionString)
        {
            if (container.HasDropStoredProcedureCommands)
            {
                this.dbService.CreateConnection(connectionString, this.ProviderName);
                this.dbService.Open();

                foreach (var command in container.dropStoredProcedureCommands)
                {
                    var query = command.Value.GetQuery(queryService);
                    this.dbService.Execute(query);
                }

                this.dbService.Close();
            }
        }

        public void CreateProcedures(string connectionString)
        {

            if (container.HasCreateStoredProcedureCommands)
            {
                this.dbService.CreateConnection(connectionString, this.ProviderName);
                this.dbService.Open();

                foreach (var command in container.createStoredProcedureCommands)
                {
                    // drop if exists
                    var dropQuery = this.queryService.GetDropStoredProcedure(command.Value.ProcedureName);
                    this.dbService.Execute(dropQuery);

                    // create
                    var query = command.Value.GetQuery(queryService);
                    this.dbService.Execute(query);
                }

                this.dbService.Close();
            }
        }

        public void DoMigrationsFromMemory(string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            try
            {
                var query = this.GetMigrationQuery();
                if (!string.IsNullOrEmpty(query))
                {
                    this.dbService.CreateConnection(connectionString, this.ProviderName);

                    this.dbService.Open();
                    this.dbService.Execute(query);

                    this.dbService.Close();
                }
                this.DropProcedures(connectionString);
                this.CreateProcedures(connectionString);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                container.ClearMigrations();
            }
        }

        // migration one

        public void RunMigrationAndUpdateDatabase(RecognizedMigrationFile recognizedType, string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            this.assemblyService.RunMigration(recognizedType, direction);
            this.DoMigrationsFromMemory(connectionString, direction);
        }

        public void DoMigrationOnlyFor(string migrationFileName, Type[] assemblyTypes, string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedType = this.assemblyService.FindType<Migration>(assemblyTypes, migrationFileName);
            if (recognizedType != null)
            {
                this.RunMigrationAndUpdateDatabase(recognizedType, connectionString, direction);
            }
        }

        public void DoMigrationOnlyFor(string migrationFileName, string assemblyPath, string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedType = this.assemblyService.FindType<Migration>(assemblyPath, migrationFileName);
            if (recognizedType != null)
            {
                this.RunMigrationAndUpdateDatabase(recognizedType, connectionString, direction);
            }
        }

        // execute query

        public int ExecuteQuery(string query, string connectionString)
        {
            int rowsAffected = 0;
            dbService.CreateConnection(connectionString, this.ProviderName);
            dbService.Open();
            rowsAffected = dbService.Execute(query);
            dbService.Close();
            return rowsAffected;
        }

        // scripts

        public void CreateMigrationsScript(string assemblyPath, string fileName)
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

                var query = GetMigrationQuery();

                File.WriteAllText(fileName, query);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                container.ClearMigrations();
            }

        }

        public void CreateStoredProcedureScript(string assemblyPath, string fileName, string engine = null)
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

                var query = GetStoredProcedureQuery();

                File.WriteAllText(fileName, query);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                container.ClearMigrations();
            }

        }

        public void CreateSeedScript(string assemblyPath, string fileName)
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

                var query = GetSeedQuery();

                File.WriteAllText(fileName, query);

                container.ClearSeeders();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                container.ClearMigrations();
            }

        }

    }
}