
// Type: Umbraco.Forms.Core.Models.Mapping.FormMapDefinition
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Mapping;


#nullable enable
namespace Umbraco.Forms.Core.Models.Mapping
{
  internal sealed class FormMapDefinition : IMapDefinition
  {
    public void DefineMaps(IUmbracoMapper mapper)
    {
      mapper.Define<Form, FormEntity>((Func<Form, MapperContext, FormEntity>) ((source, context) => new FormEntity()), new Action<Form, FormEntity, MapperContext>(this.Map));
      mapper.Define<FormEntity, Form>((Func<FormEntity, MapperContext, Form>) ((source, context) => new Form()), new Action<FormEntity, Form, MapperContext>(this.Map));
      mapper.Define<FormSlim, FormEntitySlim>((Func<FormSlim, MapperContext, FormEntitySlim>) ((source, context) => new FormEntitySlim()), new Action<FormSlim, FormEntitySlim, MapperContext>(this.Map));
      mapper.Define<FormEntitySlim, FormSlim>((Func<FormEntitySlim, MapperContext, FormSlim>) ((source, context) => new FormSlim()), new Action<FormEntitySlim, FormSlim, MapperContext>(this.Map));
    }

    internal void Map(Form source, FormEntity target, MapperContext context)
    {
      target.AutocompleteAttribute = source.AutocompleteAttribute;
      target.CssClass = source.CssClass;
      target.DataSource = source.DataSource;
      target.DaysToRetainSubmittedRecordsFor = source.DaysToRetainSubmittedRecordsFor;
      target.DaysToRetainApprovedRecordsFor = source.DaysToRetainApprovedRecordsFor;
      target.DaysToRetainRejectedRecordsFor = source.DaysToRetainRejectedRecordsFor;
      target.DisplayDefaultFields = source.DisplayDefaultFields;
      target.SelectedDisplayFields = source.SelectedDisplayFields;
      target.FieldIndicationType = source.FieldIndicationType;
      target.DisableDefaultStylesheet = source.DisableDefaultStylesheet;
      target.GoToPageOnSubmit = source.GoToPageOnSubmit;
      target.HideFieldValidation = source.HideFieldValidation;
      target.Indicator = source.Indicator;
      target.InvalidErrorMessage = source.InvalidErrorMessage;
      target.ManualApproval = source.ManualApproval;
      target.MessageOnSubmit = source.MessageOnSubmit;
      target.MessageOnSubmitIsHtml = source.MessageOnSubmitIsHtml;
      target.NextLabel = source.NextLabel;
      target.PagingDetailsFormat = source.PagingDetailsFormat;
      target.PageCaptionFormat = source.PageCaptionFormat;
      target.Pages = source.Pages;
      target.PrevLabel = source.PrevLabel;
      target.RequiredErrorMessage = source.RequiredErrorMessage;
      target.ShowPagingOnMultiPageForms = source.ShowPagingOnMultiPageForms;
      target.ShowSummaryPageOnMultiPageForms = source.ShowSummaryPageOnMultiPageForms;
      target.ShowValidationSummary = source.ShowValidationSummary;
      target.StoreRecordsLocally = source.StoreRecordsLocally;
      target.SubmitLabel = source.SubmitLabel;
      target.SummaryLabel = source.SummaryLabel;
      target.ValidationRules = source.ValidationRules;
      target.XPathOnSubmit = source.XPathOnSubmit;
      target.CreateDate = source.Created;
      target.CreatedBy = source.CreatedBy;
      target.UpdateDate = source.Updated;
      target.UpdatedBy = source.UpdatedBy;
      target.Name = source.Name;
      target.Key = source.Id;
      target.FolderId = source.FolderId;
      target.NodeId = source.NodeId;
    }

    [ExcludeFromCodeCoverage]
    internal void Map(FormEntity source, Form target, MapperContext context)
    {
      target.AutocompleteAttribute = source.AutocompleteAttribute;
      target.CssClass = source.CssClass;
      target.DataSource = source.DataSource;
      target.DaysToRetainSubmittedRecordsFor = source.DaysToRetainSubmittedRecordsFor;
      target.DaysToRetainApprovedRecordsFor = source.DaysToRetainApprovedRecordsFor;
      target.DaysToRetainRejectedRecordsFor = source.DaysToRetainRejectedRecordsFor;
      target.DisplayDefaultFields = source.DisplayDefaultFields;
      target.SelectedDisplayFields = source.SelectedDisplayFields;
      target.FieldIndicationType = source.FieldIndicationType;
      target.DisableDefaultStylesheet = source.DisableDefaultStylesheet;
      target.GoToPageOnSubmit = source.GoToPageOnSubmit;
      target.HideFieldValidation = source.HideFieldValidation;
      target.Indicator = source.Indicator;
      target.InvalidErrorMessage = source.InvalidErrorMessage;
      target.ManualApproval = source.ManualApproval;
      target.MessageOnSubmit = source.MessageOnSubmit;
      target.MessageOnSubmitIsHtml = source.MessageOnSubmitIsHtml;
      target.NextLabel = source.NextLabel;
      target.PagingDetailsFormat = source.PagingDetailsFormat;
      target.PageCaptionFormat = source.PageCaptionFormat;
      target.Pages = source.Pages;
      target.PrevLabel = source.PrevLabel;
      target.RequiredErrorMessage = source.RequiredErrorMessage;
      target.ShowPagingOnMultiPageForms = source.ShowPagingOnMultiPageForms;
      target.ShowSummaryPageOnMultiPageForms = source.ShowSummaryPageOnMultiPageForms;
      target.ShowValidationSummary = source.ShowValidationSummary;
      target.StoreRecordsLocally = source.StoreRecordsLocally;
      target.SubmitLabel = source.SubmitLabel;
      target.SummaryLabel = source.SummaryLabel;
      target.ValidationRules = source.ValidationRules;
      target.XPathOnSubmit = source.XPathOnSubmit;
      target.Created = source.CreateDate;
      target.CreatedBy = source.CreatedBy;
      target.Updated = source.UpdateDate;
      target.UpdatedBy = source.UpdatedBy;
      target.Name = source.Name;
      target.Id = source.Key;
      target.FolderId = source.FolderId;
      target.NodeId = source.NodeId;
    }

    [ExcludeFromCodeCoverage]
    internal void Map(FormSlim source, FormEntitySlim target, MapperContext context)
    {
      target.CreateDate = source.Created;
      target.Name = source.Name;
      target.Key = source.Id;
      target.FolderId = source.FolderId;
    }

    [ExcludeFromCodeCoverage]
    internal void Map(FormEntitySlim source, FormSlim target, MapperContext context)
    {
      target.Created = source.CreateDate;
      target.Name = source.Name;
      target.Id = source.Key;
      target.FolderId = source.FolderId;
    }
  }
}
