using System.Collections.Generic;
using EasyMigLib.Commands;

namespace EasyMigLib.Query
{
    public interface IQueryService
    {
        string EndQuote { get; }
        string StartQuote { get; }
        string Delimiter { get; }

        void AddTimestamps(CreateTableCommand table);
        string FormatValueString(object value);
        string FormatWithSchemaName(string value);
        string GetAddColumn(string tableName, MigrationColumn column);
        string GetAddForeignKeyConstraint(string tableName, ForeignKeyColumn foreignKey);
        string GetAddForeignKeyConstraints(CreateTableCommand createTableCommand);
        string GetAddPrimaryKeyConstraint(CreateTableCommand createTableCommand);
        string GetAddPrimaryKeyConstraint(string tableName, string[] primaryKeys);
        string GetColumn(MigrationColumn column);
        string GetColumnType(ColumnType columnType);
        string GetCreateDatabase(string databaseName);
        string GetCreateAndUseDatabase(string databaseName);
        string GetCreateStoredProcedure(string procedureName, Dictionary<string, DatabaseParameter> parameters, string body);
        string GetCreateTable(CreateTableCommand table);
        string GetDropColumn(string tableName, string columnName);
        string GetDropDatabase(string databaseName);
        string GetDropStoredProcedure(string procedureName, bool delimiter = true);
        string GetDropTable(string tableName);
        string GetModifyColumn(string tableName, MigrationColumn column);
        string GetParameter(DatabaseParameter parameter);
        string GetParameters(Dictionary<string, DatabaseParameter> parameters);
        List<string> GetReservedWords();
        string GetSeedColumns(Dictionary<string, object> columnValues);
        string GetSeedRow(string tableName, Dictionary<string, object> columnValues);
        string GetSeeds(CreateTableCommand createTableCommand);
        string GetSeedValues(Dictionary<string, object> columnValues);
        string GetUseDatabase(string databaseName);
        bool IsReservedWord(string value);
        void SetDefaultDelimiter(string delimiter);
        string WrapWithQuotes(string value);
    }
}