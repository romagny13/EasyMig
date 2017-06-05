using EasyMigLib.Services;
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

        public DatabaseTable GetTable(string databaseName, string tableName, string connectionString, string providerName)
        {
            var queryService = DatabaseInformationQueryFactories.GetService(providerName);

            var query = queryService.GetTable(databaseName, tableName);
            var columnQuery = queryService.GetColumns(databaseName, tableName);
            var foreignKeyQuery = queryService.GetForeignKeys(databaseName, tableName);

            dbService.CreateConnection(connectionString, providerName);
            dbService.Open();

            var table = dbService.ReadOne(query);
            var columnList = dbService.ReadAll(columnQuery);
            var foreignKeyList = dbService.ReadAll(foreignKeyQuery);

            var columns = new Dictionary<string, Dictionary<string, object>>();
            var primaryKeys = new Dictionary<string, Dictionary<string, object>>();
            var foreignKeys = new Dictionary<string, Dictionary<string, object>>();

            if (queryService is MySQLDatabaseInformationQueryService)
            {
                foreach (var column in columnList)
                {
                    var columnName = column["COLUMN_NAME"].ToString();
                    columns[columnName] = column;
                    if (column["COLUMN_KEY"].ToString() == "PRI")
                    {
                        primaryKeys[columnName] = column;
                    }
                }

                foreach (var column in foreignKeyList)
                {
                    var columnName = column["COLUMN_NAME"].ToString();
                    foreignKeys[columnName] = column;
                }
            }
            else
            {
                foreach (var column in columnList)
                {
                    var columnName = column["COLUMN_NAME"].ToString();
                    columns[columnName] = column;
                }

                var primaryKeyQuery = queryService.GetPrimaryKeys(databaseName, tableName);
                var primaryKeyList = dbService.ReadAll(primaryKeyQuery);

                foreach (var column in primaryKeyList)
                {
                    var columnName = column["COLUMN_NAME"].ToString();
                    primaryKeys[columnName] = column;
                }

                foreach (var column in foreignKeyList)
                {
                    var columnName = column["COLUMN_NAME"].ToString();
                    foreignKeys[columnName] = column;
                }
            }

            dbService.Close();

            return new DatabaseTable(tableName, table, columns, primaryKeys, foreignKeys);
        }
    }

}
