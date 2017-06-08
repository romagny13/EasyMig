using EasyMigLib.Commands;
using EasyMigLib.MigrationReflection;
using System;

namespace EasyMigLib.SqlClient
{
    public class SqlServerDb
    {
        protected SqlSchema container;

        public SqlServerDb(CommandContainer commandContainer)
        {
            this.container = new SqlSchema(commandContainer);
        }

        public string GetMigrationQuery()
        {
            return container.GetMigrationQuery();
        }

        public string GetStoredProcedureQuery()
        {
            return container.GetStoredProcedureQuery();
        }

        public string GetSeedQuery()
        {
            return container.GetSeedQuery();
        }

        public void DoSeedForAssembly(string assemblyPath, string connectionString)
        {
            container.DoSeedForAssembly(assemblyPath, connectionString);
        }

        public void DoSeedForTypes(Type[] assemblyTypes, string connectionString)
        {
            container.DoSeedForTypes(assemblyTypes, connectionString);
        }

        public void DoSeedOnlyFor(string seedFileName, string assemblyPath, string connectionString)
        {
            container.DoSeedOnlyFor(seedFileName, assemblyPath, connectionString);
        }

        public void DoSeedOnlyFor(string seedFileName, Type[] assemblyTypes, string connectionString)
        {
            container.DoSeedOnlyFor(seedFileName, assemblyTypes, connectionString);
        }

        public void DoSeedFromMemory(string connectionString)
        {
            container.DoSeedFromMemory(connectionString);
        }

        public void DoMigrationsForAssembly(string assemblyPath, string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            container.DoMigrationsForAssembly(assemblyPath, connectionString, direction);
        }

        public void DoMigrationsForTypes(Type[] assemblyTypes, string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            container.DoMigrationsForTypes(assemblyTypes, connectionString, direction);
        }

        public void DoMigrationsFromMemory(string connectionString)
        {
            container.DoMigrationsFromMemory(connectionString);
        }

        public void DoMigrationOnlyFor(string migrationFileName, string assemblyPath, string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            container.DoMigrationOnlyFor(migrationFileName, assemblyPath, connectionString, direction);
        }

        public void DoMigrationOnlyFor(string migrationFileName, Type[] assemblyTypes, string connectionString, MigrationDirection direction = MigrationDirection.Up)
        {
            container.DoMigrationOnlyFor(migrationFileName, assemblyTypes, connectionString, direction);
        }

        public int ExecuteQuery(string query, string connectionString)
        {
            return container.ExecuteQuery(query, connectionString);
        }

        public void CreateMigrationScript(string assemblyPath, string fileName)
        {
            container.CreateMigrationsScript(assemblyPath, fileName);
        }

        public void CreateStoredProcedureScript(string assemblyPath, string fileName)
        {
            container.CreateStoredProcedureScript(assemblyPath, fileName);
        }

        public void CreateSeedScript(string assemblyPath, string fileName)
        {
            container.CreateSeedScript(assemblyPath, fileName);
        }
       
    }
}
