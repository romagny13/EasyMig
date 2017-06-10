namespace EasyMigLib.Schema
{
    public class ForeignKeyColumn : MigrationColumn
    {
        public string TableReferenced { get; }
        public string PrimaryKeyReferenced { get; }

        public ForeignKeyColumn(
            string tableName,
            string columnName,
            ColumnType columnType,
            string tableReferenced,
            string primaryKeyReferenced,
            bool nullable = false,
            object defaultValue = null)
            : base(tableName, columnName, columnType, nullable, defaultValue)
        {
            this.TableReferenced = tableReferenced;
            this.PrimaryKeyReferenced = primaryKeyReferenced;
        }
    }
}
