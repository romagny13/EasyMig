using EasyMigLib.Information;
using EasyMigLib.Migrations.MySqlClient;
using EasyMigLib.Migrations.SqlClient;
using EasyMigLib.Schema;

namespace EasyMigLib
{
    public class EasyMig
    {
        internal static DatabaseSchema schema;

        public static MySqlDb ToMySql => new MySqlDb(schema);

        public static SqlServerDb ToSqlServer => new SqlServerDb(schema);

        public static DatabaseInformation Information => new DatabaseInformation();

        static EasyMig()
        {
            schema = new DatabaseSchema();
        }

        public static void CreateDatabase(string databaseName)
        {
            schema.CreateDatabase(databaseName);
        }

        public static void CreateAndUseDatabase(string databaseName)
        {
            schema.CreateAndUseDatabase(databaseName);
        }

        public static void DropDatabaseIfExists(string databaseName)
        {
            schema.DropDatabase(databaseName);
        }

        public static CreateTableSchema CreateTable(string tableName)
        {
            return schema.CreateTable(tableName);
        }

        public static AlterTableSchema AlterTable(string tableName)
        {
            return schema.AlterTable(tableName);
        }

        public static void DropTable(string tableName)
        {
            schema.DropTable(tableName);
        }

        public static StoredProcedureSchema CreateStoredProcedure(string procedureName)
        {
            return schema.CreateStoredProcedure(procedureName);
        }

        public static void DropStoredProcedure(string procedureName)
        {
            schema.DropStoredProcedure(procedureName);
        }

        public static SeedTableSchema SeedTable(string tableName)
        {
            return schema.SeedTable(tableName);
        }

        public static void ClearMigrations()
        {
            schema.ClearMigrations();
        }

        public static void ClearSeeders()
        {
            schema.ClearSeeders();
        }

    }
}
