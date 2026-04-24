
// Type: Umbraco.Forms.Web.Models.Mapping.FormDesignMapping
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Models.Mapping
{
  public class FormDesignMapping : IMapDefinition
  {
    public void DefineMaps(IUmbracoMapper mapper)
    {
      mapper.Define<FormDesign, List<Workflow>>((Func<FormDesign, MapperContext, List<Workflow>>) ((source, context) => new List<Workflow>()), new Action<FormDesign, List<Workflow>, MapperContext>(this.Map));
      mapper.Define<FormDesign, Form>((Func<FormDesign, MapperContext, Form>) ((source, context) => new Form()), new Action<FormDesign, Form, MapperContext>(this.Map));
      mapper.Define<Form, FormDesign>((Func<Form, MapperContext, FormDesign>) ((source, context) => new FormDesign()), new Action<Form, FormDesign, MapperContext>(this.Map));
    }

    private void Map(FormDesign source, List<Workflow> target, MapperContext context)
    {
      if (source.FormWorkflows.OnSubmit == null)
        source.FormWorkflows.OnSubmit = new List<FormWorkflowWithTypeSettings>();
      if (source.FormWorkflows.OnApprove == null)
        source.FormWorkflows.OnApprove = new List<FormWorkflowWithTypeSettings>();
      if (source.FormWorkflows.OnReject == null)
        source.FormWorkflows.OnReject = new List<FormWorkflowWithTypeSettings>();
      target.AddRange(this.Workflows((IEnumerable<FormWorkflowWithTypeSettings>) source.FormWorkflows.OnSubmit, FormState.Submitted, source.Id));
      target.AddRange(this.Workflows((IEnumerable<FormWorkflowWithTypeSettings>) source.FormWorkflows.OnApprove, FormState.Approved, source.Id));
      target.AddRange(this.Workflows((IEnumerable<FormWorkflowWithTypeSettings>) source.FormWorkflows.OnReject, FormState.Rejected, source.Id));
    }

    private IEnumerable<Workflow> Workflows(
      IEnumerable<FormWorkflowWithTypeSettings> workflowWithTypeSettings,
      FormState state,
      Guid formId)
    {
      return workflowWithTypeSettings.Select<FormWorkflowWithTypeSettings, Workflow>((Func<FormWorkflowWithTypeSettings, Workflow>) (workflow => new Workflow()
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
        Settings = workflow.Settings.ToDictionary<KeyValuePair<string, string>, string, string>((Func<KeyValuePair<string, string>, string>) (k => k.Key), (Func<KeyValuePair<string, string>, string>) (v => v.Value), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase),
        IsMandatory = workflow.IsMandatory,
        Condition = workflow.Condition
      }));
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
