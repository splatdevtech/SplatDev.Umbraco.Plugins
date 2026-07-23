using FormBuilder.Core.Enums;

using Umbraco.Cms.Core.Mapping;

namespace FormBuilder.Core.Models
{
    /// <summary>Defines mapping profiles for the form designer.</summary>
    public class FormDesignMapping : IMapDefinition
    {
        /// <inheritdoc />
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define((source, context) => [], new Action<FormDesign, List<Workflow>, MapperContext>(Map));
            mapper.Define((source, context) => new Form(), new Action<FormDesign, Form, MapperContext>(Map));
            mapper.Define((source, context) => new FormDesign(), new Action<Form, FormDesign, MapperContext>(Map));
        }

        private void Map(FormDesign source, List<Workflow> target, MapperContext context)
        {
            if (source.FormWorkflows.OnSubmit is null)
                source.FormWorkflows.OnSubmit = [];
            if (source.FormWorkflows.OnApprove is null)
                source.FormWorkflows.OnApprove = [];
            if (source.FormWorkflows.OnReject is null)
                source.FormWorkflows.OnReject = [];
            target.AddRange(Workflows(source.FormWorkflows.OnSubmit, FormState.Submitted, source.Id));
            target.AddRange(Workflows(source.FormWorkflows.OnApprove, FormState.Approved, source.Id));
            target.AddRange(Workflows(source.FormWorkflows.OnReject, FormState.Rejected, source.Id));
        }

        private static IEnumerable<Workflow> Workflows(
          IEnumerable<FormWorkflowWithTypeSettings> workflowWithTypeSettings,
          FormState state,
          Guid formId)
        {
            return workflowWithTypeSettings.Select(workflow => new Workflow()
            {
                Name = workflow.Name,
                Active = workflow.Active,
                IncludeSensitiveData = workflow.IncludeSensitiveData,
                ExecutesOn = state,
                Form = formId,
                WorkflowTypeId = workflow.WorkflowTypeId,
                SortOrder = workflow.SortOrder,
                Deleted = workflow.IsDeleted,
                Id = workflow.Id == Guid.Empty ? Guid.NewGuid() : workflow.Id,
                Settings = workflow.Settings.ToDictionary(k => k.Key, v => v.Value, StringComparer.OrdinalIgnoreCase),
                IsMandatory = workflow.IsMandatory,
                Condition = workflow.Condition
            });
        }

        private void Map(FormDesign source, Form target, MapperContext context)
        {
            target.Name = source.Name;
            target.Created = source.Created;
            target.CreatedBy = source.CreatedBy;
            target.CreatedByName = source.CreatedByName;
            target.Updated = source.Updated;
            target.UpdatedBy = source.UpdatedBy;
            target.UpdatedByName = source.UpdatedByName;
            target.Pages = source.Pages;
            target.Id = source.Id;
            target.FieldIndicationType = source.FieldIndicationType;
            target.Indicator = source.Indicator;
            target.ShowValidationSummary = source.ShowValidationSummary;
            target.HideFieldValidation = source.HideFieldValidation;
            target.RequiredErrorMessage = source.RequiredErrorMessage;
            target.InvalidErrorMessage = source.InvalidErrorMessage;
            target.MessageOnSubmit = source.MessageOnSubmit;
            target.MessageOnSubmitIsHtml = source.MessageOnSubmitIsHtml;
            target.GoToPageOnSubmit = source.GoToPageOnSubmit;
            target.XPathOnSubmit = source.XPathOnSubmit;
            target.ManualApproval = source.ManualApproval;
            target.StoreRecordsLocally = source.StoreRecordsLocally;
            target.AutocompleteAttribute = source.AutocompleteAttribute;
            target.DaysToRetainSubmittedRecordsFor = source.DaysToRetainSubmittedRecordsFor;
            target.DaysToRetainApprovedRecordsFor = source.DaysToRetainApprovedRecordsFor;
            target.DaysToRetainRejectedRecordsFor = source.DaysToRetainRejectedRecordsFor;
            target.DisplayDefaultFields = source.DisplayDefaultFields;
            target.SelectedDisplayFields = source.SelectedDisplayFields;
            target.CssClass = source.CssClass;
            target.DisableDefaultStylesheet = source.DisableDefaultStylesheet;
            target.DataSource = source.DataSource;
            target.SubmitLabel = source.SubmitLabel;
            target.NextLabel = source.NextLabel;
            target.PrevLabel = source.PrevLabel;
            target.FolderId = source.FolderId;
            target.NodeId = source.NodeId;
            target.ShowPagingOnMultiPageForms = source.ShowPagingOnMultiPageForms;
            target.PagingDetailsFormat = source.PagingDetailsFormat;
            target.PageCaptionFormat = source.PageCaptionFormat;
            target.ShowSummaryPageOnMultiPageForms = source.ShowSummaryPageOnMultiPageForms;
            target.SummaryLabel = source.SummaryLabel;
            target.ValidationRules = source.ValidationRules;
            target.EnsureFormStructureIds();
        }

        private void Map(Form source, FormDesign target, MapperContext context)
        {
            target.Name = source.Name;
            target.Created = source.Created;
            target.CreatedBy = source.CreatedBy;
            target.CreatedByName = source.CreatedByName;
            target.Updated = source.Updated;
            target.UpdatedBy = source.UpdatedBy;
            target.UpdatedByName = source.UpdatedByName;
            target.Pages = source.Pages;
            target.Id = source.Id;
            target.FieldIndicationType = source.FieldIndicationType;
            target.Indicator = source.Indicator;
            target.ShowValidationSummary = source.ShowValidationSummary;
            target.HideFieldValidation = source.HideFieldValidation;
            target.RequiredErrorMessage = source.RequiredErrorMessage;
            target.InvalidErrorMessage = source.InvalidErrorMessage;
            target.MessageOnSubmit = source.MessageOnSubmit;
            target.MessageOnSubmitIsHtml = source.MessageOnSubmitIsHtml;
            target.GoToPageOnSubmit = source.GoToPageOnSubmit;
            target.XPathOnSubmit = source.XPathOnSubmit;
            target.ManualApproval = source.ManualApproval;
            target.AutocompleteAttribute = source.AutocompleteAttribute;
            target.DaysToRetainSubmittedRecordsFor = source.DaysToRetainSubmittedRecordsFor;
            target.DaysToRetainApprovedRecordsFor = source.DaysToRetainApprovedRecordsFor;
            target.DaysToRetainRejectedRecordsFor = source.DaysToRetainRejectedRecordsFor;
            target.DisplayDefaultFields = source.DisplayDefaultFields;
            target.SelectedDisplayFields = source.SelectedDisplayFields;
            target.StoreRecordsLocally = source.StoreRecordsLocally;
            target.CssClass = source.CssClass;
            target.DisableDefaultStylesheet = source.DisableDefaultStylesheet;
            target.DataSource = source.DataSource;
            target.SubmitLabel = source.SubmitLabel;
            target.NextLabel = source.NextLabel;
            target.PrevLabel = source.PrevLabel;
            target.FolderId = source.FolderId;
            target.NodeId = source.NodeId;
            target.ShowPagingOnMultiPageForms = source.ShowPagingOnMultiPageForms;
            target.PagingDetailsFormat = source.PagingDetailsFormat;
            target.PageCaptionFormat = source.PageCaptionFormat;
            target.ShowSummaryPageOnMultiPageForms = source.ShowSummaryPageOnMultiPageForms;
            target.SummaryLabel = source.SummaryLabel;
            target.ValidationRules = source.ValidationRules;
            target.EnsureFormStructureIds();
        }
    }
}