namespace EasyMigLib.Schema
{
    public class MigrationColumn
    {
        public string TableName { get; }
        public string ColumnName { get;  }
        public ColumnType ColumnType { get;  }

        public bool Nullable { get;  }
        public bool Unique { get;  }
        public object DefaultValue { get;  }

        public MigrationColumn(
            string tableName,
            string columnName, 
            ColumnType columnType, 
            bool nullable, 
            object defaultValue = null, 
            bool unique = false)
        {
            this.TableName = tableName;
            this.ColumnName = columnName;
            this.ColumnType = columnType;
            this.Nullable = nullable;
            this.DefaultValue = defaultValue;
            this.Unique = unique;
        }
    }
}
