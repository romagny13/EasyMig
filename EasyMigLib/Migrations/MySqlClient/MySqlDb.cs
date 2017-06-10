using EasyMigLib.MigrationReflection;
using EasyMigLib.Schema;
using System;
using System.Collections.Generic;

namespace EasyMigLib.Migrations.MySqlClient
{
    public class MySqlDb
    {
        protected MySqlExecutor executor;

        public MySqlDb(DatabaseSchema schema)
        {
            this.executor = new MySqlExecutor(schema);
        }

        public string GetMigrationQuery(string engine = "InnoDB")
        {
            return executor.GetMigrationQuery(engine);
        }

        public string GetSeedQuery()
        {
            return executor.GetSeedQuery();
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
            string connectionString, string engine = "InnoDB", MigrationDirection direction = MigrationDirection.Up)
        {
            executor.DoMigrationsForAssembly(assemblyPath, connectionString, engine, direction);
        }

        public void DoMigrationsForTypes(Type[] assemblyTypes, 
            string connectionString, string engine = "InnoDB", MigrationDirection direction = MigrationDirection.Up)
        {
            executor.DoMigrationsForTypes(assemblyTypes, connectionString, engine, direction);
        }

        public void DoMigrationOnlyFor(string migrationFileName, string assemblyPath, 
            string connectionString, string engine = "InnoDB", MigrationDirection direction = MigrationDirection.Up)
        {
            executor.DoMigrationOnlyFor(migrationFileName, assemblyPath, connectionString, engine, direction);
        }

        public void DoMigrationsFromMemory(string connectionString, string engine = "InnoDB")
        {
            executor.DoMigrationsFromMemory(connectionString, engine);
        }

        public int ExecuteQuery(string query, string connectionString)
        {
            return executor.OpenConnectionAndExecuteQuery(query, connectionString);
        }

        public void ExecuteQueries(List<string> queries, string connectionString)
        {
            executor.OpenConnectionAndExecuteQueries(queries, connectionString);
        }

        public void CreateMigrationScript(string assemblyPath, string fileName, string engine = "InnoDB")
        {
            executor.CreateMigrationsScript(assemblyPath, fileName, engine);
        }

        public void CreateMigrationScriptFromMemory(string fileName, string engine = "InnoDB")
        {
            executor.CreateMigrationScriptFromMemory(fileName, engine);
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
