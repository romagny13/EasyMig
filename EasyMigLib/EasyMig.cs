using EasyMigLib.Commands;
using EasyMigLib.MySqlClient;
using EasyMigLib.SqlClient;
using EasyMigLib.SqlClientAttachedDbFile;

namespace EasyMigLib
{

    public static class EasyMig
    {
        private static CommandContainer container;

        public static MySqlDb ToMySql => new MySqlDb(container);

        public static SqlServerDb ToSqlServer => new SqlServerDb(container);

        public static SqlServerAttachedDbFile ToSqlServerAttachedDbFile => new SqlServerAttachedDbFile(container);

        public static DatabaseInformation Information => new DatabaseInformation();

        static EasyMig()
        {
            container = new CommandContainer();
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
