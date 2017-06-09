using System.Linq;
using System.Text.RegularExpressions;

namespace EasyMigLib.Db.SqlClient
{
    public class SqlQuerySplitter : IQuerySplitter
    {
        public string[] SplitQuery(string query)
        {
            // split on "GO"
            var regex = new Regex("\\s+GO\\s*", RegexOptions.Multiline);
            return regex.Split(query).Where((s) => !string.IsNullOrWhiteSpace(s)).ToArray();
        }
    }
}
