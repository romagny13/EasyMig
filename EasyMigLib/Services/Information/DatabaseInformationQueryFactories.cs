using System;
using System.Text.RegularExpressions;

namespace EasyMigLib.Services
{
    public class DatabaseInformationQueryFactories
    {
        public static DatabaseInformationQueryService GetService(string providerName)
        {
            if (providerName == "System.Data.SqlClient")
            {
                return new SqlDatabaseInformationQueryService();
            }
            else if (providerName == "MySql.Data.MySqlClient")
            {
                return new MySQLDatabaseInformationQueryService();
            }
            else
            {
                throw new Exception("Provider " + providerName.ToString() + " not supported");
            }
        }
    }

}
