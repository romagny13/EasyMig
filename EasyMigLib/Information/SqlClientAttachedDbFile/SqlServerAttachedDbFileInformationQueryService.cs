namespace EasyMigLib.Information.SqlClientAttachedDbFile
{
    public class SqlServerAttachedDbFileInformationQueryService : ISqlServerAttachedDbFileInformationQueryService
    {

        public string GetTableExists(string tableName)
        {
            return "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" + tableName + "'";
        }

        public string GetColumnExists(string tableName, string columnName)
        {
            return "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='" + tableName + "' AND COLUMN_NAME='" + columnName + "'";
        }

        public string GetProcedureExists(string procedureName)
        {
            return "select count(*) from sys.procedures where name='" + procedureName + "'";
        }

        public string GetTable(string tableName)
        {
            return "SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='" + tableName + "'";
        }

        public string GetColumns(string tableName)
        {
            return "SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='" + tableName + "'";
        }

        public string GetPrimaryKeys(string tableName)
        {
            return "SELECT c.* FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS as tc, INFORMATION_SCHEMA.KEY_COLUMN_USAGE as kcu,INFORMATION_SCHEMA.COLUMNS as c WHERE tc.CONSTRAINT_TYPE='PRIMARY KEY' and tc.CONSTRAINT_NAME=kcu.CONSTRAINT_NAME AND kcu.COLUMN_NAME=c.COLUMN_NAME AND kcu.TABLE_NAME=tc.TABLE_NAME AND tc.TABLE_NAME=c.TABLE_NAME AND c.TABLE_NAME='" + tableName + "'";
        }

        public string GetForeignKeys(string tableName)
        {
            return "select o.name as TABLE_NAME, fc.name as COLUMN_NAME, ro.name as TABLE_REFERENCED, c.name as PRIMARY_KEY from sysobjects o join sysforeignkeys fk on fk.fkeyid=o.id join sysobjects ro on ro.id=fk.rkeyid join syscolumns c on c.id=ro.id and c.colid=fk.rkey join syscolumns fc on fc.id=o.id and fc.colid=fk.fkey where o.name='" + tableName + "'";
        }

        public string GetTableRows(string tableName, int? limit)
        {
            if (limit.HasValue)
            {
                return "SELECT TOP " + limit + " * FROM [dbo].[" + tableName + "];";
            }
            else
            {
                return "SELECT * FROM [dbo].[" + tableName + "];";
            }
        }
    }

}
