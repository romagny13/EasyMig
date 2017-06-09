using EasyMigLib.Query;

namespace EasyMigLib.Commands
{
    public class DropStoredProcedureCommand : DatabaseCommand
    {
        public string ProcedureName { get; protected set; }
 
        public DropStoredProcedureCommand(string procedureName)
        {
            this.ProcedureName = procedureName;
        }

        public override string GetQuery(IQueryService queryService)
        {
            return queryService.GetDropStoredProcedure(this.ProcedureName);
        }
    }
}
