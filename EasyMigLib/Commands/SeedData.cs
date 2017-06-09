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

        public bool Has(string columnName)
        {
            return this.container.ContainsKey(columnName);
        }

        public object Get(string columnName)
        {
            if (!this.Has(columnName)) { throw new EasyMigException("No Column " + columnName + " defined"); }

           return this.container[columnName];
        }

        public SeedData Set(string columnName, object value)
        {
            if (this.Has(columnName)) { throw new EasyMigException("Column " + columnName + " already defined"); }

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
