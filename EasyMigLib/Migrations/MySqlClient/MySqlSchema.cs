using EasyMigLib.Commands;
using EasyMigLib.Db;
using EasyMigLib.Db.MySqlClient;
using EasyMigLib.MigrationReflection;
using EasyMigLib.Query;
using EasyMigLib.Query.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyMigLib.Migrations.MySqlClient
{

    public class MySqlSchema
    {
        protected IMigrationAssemblyService assemblyService;
        protected IQueryService queryService;
        protected IMySqlDbService dbService;
        protected IQuerySplitter splitter;

        protected CommandContainer container;

        public MySqlSchema(CommandContainer container)
            : this(new MigrationAssemblyService(),
                  new MySqlQueryService(),
                  new MySqlDbService(),
                  new MySqlQuerySplitter(),
                  container)
        { }

        public MySqlSchema(
            IMigrationAssemblyService assemblyService,
            IQueryService queryService,
            IMySqlDbService dbService,
            IQuerySplitter splitter,
            CommandContainer container)
        {
            this.assemblyService = assemblyService;
            this.queryService = queryService;
            this.dbService = dbService;
            this.splitter = splitter;

            this.container = container;
        }

        public Dictionary<string, List<RecognizedMigrationFile>> Group(List<RecognizedMigrationFile> recognizedTypes)
        {
            return this.assemblyService.Group(recognizedTypes);
        }

        public RecognizedMigrationFile GetLast(List<RecognizedMigrationFile> recognizedTypes)
        {
            return recognizedTypes.Last();
        }

        public void RunMigration(RecognizedMigrationFile last, MigrationDirection direction = MigrationDirection.Up)
        {
            this.assemblyService.RunMigration(last, direction);
        }

        public void RunSeeder(RecognizedMigrationFile last)
        {
            this.assemblyService.RunSeeder(last);
        }

        public List<RecognizedMigrationFile> FindTypes<T>(Type[] assemblyTypes)
        {
            return this.assemblyService.FindTypes<T>(assemblyTypes);
        }

        public List<RecognizedMigrationFile> FindTypes<T>(string assemblyPath)
        {
            return this.assemblyService.FindTypes<T>(assemblyPath);
        }

        public RecognizedMigrationFile FindType<T>(Type[] assemblyTypes, string matchName)
        {
            return this.assemblyService.FindType<T>(assemblyTypes, matchName);
        }

        public RecognizedMigrationFile FindType<T>(string assemblyPath, string matchName)
        {
            return this.assemblyService.FindType<T>(assemblyPath, matchName);
        }

        public string GetCreateStoredProceduresQuery()
        {
            var result = "DELIMITER $$\r\r";
            foreach (var command in container.createStoredProcedureCommands)
            {
                // drop if exists
                result += this.queryService.GetDropStoredProcedure(command.Value.ProcedureName, false) + "$$";
                // create
                result += command.Value.GetQuery(queryService) + "$$\r\r";
            }
            result += "DELIMITER ;\r";
            return result;
        }

        // query

        public string GetSeedQuery()
        {
            return container.GetSeedQuery(this.queryService);
        }

        public void TrySetEngine(string engine)
        {
            if (!string.IsNullOrEmpty(engine))
            {
                (this.queryService as MySqlQueryService).SetEngine(engine);
            }
        }

        public virtual string GetMigrationQuery(string engine)
        {
            var result = new List<string>();

            this.TrySetEngine(engine);

            // drop database

            if (container.HasDropDatabaseCommands)
            {
                result.Add(container.GetDropDatabasesQuery(this.queryService));
            }

            // create database

            if (container.HasCreateDatabaseCommands)
            {
                result.Add(container.GetCreateDatabasesQuery(this.queryService));
            }

            // create and use database

            if (container.HasCreateAndUseDatabaseCommands)
            {
                result.Add(container.GetCreateAndUseDatabasesQuery(this.queryService));
            }

            if (container.HasCreateTableCommands)
            {
                result.Add(container.GetCreateTablesQuery(this.queryService));
            }

            // alter tables

            if (container.HasDropTableCommands)
            {
                result.Add(container.GetDropTablesQuery(this.queryService));
            }

            if (container.HasAlterTableCommands)
            {
                result.Add(container.GetAlterTablesQuery(this.queryService));
            }

            // drop stored procedure

            if (container.HasDropStoredProcedureCommands)
            {
                result.Add(container.GetDropStoredProceduresQuery(this.queryService));
            }

            // create stored procedure

            if (container.HasCreateStoredProcedureCommands)
            {
                result.Add(this.GetCreateStoredProceduresQuery());
            }

            return string.Join("\r", result);
        }

        // seed all

        public string[] SplitQuery(string query)
        {
            return this.splitter.SplitQuery(query);
        }

        public bool IsValidQuery(string query)
        {
            return !string.IsNullOrWhiteSpace(query);
        }

        public void ExecuteQueries(string[] queries)
        {
            foreach (var query in queries)
            {
                if (this.IsValidQuery(query))
                {
                    this.dbService.Execute(query);
                }
            }
        }

        public void DoSeedAll(List<RecognizedMigrationFile> recognizedTypes, string connectionString)
        {
            try
            {
                this.dbService.CreateConnection(connectionString);
                this.dbService.Open();

                var groups = this.Group(recognizedTypes);

                foreach (var group in groups)
                {
                    var last = this.GetLast(group.Value);

                    this.RunSeeder(last);

                    var query = this.GetSeedQuery();
                    if (this.IsValidQuery(query))
                    {
                        var queries = this.SplitQuery(query);
                        this.ExecuteQueries(queries);
                    }
                    container.ClearSeeders();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                this.dbService.Close();
                container.ClearSeeders();
            }
        }

        public void DoSeedForAssembly(string assemblyPath, string connectionString)
        {
            var recognizedTypes = this.FindTypes<Seeder>(assemblyPath);
            if (recognizedTypes.Count > 0)
            {
                this.DoSeedAll(recognizedTypes, connectionString);
            }
        }

        public void DoSeedForTypes(Type[] assemblyTypes, string connectionString)
        {
            var recognizedTypes = this.FindTypes<Seeder>(assemblyTypes);
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
                this.dbService.CreateConnection(connectionString);
                this.dbService.Open();

                this.RunSeeder(recognizedType);

                var query = this.GetSeedQuery();
                var queries = this.SplitQuery(query);
                this.ExecuteQueries(queries);

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
            var recognizedType = this.FindType<Seeder>(assemblyTypes, seederFileName);
            if (recognizedType != null)
            {
                this.DoSeedOne(recognizedType, connectionString);
            }
        }

        public void DoSeedOnlyFor(string seederFileName, string assemblyPath, string connectionString)
        {
            var recognizedType = this.FindType<Seeder>(assemblyPath, seederFileName);
            if (recognizedType != null)
            {
                this.DoSeedOne(recognizedType, connectionString);
            }
        }

        // seed from memory

        public void DoSeedFromMemory(string connectionString)
        {
            this.dbService.CreateConnection(connectionString);
            this.dbService.Open();

            var query = this.GetSeedQuery();
            if (this.IsValidQuery(query))
            {
                var queries = this.SplitQuery(query);
                this.ExecuteQueries(queries);
            }

            this.dbService.Close();

            container.ClearSeeders();
        }

        // do migrations

        public void DoMigrations(List<RecognizedMigrationFile> recognizedTypes, string connectionString, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            try
            {
                this.dbService.CreateConnection(connectionString);
                this.dbService.Open();

                var groups = this.Group(recognizedTypes);

                foreach (var group in groups)
                {
                    var last = this.GetLast(group.Value);

                    this.RunMigration(last, direction);

                    var query = this.GetMigrationQuery(engine);
                    if (this.IsValidQuery(query))
                    {
                        var queries = this.SplitQuery(query);
                        this.ExecuteQueries(queries);
                    }

                    container.ClearMigrations();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.dbService.Close();
                container.ClearMigrations();
            }
        }

        public void DoMigrationsForTypes(Type[] assemblyTypes, string connectionString, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedTypes = this.FindTypes<Migration>(assemblyTypes);
            if (recognizedTypes.Count > 0)
            {
                this.DoMigrations(recognizedTypes, connectionString, engine, direction);
            }
        }

        public void DoMigrationsForAssembly(string assemblyPath, string connectionString, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedTypes = this.FindTypes<Migration>(assemblyPath);
            if (recognizedTypes.Count > 0)
            {
                this.DoMigrations(recognizedTypes, connectionString, engine, direction);
            }
        }

        public void DoMigrationsFromMemory(string connectionString, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var query = this.GetMigrationQuery(engine);
            if (this.IsValidQuery(query))
            {

                this.dbService.CreateConnection(connectionString);
                this.dbService.Open();

                var queries = this.SplitQuery(query);
                this.ExecuteQueries(queries);

                container.ClearMigrations();

                this.dbService.Close();
            }
        }

        // migration one

        public void RunMigrationAndUpdateDatabase(RecognizedMigrationFile recognizedType, string connectionString, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            this.RunMigration(recognizedType, direction);
            this.DoMigrationsFromMemory(connectionString, engine, direction);
        }

        public void DoMigrationOnlyFor(string migrationFileName, Type[] assemblyTypes, string connectionString, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedType = this.FindType<Migration>(assemblyTypes, migrationFileName);
            if (recognizedType != null)
            {
                this.RunMigrationAndUpdateDatabase(recognizedType, connectionString, engine, direction);
            }
        }

        public void DoMigrationOnlyFor(string migrationFileName, string assemblyPath, string connectionString, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedType = this.FindType<Migration>(assemblyPath, migrationFileName);
            if (recognizedType != null)
            {
                this.RunMigrationAndUpdateDatabase(recognizedType, connectionString, engine, direction);
            }
        }

        // execute query

        public int ExecuteQuery(string query, string connectionString)
        {
            int rowsAffected = 0;
            if (this.IsValidQuery(query))
            {
                dbService.CreateConnection(connectionString);
                dbService.Open();
                rowsAffected = dbService.Execute(query);
                dbService.Close();
            }
            return rowsAffected;
        }

        // scripts

        public void CreateScript(string fileName, string content)
        {
            File.WriteAllText(fileName, content);
        }

        public void CreateMigrationsScript(string assemblyPath, string fileName, string engine = null)
        {
            try
            {
                var recognizedTypes = this.FindTypes<Migration>(assemblyPath);
                if (recognizedTypes.Count == 0) { throw new EasyMigException("No Migration found"); }

                var groups = this.Group(recognizedTypes);
                foreach (var group in groups)
                {
                    var last = this.GetLast(group.Value);
                    this.RunMigration(last);
                }

                var query = GetMigrationQuery(engine);
                this.CreateScript(fileName, query);
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

        public void CreateSeedScript(string assemblyPath, string fileName)
        {
            try
            {
                var recognizedTypes = this.FindTypes<Seeder>(assemblyPath);
                if (recognizedTypes.Count == 0) { throw new EasyMigException("No Seeder found"); }

                var groups = this.Group(recognizedTypes);
                foreach (var group in groups)
                {
                    var last = this.GetLast(group.Value);
                    this.RunSeeder(last);
                }

                var query = GetSeedQuery();
                this.CreateScript(fileName, query);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                container.ClearSeeders();
            }

        }

    }
}
