using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace EasyMigLib.Services
{
    public interface IDbService
    {
        DbConnection Connection { get; }

        void Close();
        IDbService CreateConnection(string connectionString, string providerName);
        int Execute(string sql);
        Task ExecuteAsync(string sql);
        bool IsOpen();
        void Open();
        Task OpenAsync();
        Dictionary<string, object> ReadOne(string sql);
        object ExecuteScalar(string sql, Dictionary<string, object> parameters = null);
        List<Dictionary<string, object>> ReadAll(string sql);
    }
}