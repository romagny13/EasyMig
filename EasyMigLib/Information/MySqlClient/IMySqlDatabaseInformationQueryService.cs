namespace EasyMigLib.Information.MySqlClient
{
    public interface IMySqlDatabaseInformationQueryService
    {
        string GetColumnExists(string databaseName, string tableName, string columnName);
        string GetColumns(string databaseName, string tableName);
        string GetDatabaseExists(string databaseName);
        string GetForeignKeys(string databaseName, string tableName);
        string GetPrimaryKeys(string databaseName, string tableName);
        string GetProcedureExists(string databaseName, string procedureName);
        string GetTable(string databaseName, string tableName);
        string GetTableExists(string databaseName, string tableName);
        string GetTableRows(string tableName, int? limit);
    }
}