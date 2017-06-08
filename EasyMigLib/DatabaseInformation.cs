using EasyMigLib.Information.MySqlClient;
using EasyMigLib.Information.SqlClient;
using EasyMigLib.Information.SqlClientAttachedDbFile;

namespace EasyMigLib
{
    public class DatabaseInformation
    {
        public MySqlInformation MySql { get; }
        public SqlInformation SqlServer { get; }
        public SqlFileInformation SqlServerAttachedDbFile { get; }

        public DatabaseInformation()
        {
            this.MySql = new MySqlInformation();
            this.SqlServer = new SqlInformation();
            this.SqlServerAttachedDbFile = new SqlFileInformation();
        }
    }
}
