using EasyMigLib.Commands;
using EasyMigLib.MigrationReflection;
using System;

namespace EasyMigLib.Migrations.MySqlClient
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

        public void DoSeedFromMemory(string connectionString)
        {
            container.DoSeedFromMemory(connectionString);
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

        public void CreateSeedScript(string assemblyPath, string fileName)
        {
            container.CreateSeedScript(assemblyPath, fileName);
        }
    }
}
