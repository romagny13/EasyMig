using EasyMigLib.Commands;
using EasyMigLib.Information;
using EasyMigLib.Migrations.MySqlClient;
using EasyMigLib.Migrations.SqlClient;

namespace EasyMigLib
{

    public static class EasyMig
    {
        private static CommandContainer container;

        public static MySqlDb ToMySql => new MySqlDb(container);

        public static SqlServerDb ToSqlServer => new SqlServerDb(container);

        public static DatabaseInformation Information => new DatabaseInformation();

        static EasyMig()
        {
            container = new CommandContainer();
        }

        public static CreateDatabaseCommand CreateDatabase(string databaseName)
        {
            return container.CreateDatabase(databaseName);
        }

        public static CreateAndUseDatabaseCommand CreateAndUseDatabase(string databaseName)
        {
            return container.CreateAndUseDatabase(databaseName);
        }

        public static DropDatabaseCommand DropDatabaseIfExists(string databaseName)
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

        public static CreateStoredProcedureCommand CreateStoredProcedure(string procedureName)
        {
            return container.CreateStoredProcedure(procedureName);
        }

        public static DropStoredProcedureCommand DropStoredProcedure(string procedureName)
        {
            return container.DropStoredProcedure(procedureName);
        }

        public static SeedTableCommand SeedTable(string tableName)
        {
            return container.SeedTable(tableName);
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
