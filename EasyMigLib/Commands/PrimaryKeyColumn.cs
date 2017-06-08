namespace EasyMigLib.Commands
{
    public class PrimaryKeyColumn : MigrationColumn
    {
        public bool AutoIncrement { get; }

        public PrimaryKeyColumn(string columnName, ColumnType columnType, bool autoIncrement)
            :base(columnName, columnType, false, null)
        {
            this.AutoIncrement = autoIncrement;
        }
    }
}
