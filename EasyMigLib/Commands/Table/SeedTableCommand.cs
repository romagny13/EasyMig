using EasyMigLib.Query;
using System.Collections.Generic;

namespace EasyMigLib.Commands
{
    public class SeedTableCommand : DatabaseCommand
    {
        public string TableName { get; protected set; }
        internal List<SeedRwoCommand> seedRowCommands;

        public int RowCount => this.seedRowCommands.Count;

        public SeedTableCommand(string tableName)
        {
            this.TableName = tableName;
            this.seedRowCommands = new List<SeedRwoCommand>();
        }

        public SeedTableCommand Insert(Dictionary<string, object> columnValues)
        {
            this.seedRowCommands.Add(new SeedRwoCommand(this.TableName, columnValues));
            return this;
        }

        public SeedTableCommand Insert(SeedData seedData)
        {
            return this.Insert(seedData.container);
        }

        public Dictionary<string, object> GetRow(int index)
        {
            if (index >= 0 && index < this.RowCount)
            {
                return this.seedRowCommands[index].columnValues;
            }
            return null;
        }

        public override string GetQuery(IQueryService queryService)
        {
            var result = "";
            foreach (var seedRowCommand in this.seedRowCommands)
            {
                result += seedRowCommand.GetQuery(queryService);
            }
            return result;
        }
    }
}
