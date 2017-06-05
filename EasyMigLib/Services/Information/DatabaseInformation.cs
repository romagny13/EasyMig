using System.Collections.Generic;

namespace EasyMigLib.Services
{
    public class DatabaseInformation
    {
        protected IDbService dbService;

        public DatabaseInformation()
            : this(new DbService())
        { }

        public DatabaseInformation(IDbService dbService)
        {
            this.dbService = dbService;
        }

        protected bool CheckDatabaseInformation(string query, string connectionString, string providerName)
        {
            dbService.CreateConnection(connectionString, providerName);
            dbService.Open();
            var result = dbService.ExecuteScalar(query);
            dbService.Close();

            int intResult = -1;
            int.TryParse(result.ToString(), out intResult);

            return intResult == 1;
        }

        public bool DatabaseExists(string databaseName, string connectionString, string providerName)
        {
            var queryService = DatabaseInformationQueryFactories.GetService(providerName);
            var query = queryService.GetDatabaseExists(databaseName);

            return this.CheckDatabaseInformation(query, connectionString, providerName);
        }

        public bool TableExists(string databaseName, string tableName, string connectionString, string providerName)
        {
            var queryService = DatabaseInformationQueryFactories.GetService(providerName);
            var query = queryService.GetTableExists(databaseName, tableName);

            return this.CheckDatabaseInformation(query, connectionString, providerName);
        }

        public bool ColumnExists(string databaseName, string tableName, string columnName, string connectionString, string providerName)
        {
            var queryService = DatabaseInformationQueryFactories.GetService(providerName);
            var query = queryService.GetColumnExists(databaseName, tableName, columnName);

            return this.CheckDatabaseInformation(query, connectionString, providerName);
        }

        public List<Dictionary<string, object>> GetTableRows(string tableName, string connectionString, string providerName, int? limit = null)
        {
            var queryService = DatabaseInformationQueryFactories.GetService(providerName);

            var query = queryService.GetTableRows(tableName, limit);

            dbService.CreateConnection(connectionString, providerName);
            dbService.Open();

            var result = dbService.ReadAll(query);

            return result;
        }

        public Dictionary<string, Dictionary<string, object>> MergeListToDictionary(List<Dictionary<string, object>> list, string columnKey)
        {
            var result = new Dictionary<string, Dictionary<string, object>>();
            foreach (var item in list)
            {
                var key = item[columnKey].ToString();
                result[key] = item;
            }
            return result;
        }

        public DatabaseTable GetTable(string databaseName, string tableName, string connectionString, string providerName)
        {
            var queryService = DatabaseInformationQueryFactories.GetService(providerName);

            var query = queryService.GetTable(databaseName, tableName);
            var columnQuery = queryService.GetColumns(databaseName, tableName);
            var primaryKeyQuery = queryService.GetPrimaryKeys(databaseName, tableName);
            var foreignKeyQuery = queryService.GetForeignKeys(databaseName, tableName);

            dbService.CreateConnection(connectionString, providerName);
            dbService.Open();

            var table = dbService.ReadOne(query);
            var columnList = dbService.ReadAll(columnQuery);
            var primaryKeyList = dbService.ReadAll(primaryKeyQuery);
            var foreignKeyList = dbService.ReadAll(foreignKeyQuery);

            var columns = this.MergeListToDictionary(columnList, "COLUMN_NAME");
            var primaryKeys = this.MergeListToDictionary(primaryKeyList, "COLUMN_NAME");
            var foreignKeys = this.MergeListToDictionary(foreignKeyList, "COLUMN_NAME");

            dbService.Close();

            return new DatabaseTable(tableName, table, columns, primaryKeys, foreignKeys);
        }
    }

}
