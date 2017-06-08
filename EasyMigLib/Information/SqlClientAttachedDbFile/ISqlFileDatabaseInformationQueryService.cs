namespace EasyMigLib.Information.SqlClientAttachedDbFile
{
    public interface ISqlFileDatabaseInformationQueryService
    {
        string GetColumnExists(string tableName, string columnName);
        string GetColumns(string tableName);
        string GetForeignKeys(string tableName);
        string GetPrimaryKeys(string tableName);
        string GetTable(string tableName);
        string GetTableExists(string tableName);
        string GetTableRows(string tableName, int? limit);
        string GetProcedureExists(string procedureName);
    }
}