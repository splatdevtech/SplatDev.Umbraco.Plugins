using FormBuilder.Core.Enums;
using FormBuilder.Core.Interfaces;

using Umbraco.Cms.Core.Models.DeliveryApi;

namespace FormBuilder.Core.Dto
{
    public class FormDto : IPostSubmissionDetail
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Indicator { get; set; } = string.Empty;

        public string? CssClass { get; set; }

        public string? NextLabel { get; set; }

        public string? PreviousLabel { get; set; }

        public string? SubmitLabel { get; set; }

        public bool DisableDefaultStylesheet { get; set; }

        public FormFieldIndication FieldIndicationType { get; set; }

        public bool HideFieldValidation { get; set; }

        public string? MessageOnSubmit { get; set; }

        public bool MessageOnSubmitIsHtml { get; set; }

        public bool ShowValidationSummary { get; set; }

        public Guid? GotoPageOnSubmit { get; set; }

        public IApiContentRoute? GotoPageOnSubmitRoute { get; set; }

        public IEnumerable<FormPageDto> Pages { get; set; } = [];

        public IEnumerable<FormValidationRuleDto> ValidationRules { get; set; } = [];
    }
}