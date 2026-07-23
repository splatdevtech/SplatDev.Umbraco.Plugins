using Umbraco.Cms.Core.Packaging;

namespace FormBuilder.Core.Migrations
{
    public class FormBuilderMigrationPlan : PackageMigrationPlan
    {
        public FormBuilderMigrationPlan()
          : base("Form.Builder", "Form Builder", "FormBuilder")
        {
        }

        public override string InitialState => "{formbuilder-init-state}";

        public override bool IgnoreCurrentState => false;

        protected override void DefinePlan() => From(InitialState)
            .To<InstallFormBuilder>(Constants.System.MigrationPlanName);
    }
}