using FormBuilder.Core.Models;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Core.Mapping;

namespace FormBuilder.Core.Mapping
{
    internal sealed class FormMapDefinition : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define((source, context) => new FormEntity(), new Action<Form, FormEntity, MapperContext>(Map));
            mapper.Define((source, context) => new Form(), new Action<FormEntity, Form, MapperContext>(Map));
            mapper.Define((source, context) => new FormEntitySlim(), new Action<FormSlim, FormEntitySlim, MapperContext>(Map));
            mapper.Define((source, context) => new FormSlim(), new Action<FormEntitySlim, FormSlim, MapperContext>(Map));
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