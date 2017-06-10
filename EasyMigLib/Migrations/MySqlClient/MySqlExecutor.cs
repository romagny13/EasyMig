using EasyMigLib.Db.MySqlClient;
using EasyMigLib.MigrationReflection;
using EasyMigLib.Query;
using EasyMigLib.Query.MySqlClient;
using EasyMigLib.Schema;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EasyMigLib.Migrations.MySqlClient
{
    public class MySqlExecutor
    {
        protected IMigrationAssemblyService assemblyService;
        protected IQueryService queryService;
        protected IMySqlDbService dbService;

        protected DatabaseSchema schema;

        public MySqlExecutor(DatabaseSchema schema)
            : this(new MigrationAssemblyService(),
                  new MySqlQueryService(),
                  new MySqlDbService(),
                  schema)
        { }

        public MySqlExecutor(
            IMigrationAssemblyService assemblyService,
            IQueryService queryService,
            IMySqlDbService dbService,
            DatabaseSchema schema)
        {
            this.assemblyService = assemblyService;
            this.queryService = queryService;
            this.dbService = dbService;

            this.schema = schema;
        }

        // queries

        public void TrySetEngine(string engine)
        {
            if (!string.IsNullOrEmpty(engine))
            {
                (this.queryService as MySqlQueryService).SetEngine(engine);
            }
        }

        public List<string> GetDropTableList()
        {
            var result = new List<string>();
            foreach (var table in this.schema.tablesToCreate)
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
            foreach (var table in this.schema.tablesToCreate)
            {
                if (!table.Value.HasForeignKeys && !result.Contains(table.Key))
                {
                    result.Add(table.Key);
                }
            }
            return result;
        }

        public List<string> GetDropDatabasesQueries()
        {
            var result = new List<string>();
            foreach (var databaseName in this.schema.databasesToDrop)
            {
                result.Add(this.queryService.GetDropDatabase(databaseName));
            }
            return result;
        }

        public List<string> GetCreateDatabasesQueries()
        {
            var result = new List<string>();
            foreach (var databaseName in this.schema.databasesToCreate)
            {
                result.Add(this.queryService.GetCreateDatabase(databaseName));
            }
            return result;
        }

        public List<string> GetCreateAndUseDatabasesQueries()
        {
            var result = new List<string>();
            foreach (var databaseName in this.schema.databasesToCreateAndUse)
            {
                result.Add(this.queryService.GetCreateDatabase(databaseName));
                result.Add(this.queryService.GetUseDatabase(databaseName));
            }
            return result;
        }

        public List<string> GetCreateTablesQueries(bool script = false)
        {
            var result = new List<string>();

            // drop tables

            var dropTables = this.GetDropTableList();
            foreach (var tableName in dropTables)
            {
                result.Add(queryService.GetDropTable(tableName));
            }

            // create tables

            foreach (var table in this.schema.tablesToCreate)
            {


                result.Add(this.queryService.GetCreateTable(table.Value));
            }

            // insert rows

            foreach (var table in this.schema.tablesToCreate)
            {
                if (table.Value.HasSeeds)
                {
                    if (script)
                    {
                        result.Add(this.queryService.GetSeeds(table.Value));
                    }
                    else
                    {
                        foreach (var row in table.Value.seedTable.rows)
                        {
                            result.Add(this.queryService.GetSeedRow(row.TableName, row.columnValues));
                        }
                    }
                }
            }

            // primary keys

            foreach (var table in this.schema.tablesToCreate)
            {
                if (table.Value.HasPrimaryKeys)
                {
                    result.Add(this.queryService.GetAddPrimaryKeyConstraint(table.Value));
                }
            }

            // foreign keys

            foreach (var command in this.schema.tablesToCreate)
            {
                if (command.Value.HasForeignKeys)
                {
                    result.Add(this.queryService.GetAddForeignKeyConstraints(command.Value));
                }
            }
            return result;
        }

        public List<string> GetAlterTablesQueries()
        {
            var result = new List<string>();
            foreach (var table in this.schema.tablesToAlter)
            {
                var tableName = table.Value.TableName;
                if (table.Value.HasColumnsToAdd)
                {
                    foreach (var column in table.Value.columnsToAdd)
                    {
                        result.Add(this.queryService.GetAddColumn(tableName,column.Value));
                    }
                }

                if (table.Value.HasColumnsToModify)
                {
                    foreach (var column in table.Value.columnsToModify)
                    {
                        result.Add(this.queryService.GetModifyColumn(tableName, column.Value));
                    }
                }

                if (table.Value.HasColumnsToDrop)
                {
                    foreach (var columnName in table.Value.columnsToDrop)
                    {
                        result.Add(this.queryService.GetDropColumn(tableName, columnName));
                    }
                }

                if (table.Value.HasPrimaryKeyConstraint)
                {
                    result.Add(this.queryService.GetAddPrimaryKeyConstraint(tableName,table.Value.primaryKeyConstraint.PrimaryKeys));
                }

                if (table.Value.HasForeignKeyConstraints)
                {
                    foreach (var foreignKey in table.Value.foreignKeyConstraints)
                    {
                        result.Add(this.queryService.GetAddForeignKeyConstraint(tableName, foreignKey.Value));
                    }
                }
            }
            return result;
        }

        public List<string> GetDropTablesQueries()
        {
            var result = new List<string>();
            foreach (var tableName in this.schema.tablesToDrop)
            {
                result.Add(this.queryService.GetDropTable(tableName));
            }
            return result;
        }

        public List<string> GetDropStoredProceduresQueries()
        {
            var result = new List<string>();
            foreach (var procedureName in this.schema.storedProceduresToDrop)
            {
                result.Add(this.queryService.GetDropStoredProcedure(procedureName));
            }
            return result;
        }

        public List<string> GetCreateStoredProceduresQueries()
        {
            var result = new List<string>();
            foreach (var procedure in this.schema.storedProceduresToCreate)
            {
                // drop if exists
                result.Add(this.queryService.GetDropStoredProcedure(procedure.Value.ProcedureName));
                // create
                result.Add(this.queryService.GetCreateStoredProcedure(procedure.Value.ProcedureName, procedure.Value.Parameters, procedure.Value.Body));
            }
            return result;
        }

        public string GetCreateStoredProceduresQueryForScript()
        {
            var result = "DELIMITER $$\r\r";
            foreach (var procedure in this.schema.storedProceduresToCreate)
            {
                // drop if exists
                result += this.queryService.GetDropStoredProcedure(procedure.Value.ProcedureName, false) + "$$";
                // create
                result += this.queryService.GetCreateStoredProcedure(procedure.Value.ProcedureName, procedure.Value.Parameters, procedure.Value.Body) + "$$\r\r";
            }
            result += "DELIMITER ;\r";
            return result;
        }

        public List<string> GetMigrationQueries(string engine, bool script = false)
        {
            var result = new List<string>();

            this.TrySetEngine(engine);

            // drop database

            if (schema.HasDatabasesToDrop)
            {
                Util.Concat(result, this.GetDropDatabasesQueries());
            }

            // create database

            if (schema.HasDatabasesToCreate)
            {
                Util.Concat(result, this.GetCreateDatabasesQueries());
            }

            // create and use database

            if (schema.HasDatabasesToCreateAndUse)
            {
                Util.Concat(result, this.GetCreateAndUseDatabasesQueries());
            }

            // drop tables

            if (schema.HasTablesToDrop)
            {
                Util.Concat(result, this.GetDropTablesQueries());
            }

            // create tables

            if (schema.HasTablesToCreate)
            {
                Util.Concat(result, this.GetCreateTablesQueries(script));
            }

            // alter tables

            if (schema.HasTablesToAlter)
            {
                Util.Concat(result, this.GetAlterTablesQueries());
            }

            // drop stored procedure

            if (schema.HasStoredProceduresToDrop)
            {
                Util.Concat(result, this.GetDropStoredProceduresQueries());
            }

            // create stored procedure

            if (schema.HasStoredProceduresToCreate)
            {
                if (script)
                {
                    result.Add(this.GetCreateStoredProceduresQueryForScript());
                }
                else
                {
                    Util.Concat(result, this.GetCreateStoredProceduresQueries());
                }
            }

            return result;
        }

        public string GetMigrationQuery(string engine = null)
        {
            var result = this.GetMigrationQueries(engine, true);
            if (result.Count > 0)
            {
                return string.Join("\r", result);
            }
            else
            {
                return "";
            }
        }

        public List<string> GetSeedQueries()
        {
            var result = new List<string>();
            foreach (var table in this.schema.tablesToSeed)
            {
                if (table.Value.HasRows)
                {
                    foreach (var row in table.Value.rows)
                    {
                        result.Add(this.queryService.GetSeedRow(row.TableName, row.columnValues));
                    }
                }
            }
            return result;
        }

        public string GetSeedQuery()
        {
            var result = new List<string>();
            foreach (var table in this.schema.tablesToSeed)
            {
                if (table.Value.HasRows)
                {
                    result.Add(this.queryService.GetSeeds(table.Value));
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

        // assembly

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


        // migration

        public void DoMigrationsFromMemory(string connectionString, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var queries = this.GetMigrationQueries(engine);
            this.OpenConnectionAndExecuteQueries(queries, connectionString);
            schema.ClearMigrations();
        }

        public void GroupAndExecuteMigrations(List<RecognizedMigrationFile> recognizedTypes, 
            string connectionString, string engine = null, MigrationDirection direction = MigrationDirection.Up)
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

                    var queries = this.GetMigrationQueries(engine);
                    this.ExecuteQueries(queries);

                    schema.ClearMigrations();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.dbService.Close();
                schema.ClearMigrations();
            }
        }

        public void DoMigrationsForAssembly(string assemblyPath, 
            string connectionString, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedTypes = this.FindTypes<Migration>(assemblyPath);
            if (recognizedTypes.Count > 0)
            {
                this.GroupAndExecuteMigrations(recognizedTypes, connectionString, engine, direction);
            }
        }

        public void DoMigrationsForTypes(Type[] assemblyTypes, 
            string connectionString, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedTypes = this.FindTypes<Migration>(assemblyTypes);
            if (recognizedTypes.Count > 0)
            {
                this.GroupAndExecuteMigrations(recognizedTypes, connectionString, engine, direction);
            }
        }

        public void DoMigrationOnlyFor(string migrationFileName, Type[] assemblyTypes, string connectionString, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedType = this.FindType<Migration>(assemblyTypes, migrationFileName);
            if (recognizedType != null)
            {
                this.RunMigration(recognizedType, direction);
                this.DoMigrationsFromMemory(connectionString, engine, direction);
            }
        }

        public void DoMigrationOnlyFor(string migrationFileName, string assemblyPath, string connectionString, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            var recognizedType = this.FindType<Migration>(assemblyPath, migrationFileName);
            if (recognizedType != null)
            {
                this.RunMigration(recognizedType, direction);
                this.DoMigrationsFromMemory(connectionString, engine, direction);
            }
        }

        // seed

        public void DoSeedFromMemory(string connectionString)
        {
            var queries = this.GetSeedQueries();
            this.OpenConnectionAndExecuteQueries(queries, connectionString);
            schema.ClearSeeders();
        }

        public void GroupAndExecuteSeeders(List<RecognizedMigrationFile> recognizedTypes, string connectionString)
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

                    var queries = this.GetSeedQueries();
                    this.ExecuteQueries(queries);

                    this.schema.ClearSeeders();
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.dbService.Close();
                this.schema.ClearSeeders();
            }
        }

        public void DoSeedForAssembly(string assemblyPath, string connectionString)
        {
            var recognizedTypes = this.FindTypes<Seeder>(assemblyPath);
            if (recognizedTypes.Count > 0)
            {
                this.GroupAndExecuteSeeders(recognizedTypes, connectionString);
            }
        }

        public void DoSeedForTypes(Type[] assemblyTypes, string connectionString)
        {
            var recognizedTypes = this.FindTypes<Seeder>(assemblyTypes);
            if (recognizedTypes.Count > 0)
            {
                this.GroupAndExecuteSeeders(recognizedTypes, connectionString);
            }
        }

        public void DoSeedOne(RecognizedMigrationFile recognizedType, string connectionString)
        {
            try
            {
                this.RunSeeder(recognizedType);

                var queries = this.GetSeedQueries();
                this.OpenConnectionAndExecuteQueries(queries, connectionString);

                this.schema.ClearSeeders();
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                this.schema.ClearSeeders();
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

        // execute queries

        public bool IsValidQuery(string query)
        {
            return !string.IsNullOrWhiteSpace(query);
        }

        protected void ExecuteQueries(List<string> queries)
        {
            foreach (var query in queries)
            {
                if (this.IsValidQuery(query))
                {
                    this.dbService.Execute(query);
                }
            }
        }

        public void OpenConnectionAndExecuteQueries(List<string> queries, string connectionString)
        {
            if (queries.Count > 0)
            {
                this.dbService.CreateConnection(connectionString);
                this.dbService.Open();

                this.ExecuteQueries(queries);

                this.dbService.Close();
            }
        }

        public int OpenConnectionAndExecuteQuery(string query, string connectionString)
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

        // save to file

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
                schema.ClearMigrations();
            }

        }

        public void CreateMigrationScriptFromMemory(string fileName, string engine = null)
        {
            try
            {
                var query = GetMigrationQuery(engine);
                this.CreateScript(fileName, query);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                schema.ClearMigrations();
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
                schema.ClearSeeders();
            }
        }

        public void CreateSeedScriptFromMemory(string fileName)
        {
            try
            {
                var query = GetSeedQuery();
                this.CreateScript(fileName, query);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                schema.ClearSeeders();
            }

        }
    }
}
