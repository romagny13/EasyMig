using System.Collections.Generic;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace EasyMigLib.Db.MySqlClient
{
    public interface IMySqlDbService
    {
        MySqlConnection Connection { get; }

        void Close();
        IMySqlDbService CreateConnection(string connectionString);
        int Execute(string sql, List<MySqlParameter> parameters = null);
        Task ExecuteAsync(string sql, List<MySqlParameter> parameters = null);
        object ExecuteScalar(string sql, List<MySqlParameter> parameters = null);
        bool IsOpen();
        void Open();
        Task OpenAsync();
        List<Dictionary<string, object>> ReadAll(string sql, List<MySqlParameter> parameters = null);
        Dictionary<string, object> ReadOne(string sql, List<MySqlParameter> parameters = null);
    }
}