using System.Collections.Generic;
using EasyMigLib.Schema;

namespace EasyMigLib.Query
{
    public interface IQueryService
    {
        string Delimiter { get; }
        string EndQuote { get; }
        string StartQuote { get; }

        void AddTimestamps(CreateTableSchema table);
        string FormatBody(string body);
        string FormatValueString(object value);
        string FormatWithSchemaName(string value);
        string GetAddColumn(string tableName, MigrationColumn column);
        string GetAddForeignKeyConstraint(string tableName, ForeignKeyColumn foreignKey);
        string GetAddForeignKeyConstraint(string tableName, ForeignKeyConstraint foreignKey);
        string GetAddForeignKeyConstraints(CreateTableSchema table);
        string GetAddPrimaryKeyConstraint(CreateTableSchema table);
        string GetAddPrimaryKeyConstraint(string tableName, string[] primaryKeys);
        string GetColumn(MigrationColumn column);
        string GetColumnType(ColumnType columnType);
        string GetCreateAndUseDatabase(string databaseName);
        string GetCreateDatabase(string databaseName);
        string GetCreateStoredProcedure(string procedureName, Dictionary<string, StoredProcedureParameter> parameters, string body);
        string GetCreateTable(CreateTableSchema table);
        string GetDefaultDelimiter();
        string GetDropColumn(string tableName, string columnName);
        string GetDropDatabase(string databaseName);
        string GetDropStoredProcedure(string procedureName, bool delimiter = true);
        string GetDropTable(string tableName);
        string GetModifyColumn(string tableName, MigrationColumn column);
        string GetParameter(StoredProcedureParameter parameter);
        string GetParameters(Dictionary<string, StoredProcedureParameter> parameters);
        List<string> GetReservedWords();
        string GetSeedColumns(Dictionary<string, object> columnValues);
        string GetSeedRow(string tableName, Dictionary<string, object> columnValues);
        string GetSeeds(CreateTableSchema table);
        string GetSeeds(SeedTableSchema table);
        string GetSeedValues(Dictionary<string, object> columnValues);
        string GetUseDatabase(string databaseName);
        bool IsReservedWord(string value);
        void SetDefaultDelimiter(string delimiter);
        string WrapWithQuotes(string value);
    }
}