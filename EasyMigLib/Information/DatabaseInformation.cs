using EasyMigLib.Information.MySqlClient;
using EasyMigLib.Information.SqlClient;
using EasyMigLib.Information.SqlClientAttachedDbFile;

namespace EasyMigLib.Information
{
    public class DatabaseInformation
    {
        public MySqlDatabaseInformation MySql { get; }
        public SqlServerDatabaseInformation SqlServer { get; }
        public SqlServerAttachedDbFileInformation SqlServerAttachedDbFile { get; }

        public DatabaseInformation()
        {
            this.MySql = new MySqlDatabaseInformation();
            this.SqlServer = new SqlServerDatabaseInformation();
            this.SqlServerAttachedDbFile = new SqlServerAttachedDbFileInformation();
        }
    }
}
