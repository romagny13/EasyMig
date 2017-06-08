using EasyMigLib.Commands;
using EasyMigLib.MigrationReflection;
using System;

namespace EasyMigLib.MySqlClient
{
    public class MySqlDb
    {
        protected MySqlSchema container;

        public MySqlDb(CommandContainer commandContainer)
        {
            this.container = new MySqlSchema(commandContainer);
        }      

        public string GetMigrationQuery(string engine = "InnoDB")
        {
            return container.GetMigrationQuery(engine);
        }

        public string GetStoredProcedureQuery()
        {
            return container.GetStoredProcedureQuery();
        }

        public string GetSeedQuery(string engine = "InnoDB")
        {
            return container.GetSeedQuery(engine);
        }

        public void DoSeedForAssembly(string assemblyPath, string connectionString, string engine = "InnoDB")
        {
            container.DoSeedForAssembly(assemblyPath, connectionString, engine);
        }

        public void DoSeedForTypes(Type[] assemblyTypes, string connectionString, string engine = "InnoDB")
        {
            container.DoSeedForTypes(assemblyTypes, connectionString, engine);
        }

        public void DoSeedOnlyFor(string seedFileName, string assemblyPath, string connectionString, string engine = "InnoDB")
        {
            container.DoSeedOnlyFor(seedFileName, assemblyPath, connectionString, engine);
        }

        public void DoSeedOnlyFor(string seedFileName, Type[] assemblyTypes, string connectionString,  string engine = "InnoDB")
        {
            container.DoSeedOnlyFor(seedFileName, assemblyTypes, connectionString, engine);
        }

        public void DoSeedFromMemory(string connectionString, string engine = "InnoDB")
        {
            container.DoSeedFromMemory(connectionString, engine);
        }

        public void DoMigrationsForAssembly(string assemblyPath, string connectionString, string engine = "InnoDB", MigrationDirection direction = MigrationDirection.Up)
        {
            container.DoMigrationsForAssembly(assemblyPath, connectionString, engine, direction);
        }

        public void DoMigrationsForTypes(Type[] assemblyTypes, string connectionString, string engine = "InnoDB", MigrationDirection direction = MigrationDirection.Up)
        {
            container.DoMigrationsForTypes(assemblyTypes, connectionString, engine, direction);
        }

        public void DoMigrationOnlyFor(string migrationFileName, string assemblyPath, string connectionString, string engine = "InnoDB", MigrationDirection direction = MigrationDirection.Up)
        {
            container.DoMigrationOnlyFor(migrationFileName, assemblyPath, connectionString, engine, direction);
        }

        public void DoMigrationOnlyFor(string migrationFileName, Type[] assemblyTypes, string connectionString, string engine = "InnoDB", MigrationDirection direction = MigrationDirection.Up)
        {
            container.DoMigrationOnlyFor(migrationFileName, assemblyTypes, connectionString, engine, direction);
        }

        public void DoMigrationsFromMemory(string connectionString, string engine = "InnoDB")
        {
            container.DoMigrationsFromMemory(connectionString, engine);
        }

        public int ExecuteQuery(string query, string connectionString)
        {
            return container.ExecuteQuery(query, connectionString);
        }

        public void CreateMigrationScript(string assemblyPath, string fileName, string engine = "InnoDB")
        {
            container.CreateMigrationsScript(assemblyPath, fileName, engine);
        }

        public void CreateStoredProcedureScript(string assemblyPath, string fileName)
        {
            container.CreateStoredProcedureScript(assemblyPath, fileName);
        }

        public void CreateSeedScript(string assemblyPath, string fileName, string engine = "InnoDB")
        {
            container.CreateSeedScript(assemblyPath, fileName, engine);
        }
    }
}
