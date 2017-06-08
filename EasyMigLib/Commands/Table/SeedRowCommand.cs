using EasyMigLib.Query;
using System.Collections.Generic;

namespace EasyMigLib.Commands
{
    public class SeedRwoCommand
    {
        public string TableName { get; protected set; }
        internal Dictionary<string, object> columnValues;

        public SeedRwoCommand(string tableName, Dictionary<string, object> columnValues)
        {
            this.TableName = tableName;
            this.columnValues = columnValues;
        }

        public string GetQuery(IQueryService queryService)
        {
            return queryService.GetSeedRow(this.TableName, this.columnValues);
        }
    }
}
