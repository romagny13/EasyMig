namespace EasyMigLib.Information.SqlClientAttachedDbFile
{
    public interface ISqlServerAttachedDbFileInformationQueryService
    {
        string GetColumnExists(string tableName, string columnName);
        string GetColumns(string tableName);
        string GetForeignKeys(string tableName);
        string GetPrimaryKeys(string tableName);
        string GetProcedureExists(string procedureName);
        string GetTable(string tableName);
        string GetTableExists(string tableName);
        string GetTableRows(string tableName, int? limit);
    }
}