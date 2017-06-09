using EasyMigLib.Db.MySqlClient;
using System.Collections.Generic;

namespace EasyMigLib.Information.MySqlClient
{
    public class MySqlDatabaseInformation
    {
        protected IMySqlDbService dbService;
        protected IMySqlDatabaseInformationQueryService queryService;


        public MySqlDatabaseInformation()
            :this(new MySqlDbService(), new MySqlDatabaseInformationQueryService())
        { }

        public MySqlDatabaseInformation(IMySqlDbService dbService, IMySqlDatabaseInformationQueryService queryService)
        {
            this.dbService = dbService;
            this.queryService = queryService;
        }

        protected bool CheckDatabaseInformation(string query, string connectionString)
        {
            this.dbService.CreateConnection(connectionString);
            this.dbService.Open();

            var result = dbService.ExecuteScalar(query);

            this.dbService.Close();

            int intResult = -1;
            int.TryParse(result.ToString(), out intResult);

            return intResult == 1;
        }

        public bool DatabaseExists(string databaseName, string connectionString)
        {
            var query = this.queryService.GetDatabaseExists(databaseName);

            return this.CheckDatabaseInformation(query, connectionString);
        }

        public bool TableExists(string databaseName, string tableName, string connectionString)
        {
            var query = this.queryService.GetTableExists(databaseName, tableName);

            return this.CheckDatabaseInformation(query, connectionString);
        }

        public bool ColumnExists(string databaseName, string tableName, string columnName, string connectionString)
        {
            var query = this.queryService.GetColumnExists(databaseName, tableName, columnName);

            return this.CheckDatabaseInformation(query, connectionString);
        }

        public bool ProcedureExists(string databaseName, string procedureName, string connectionString)
        {
            var query = this.queryService.GetProcedureExists(databaseName, procedureName);

            return this.CheckDatabaseInformation(query, connectionString);
        }

        public List<Dictionary<string, object>> GetTableRows(string tableName, string connectionString, int? limit = null)
        {
            var query = this.queryService.GetTableRows(tableName, limit);

            this.dbService.CreateConnection(connectionString);
            this.dbService.Open();

            var result = this.dbService.ReadAll(query);

            return result;
        }

        protected Dictionary<string, Dictionary<string, object>> MergeListToDictionary(List<Dictionary<string, object>> list, string columnKey)
        {
            var result = new Dictionary<string, Dictionary<string, object>>();
            foreach (var item in list)
            {
                var key = item[columnKey].ToString();
                result[key] = item;
            }
            return result;
        }

        public DatabaseTable GetTable(string databaseName, string tableName, string connectionString)
        {
            var query = this.queryService.GetTable(databaseName, tableName);
            var columnQuery = this.queryService.GetColumns(databaseName, tableName);
            var primaryKeyQuery = this.queryService.GetPrimaryKeys(databaseName, tableName);
            var foreignKeyQuery = this.queryService.GetForeignKeys(databaseName, tableName);

            this.dbService.CreateConnection(connectionString);
            this.dbService.Open();

            var table = this.dbService.ReadOne(query);
            var columnList = this.dbService.ReadAll(columnQuery);
            var primaryKeyList = this.dbService.ReadAll(primaryKeyQuery);
            var foreignKeyList = this.dbService.ReadAll(foreignKeyQuery);

            var columns = this.MergeListToDictionary(columnList, "COLUMN_NAME");
            var primaryKeys = this.MergeListToDictionary(primaryKeyList, "COLUMN_NAME");
            var foreignKeys = this.MergeListToDictionary(foreignKeyList, "COLUMN_NAME");

            this.dbService.Close();

            return new DatabaseTable(tableName, table, columns, primaryKeys, foreignKeys);
        }
    }
}
