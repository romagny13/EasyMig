namespace EasyMigLib.Services
{
    public abstract class DatabaseInformationQueryService
    {
        public abstract string GetDatabaseExists(string databaseName);
        public abstract string GetTableExists(string databaseName, string tableName);
        public abstract string GetColumnExists(string databaseName, string tableName, string columnName);
        public abstract string GetTable(string databaseName, string tableName);
        public abstract string GetColumns(string databaseName, string tableName);
        public abstract string GetForeignKeys(string databaseName, string tableName);
        public abstract string GetTableRows(string tableName, int? limit);
        public abstract string GetPrimaryKeys(string databaseName, string tableName);
    }

}
