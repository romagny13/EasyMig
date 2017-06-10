using EasyMigLib.MigrationReflection;
using EasyMigLib.Schema;
using System;
using System.Collections.Generic;

namespace EasyMigLib.Migrations.SqlClient
{
    public class SqlServerDb
    {
        protected SqlServerExecutor executor;

        public SqlServerDb(DatabaseSchema schema)
        {
            this.executor = new SqlServerExecutor(schema);
        }

        public string GetMigrationQuery()
        {
            return executor.GetMigrationQueryWithGODelimiter();
        }

        public string GetSeedQuery()
        {
            return executor.GetSeedQueryWithGODelimiter();
        }

        public void DoSeedForAssembly(string assemblyPath, string connectionString)
        {
            executor.DoSeedForAssembly(assemblyPath, connectionString);
        }

        public void DoSeedForTypes(Type[] assemblyTypes, string connectionString)
        {
            executor.DoSeedForTypes(assemblyTypes, connectionString);
        }

        public void DoSeedOnlyFor(string seedFileName, string assemblyPath, string connectionString)
        {
            executor.DoSeedOnlyFor(seedFileName, assemblyPath, connectionString);
        }

        public void DoSeedFromMemory(string connectionString)
        {
            executor.DoSeedFromMemory(connectionString);
        }

        public void DoMigrationsForAssembly(string assemblyPath, 
            string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            executor.DoMigrationsForAssembly(assemblyPath, connectionString, direction);
        }

        public void DoMigrationsForTypes(Type[] assemblyTypes, 
            string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            executor.DoMigrationsForTypes(assemblyTypes, connectionString, direction);
        }

        public void DoMigrationOnlyFor(string migrationFileName, string assemblyPath, 
            string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            executor.DoMigrationOnlyFor(migrationFileName, assemblyPath, connectionString, direction);
        }

        public void DoMigrationsFromMemory(string connectionString)
        {
            executor.DoMigrationsFromMemory(connectionString);
        }

        public int ExecuteQuery(string query, string connectionString)
        {
            return executor.OpenConnectionAndExecuteQuery(query, connectionString);
        }

        public void ExecuteQueries(List<string> queries, string connectionString)
        {
            executor.OpenConnectionAndExecuteQueries(queries, connectionString);
        }

        public void CreateMigrationScript(string assemblyPath, string fileName)
        {
            executor.CreateMigrationsScript(assemblyPath, fileName);
        }

        public void CreateMigrationScriptFromMemory(string fileName)
        {
            executor.CreateMigrationScriptFromMemory(fileName);
        }

        public void CreateSeedScript(string assemblyPath, string fileName)
        {
            executor.CreateSeedScript(assemblyPath, fileName);
        }

        public void CreateSeedScriptFromMemory(string fileName)
        {
            executor.CreateSeedScriptFromMemory(fileName);
        }
    }
}
