namespace EasyMigLib.Db
{
    public interface IQuerySplitter
    {
        string[] SplitQuery(string query);
    }
}