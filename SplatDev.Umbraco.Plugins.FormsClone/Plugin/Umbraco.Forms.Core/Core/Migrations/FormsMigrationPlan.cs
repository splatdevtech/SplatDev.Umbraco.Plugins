
// Type: Umbraco.Forms.Core.Migrations.FormsMigrationPlan
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.Packaging;
using Umbraco.Cms.Infrastructure.Migrations;
using Umbraco.Forms.Core.Migrations.V_10_0_0;
using Umbraco.Forms.Core.Migrations.V_10_1_0;
using Umbraco.Forms.Core.Migrations.V_10_5_2;
using Umbraco.Forms.Core.Migrations.V_12_2_0;
using Umbraco.Forms.Core.Migrations.V_12_2_1;
using Umbraco.Forms.Core.Migrations.V_13_2_0;
using Umbraco.Forms.Core.Migrations.V_13_3_0;
using Umbraco.Forms.Core.Migrations.V_13_4_0;


#nullable enable
namespace Umbraco.Forms.Core.Migrations
{
  public class FormsMigrationPlan : PackageMigrationPlan
  {
    public FormsMigrationPlan()
      : base("Umbraco.Forms", "Umbraco Forms", "UmbracoForms")
    {
    }

    public override string InitialState => "{forms-init-state}";

    public override bool IgnoreCurrentState => false;

    protected override void DefinePlan() => ((MigrationPlan) this).From("{forms-init-state}").To<InstallUmbracoForms>("7c7bc5ee-4c5b-42dc-9576-5ce6dfbddb8e").To<AddRecordCultureColumn>("9f7e6fe6-bbd5-4b2b-8820-e9e0e36cc74c").To<AddRecordWorkflowAudit>("1a8f0d04-9396-40a2-9423-39fc9ae3828f").To<AddRecordWorkflowAuditExecutionStage>("6e692c5d-c670-4c34-af17-28d8dbf0dcd2").To<NoopMigration>("5d84fee1-388c-4e5f-b98c-1e66947278f1").To<EnsureIndexOnFormFolderKey>("22df962a-ae26-4bdd-b8fd-0513a9c636bf").To<AddRelationTypeBetweenContentAndForms>("c3e657f6-3ae7-4ee9-b442-01702a41de9a").To<AddFormNodeIdColumn>("e0290a40-91c9-4acb-a7ca-d312037078f2").To<PopulateNodeTableAndNodeIdToForms>("6f0eb771-6690-4b53-870a-f7dbb2785cac").To<NoopMigration>("44949e12-e4ef-42c0-949b-67286b946fe0").To<EnsureKeyForRelationTypeBetweenContentAndForms>("773ae769-00b7-4429-b7d5-de0fda0b4217").To<AddRecordAdditionalDataColumn>("55d53d2e-f795-42fb-9e77-8edfc6eed4aa").To<AddDeleteEntriesPermissions>("1fff8b7b-48e7-450a-80b1-7df628508b27").To<AddCreatedAndUpdatedByToEntities>("7e170195-cab7-48ca-98c7-bbcbd5cfda95").To<AddRecordAdditionalDataColumn>("a5ffa9a7-ca77-4a7c-a1e4-f32e25cde758").To<AddDeleteEntriesPermissions>("db5ef50d-51d0-4f93-aae9-bd3df53a3bb1").To<AddCreatedAndUpdatedByToEntities>("5b74ad79-3faa-4c08-bfba-472a860704e5");
  }
}
