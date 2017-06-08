namespace EasyMigLib.Information.MySqlClient
{
    public class MySqlDatabaseInformationQueryService : IMySqlDatabaseInformationQueryService
    {
        public string GetDatabaseExists(string databaseName)
        {
            return "SELECT COUNT(*) FROM INFORMATION_SCHEMA.SCHEMATA WHERE SCHEMA_NAME='" + databaseName + "'";
        }

        public string GetTableExists(string databaseName, string tableName)
        {
            return "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = '" + databaseName + "' AND table_name = '" + tableName + "'";
        }

        public string GetColumnExists(string databaseName, string tableName, string columnName)
        {
            return "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '" + databaseName + "' AND TABLE_NAME = '" + tableName + "' AND COLUMN_NAME = '" + columnName + "'";
        }

        public string GetTable(string databaseName, string tableName)
        {
            return "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE table_schema = '" + databaseName + "' AND table_name = '" + tableName + "'";
        }

        public string GetColumns(string databaseName, string tableName)
        {
            return "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_schema = '" + databaseName + "' AND table_name = '" + tableName + "'";
        }

        public string GetPrimaryKeys(string databaseName, string tableName)
        {
            return "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_schema = '" + databaseName + "' AND table_name = '" + tableName + "' and COLUMN_KEY='PRI'";
        }

        public string GetForeignKeys(string databaseName, string tableName)
        {
            return "SELECT table_referenced.TABLE_NAME, base_table.COLUMN_NAME, table_referenced.CONSTRAINT_TYPE, table_referenced.CONSTRAINT_NAME, base_table.REFERENCED_TABLE_NAME, base_table.REFERENCED_COLUMN_NAME FROM information_schema.TABLE_CONSTRAINTS table_referenced LEFT JOIN information_schema.KEY_COLUMN_USAGE base_table ON table_referenced.CONSTRAINT_NAME = base_table.CONSTRAINT_NAME WHERE table_referenced.CONSTRAINT_TYPE = 'FOREIGN KEY' AND table_referenced.TABLE_SCHEMA = '" + databaseName + "' AND table_referenced.TABLE_NAME = '" + tableName + "'; ";
        }

        public string GetProcedureExists(string databaseName, string procedureName)
        {
            return "SELECT count(*) FROM INFORMATION_SCHEMA.ROUTINES WHERE ROUTINE_TYPE='PROCEDURE' AND ROUTINE_SCHEMA='" + databaseName + "' AND ROUTINE_NAME='" + procedureName + "'";
        }

        public string GetTableRows(string tableName, int? limit)
        {
            if (limit.HasValue)
            {
                return "SELECT * FROM `" + tableName + "` LIMIT " + limit;
            }
            else
            {
                return "SELECT * FROM `" + tableName + "`";
            }
        }

    }

}
