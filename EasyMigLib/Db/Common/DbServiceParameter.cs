using System.Data;

namespace EasyMigLib.Db.Common
{
    public class DbServiceParameter
    {
        public string ParameterName { get; set;  }
        public DbType? ParameterType { get; set; }
        public ParameterDirection? Direction { get; set; }
        public object Value { get; set; }
    }
}
