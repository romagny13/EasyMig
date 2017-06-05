using System;

namespace EasyMigLib.Services
{
    public class QueryServiceFactories
    {
        public static QueryService GetService(string providerName, string engine = null)
        {
            if (providerName == "System.Data.SqlClient")
            {
                return new SqlQueryService(engine);
            }
            else if (providerName == "MySql.Data.MySqlClient")
            {
                return new MySQLQueryService(engine);
            }
            else
            {
                throw new Exception("Provider " + providerName.ToString() + " not supported");
            }
        }
    }
}
