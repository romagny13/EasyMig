using System.Collections.Generic;

namespace EasyMigLib.Schema
{
    public class SeedRow
    {
        public string TableName { get; protected set; }
        internal Dictionary<string, object> columnValues;

        public SeedRow(string tableName, Dictionary<string, object> columnValues)
        {
            this.TableName = tableName;
            this.columnValues = columnValues;
        }
    }
}
