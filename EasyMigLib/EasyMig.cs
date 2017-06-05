using EasyMigLib.Commands;
using EasyMigLib.Services;
using System;
using System.IO;

namespace EasyMigLib
{
    public class EasyMig
    {
        internal static CommandContainer container;

        public static DatabaseInformation DatabaseInformation { get; }

        static EasyMig()
        {
            container = new CommandContainer();

            DatabaseInformation = new DatabaseInformation();
        }

        public static CreateDatabaseCommand CreateDatabase(string databaseName)
        {
            return container.CreateDatabase(databaseName);
        }

        public static DropDatabaseCommand DropDatabase(string databaseName)
        {
            return container.DropDatabase(databaseName);
        }

        public static CreateTableCommand CreateTable(string tableName)
        {
            return container.CreateTable(tableName);
        }

        public static AlterTableCommand AlterTable(string tableName)
        {
            return container.AlterTable(tableName);
        }

        public static DropTableCommand DropTable(string tableName)
        {
            return container.DropTable(tableName);
        }       

        public static SeedTableCommand SeedTable(string tableName)
        {
            return container.SeedTable(tableName);
        }

        public static string GetMigrationQuery(string providerName, string engine = null)
        {
            return container.GetMigrationQuery(providerName, engine);
        }

        public static string GetSeedQuery(string providerName, string engine = null)
        {
            return container.GetSeedQuery(providerName, engine);
        }

        public static void DoSeedForAssembly(string assemblyPath,string connectionString, string providerName, string engine = null)
        {
            container.DoSeedForAssembly(assemblyPath, connectionString, providerName, engine);
        }

        public static void DoSeedForTypes(Type[] assemblyTypes, string connectionString, string providerName, string engine = null)
        {
            container.DoSeedForTypes(assemblyTypes, connectionString, providerName, engine);
        }

        public static void DoSeedOnlyFor(string seedFileName, string assemblyPath, string connectionString, string providerName, string engine = null)
        {
            container.DoSeedOnlyFor(seedFileName, assemblyPath, connectionString, providerName, engine);
        }

        public static void DoSeedOnlyFor(string seedFileName, Type[] assemblyTypes, string connectionString, string providerName, string engine = null)
        {
            container.DoSeedOnlyFor(seedFileName, assemblyTypes, connectionString, providerName, engine);
        }

        public static void DoSeedFromMemory(string connectionString, string providerName, string engine = null)
        {
            container.DoSeedFromMemory(connectionString, providerName, engine);
        }

        public static void DoMigrationsForAssembly(string assemblyPath, string connectionString, string providerName, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            container.DoMigrationsForAssembly(assemblyPath, connectionString, providerName, engine, direction);
        }

        public static void DoMigrationsForTypes(Type[] assemblyTypes, string connectionString, string providerName, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            container.DoMigrationsForTypes(assemblyTypes, connectionString, providerName, engine, direction);
        }

        public static void DoMigrationOnlyFor(string migrationFileName,string assemblyPath, string connectionString, string providerName, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            container.DoMigrationOnlyFor(migrationFileName, assemblyPath, connectionString, providerName, engine, direction);
        }

        public static void DoMigrationOnlyFor(string migrationFileName, Type[] assemblyTypes, string connectionString, string providerName, string engine = null, MigrationDirection direction = MigrationDirection.Up)
        {
            container.DoMigrationOnlyFor(migrationFileName, assemblyTypes, connectionString, providerName, engine, direction);
        }

        public static void DoMigrationsFromMemory(string connectionString, string providerName, string engine = null)
        {
            container.UpdateDatabase(connectionString, providerName, engine);
        }        

        public static int ExecuteQuery(string query, string connectionString, string providerName)
        {
            return container.ExecuteQuery(query, connectionString, providerName);
        }

        public static void CreateMigrationScript(string assemblyPath, string providerName, string fileName, string engine = null)
        {
           container.CreateMigrationsScript(assemblyPath, providerName, fileName, engine);
        }

        public static void CreateSeedScript(string assemblyPath, string providerName, string fileName, string engine = null)
        {
            container.CreateSeedScript(assemblyPath, providerName, fileName, engine);
        }

        public static void ClearMigrations()
        {
            container.ClearMigrations();
        }

        public static void ClearSeeders()
        {
            container.ClearSeeders();
        }
    }
}
