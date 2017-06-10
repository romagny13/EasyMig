namespace EasyMigLib.Schema
{
    public class PrimaryKeyColumn : MigrationColumn
    {
        public bool AutoIncrement { get; }

        public PrimaryKeyColumn(
            string tableName, 
            string columnName, 
            ColumnType columnType, 
            bool autoIncrement)
            : base(tableName, columnName, columnType, false, null)
        {
            this.AutoIncrement = autoIncrement;
        }
    }
}
