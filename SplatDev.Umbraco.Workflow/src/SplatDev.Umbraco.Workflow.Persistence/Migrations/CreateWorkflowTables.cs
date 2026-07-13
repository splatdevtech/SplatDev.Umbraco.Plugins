using Umbraco.Cms.Infrastructure.Migrations;

namespace SplatDev.Umbraco.Workflow.Persistence.Migrations;

public class CreateWorkflowTables : MigrationBase
{
    public CreateWorkflowTables(IMigrationContext context) : base(context)
    {
    }

    protected override void Migrate()
    {
        if (!TableExists("splatWorkflowDefinition"))
        {
            Create.Table<Entities.WorkflowDefinitionEntity>().Do();
        }

        if (!TableExists("splatWorkflowInstance"))
        {
            Create.Table<Entities.WorkflowInstanceEntity>().Do();
        }

        if (!TableExists("splatWorkflowEvent"))
        {
            Create.Table<Entities.WorkflowEventEntity>().Do();
        }

        if (!TableExists("splatWorkflowAssignment"))
        {
            Create.Table<Entities.WorkflowAssignmentEntity>().Do();
        }
    }
}
