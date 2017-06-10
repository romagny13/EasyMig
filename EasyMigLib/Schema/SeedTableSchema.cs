using System.Collections.Generic;

namespace EasyMigLib.Schema
{
    public class SeedTableSchema
    {
        public string TableName { get; protected set; }
        internal List<SeedRow> rows;

        public bool HasRows => this.RowCount > 0;

        public int RowCount => this.rows.Count;

        public SeedTableSchema(string tableName)
        {
            this.TableName = tableName;
            this.rows = new List<SeedRow>();
        }

        public SeedTableSchema Insert(Dictionary<string, object> columnValues)
        {
            this.rows.Add(new SeedRow(this.TableName, columnValues));
            return this;
        }

        public SeedTableSchema Insert(SeedData seedData)
        {
            return this.Insert(seedData.container);
        }

        public Dictionary<string, object> GetRow(int index)
        {
            if (index >= 0 && index < this.RowCount)
            {
                return this.rows[index].columnValues;
            }
            return null;
        }
    }
}
