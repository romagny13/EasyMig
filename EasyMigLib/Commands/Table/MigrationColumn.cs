namespace EasyMigLib.Commands
{
    public class MigrationColumn
    {
        public string ColumnName { get; protected set; }
        public ColumnType ColumnType { get; protected set; }

        public bool Nullable { get; protected set; }
        public bool Unique { get; protected set; }
        public object DefaultValue { get; protected set; }

        public MigrationColumn(string columnName, ColumnType columnType, bool nullable, object defaultValue = null, bool unique = false)
        {
            this.ColumnName = columnName;
            this.ColumnType = columnType;
            this.Nullable = nullable;
            this.DefaultValue = defaultValue;
            this.Unique = unique;
        }
    }
}
