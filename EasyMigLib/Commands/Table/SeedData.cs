using System;
using System.Collections.Generic;

namespace EasyMigLib.Commands
{
    public class SeedData
    {
        internal Dictionary<string, object> container;

        public SeedData()
        {
            this.container = new Dictionary<string, object>();
        }

        public bool HasColumn(string columnName)
        {
            return this.container.ContainsKey(columnName);
        }

        public SeedData Set(string columnName, object value)
        {
            if (this.HasColumn(columnName)) { throw new Exception("Column " + columnName + " already defined"); }

            this.container[columnName] = value;
            return this;
        }

        public static SeedData New
        {
            get
            {
                return new SeedData();
            }
        }
    }
}
