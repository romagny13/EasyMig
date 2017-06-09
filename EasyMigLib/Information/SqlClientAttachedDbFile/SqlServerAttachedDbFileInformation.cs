using EasyMigLib.Db.Common;
using System.Collections.Generic;

namespace EasyMigLib.Information.SqlClientAttachedDbFile
{
    public class SqlServerAttachedDbFileInformation
    {
        protected IDbService dbService;
        protected ISqlServerAttachedDbFileInformationQueryService queryService;

        public string ProviderName { get; }

        public SqlServerAttachedDbFileInformation()
            :this(new DbService(), new SqlServerAttachedDbFileInformationQueryService())
        { }

        public SqlServerAttachedDbFileInformation(IDbService dbService, ISqlServerAttachedDbFileInformationQueryService queryService)
        {
            this.dbService = dbService;
            this.queryService = queryService;

            this.ProviderName = "System.Data.SqlClient";
        }

        protected bool CheckDatabaseInformation(string query, string connectionString)
        {
            this.dbService.CreateConnection(connectionString, this.ProviderName);
            this.dbService.Open();

            var result = dbService.ExecuteScalar(query);

            this.dbService.Close();

            int intResult = -1;
            int.TryParse(result.ToString(), out intResult);

            return intResult == 1;
        }

        public bool TableExists(string tableName, string connectionString)
        {
            var query = this.queryService.GetTableExists(tableName);

            return this.CheckDatabaseInformation(query, connectionString);
        }

        public bool ColumnExists(string tableName, string columnName, string connectionString)
        {
            var query = this.queryService.GetColumnExists(tableName, columnName);

            return this.CheckDatabaseInformation(query, connectionString);
        }

        public bool ProcedureExists(string procedureName, string connectionString)
        {
            var query = this.queryService.GetProcedureExists(procedureName);

            return this.CheckDatabaseInformation(query, connectionString);
        }

        public List<Dictionary<string, object>> GetTableRows(string tableName, string connectionString, int? limit = null)
        {
            var query = this.queryService.GetTableRows(tableName, limit);

            this.dbService.CreateConnection(connectionString, this.ProviderName);
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

        public DatabaseTable GetTable(string tableName, string connectionString)
        {
            var query = this.queryService.GetTable(tableName);
            var columnQuery = this.queryService.GetColumns(tableName);
            var primaryKeyQuery = this.queryService.GetPrimaryKeys(tableName);
            var foreignKeyQuery = this.queryService.GetForeignKeys(tableName);

            this.dbService.CreateConnection(connectionString, this.ProviderName);
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
