using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyMigLib.Db.Common
{
    public interface IDbService
    {
        DbConnection Connection { get; }

        void Close();
        IDbService CreateConnection(string connectionString, string providerName);
        int Execute(string sql, List<DbServiceParameter> parameters = null);
        Task ExecuteAsync(string sql, List<DbServiceParameter> parameters = null);
        object ExecuteScalar(string sql, List<DbServiceParameter> parameters = null);
        bool IsOpen();
        void Open();
        Task OpenAsync();
        List<Dictionary<string, object>> ReadAll(string sql, List<DbServiceParameter> parameters = null);
        Dictionary<string, object> ReadOne(string sql, List<DbServiceParameter> parameters = null);
    }
}