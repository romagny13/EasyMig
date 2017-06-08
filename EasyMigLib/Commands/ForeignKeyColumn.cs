namespace EasyMigLib.Commands
{
    public class ForeignKeyColumn : MigrationColumn
    {
        public string TableReferenced { get; }
        public string PrimaryKeyReferenced { get; }

        public ForeignKeyColumn(
            string columnName,
            ColumnType columnType,
            string tableReferenced,
            string primaryKeyReferenced,
            bool nullable = false,
            object defaultValue = null)
            : base(columnName, columnType, nullable, defaultValue)
        {
            this.TableReferenced = tableReferenced;
            this.PrimaryKeyReferenced = primaryKeyReferenced;
        }
    }
}
