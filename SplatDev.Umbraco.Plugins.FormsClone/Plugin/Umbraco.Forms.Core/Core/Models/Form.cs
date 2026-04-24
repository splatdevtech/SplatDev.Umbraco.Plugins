
// Type: Umbraco.Forms.Core.Models.Form
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Data.Helpers;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
    [DataContract(Name = "form", Namespace = "")]
    [Serializable]
    public class Form : ITypeWithEditorDetails, IType, ICloneable
    {
        public Form()
          : this(new DefaultFormSettings())
        {
        }

        public Form(DefaultFormSettings defaultSettings)
        {
            this.Pages = new List<Page>();
            this.ValidationRules = new List<ValidationRule>();
            this.SetFromDefaults(defaultSettings);
        }

        protected void SetFromDefaults(DefaultFormSettings defaultSettings)
        {
            if (defaultSettings == null)
                return;
            this.ManualApproval = defaultSettings.ManualApproval;
            this.DisableDefaultStylesheet = defaultSettings.DisableStylesheet;
            this.FieldIndicationType = defaultSettings.MarkFieldsIndicator;
            this.Indicator = defaultSettings.Indicator;
            this.RequiredErrorMessage = defaultSettings.RequiredErrorMessage;
            this.InvalidErrorMessage = defaultSettings.InvalidErrorMessage;
            this.ShowValidationSummary = defaultSettings.ShowValidationSummary;
            this.HideFieldValidation = defaultSettings.HideFieldValidationLabels;
            this.NextLabel = defaultSettings.NextPageButtonLabel;
            this.PrevLabel = defaultSettings.PreviousPageButtonLabel;
            this.SubmitLabel = defaultSettings.SubmitButtonLabel;
            this.MessageOnSubmit = defaultSettings.MessageOnSubmit;
            this.MessageOnSubmitIsHtml = defaultSettings.MessageOnSubmitIsHtml;
            this.StoreRecordsLocally = defaultSettings.StoreRecordsLocally;
            this.AutocompleteAttribute = defaultSettings.AutocompleteAttribute;
            this.DaysToRetainSubmittedRecordsFor = defaultSettings.DaysToRetainSubmittedRecordsFor;
            this.DaysToRetainApprovedRecordsFor = defaultSettings.DaysToRetainApprovedRecordsFor;
            this.DaysToRetainRejectedRecordsFor = defaultSettings.DaysToRetainRejectedRecordsFor;
            this.ShowPagingOnMultiPageForms = defaultSettings.ShowPagingOnMultiPageForms;
            this.PagingDetailsFormat = defaultSettings.PagingDetailsFormat;
            this.PageCaptionFormat = defaultSettings.PageCaptionFormat;
            this.ShowSummaryPageOnMultiPageForms = defaultSettings.ShowSummaryPageOnMultiPageForms;
            this.SummaryLabel = defaultSettings.SummaryLabel;
        }

        [Required]
        [DataMember(Name = "name")]
        public string Name { get; set; } = string.Empty;

        [DataMember(Name = "created")]
        public DateTime Created { get; set; }

        [DataMember(Name = "createdBy")]
        public int? CreatedBy { get; set; }

        [DataMember(Name = "createdByName")]
        public string? CreatedByName { get; set; }

        [DataMember(Name = "updated")]
        public DateTime Updated { get; set; }

        [DataMember(Name = "updatedBy")]
        public int? UpdatedBy { get; set; }

        [DataMember(Name = "updatedByName")]
        public string? UpdatedByName { get; set; }

        [DataMember(Name = "pages")]
        public List<Page> Pages { get; set; }

        [DataMember(Name = "validationRules")]
        public List<ValidationRule> ValidationRules { get; set; }

        [DataMember(Name = "id")]
        public Guid Id { get; set; }

        [DataMember(Name = "unique")]
        public Guid Unique => this.Id;

        [DataMember(Name = "parentUnique")]
        public Guid? ParentUnique => this.FolderId;

        [DataMember(Name = "entityType")]
        public string EntityType => "form";

        [DataMember(Name = "fieldIndicationType")]
        public FormFieldIndication FieldIndicationType { get; set; }

        [DataMember(Name = "indicator")]
        public string Indicator { get; set; } = string.Empty;

        [DataMember(Name = "showValidationSummary")]
        public bool ShowValidationSummary { get; set; }

        [DataMember(Name = "hideFieldValidation")]
        public bool HideFieldValidation { get; set; }

        [DataMember(Name = "requiredErrorMessage")]
        public string RequiredErrorMessage { get; set; } = string.Empty;

        [DataMember(Name = "invalidErrorMessage")]
        public string InvalidErrorMessage { get; set; } = string.Empty;

        [DataMember(Name = "messageOnSubmit")]
        public string? MessageOnSubmit { get; set; }

        [DataMember(Name = "messageOnSubmitIsHtml")]
        public bool MessageOnSubmitIsHtml { get; set; }

        [DataMember(Name = "goToPageOnSubmit")]
        [JsonConverter(typeof(JsonIntToStringConverter))]
        public string? GoToPageOnSubmit { get; set; }

        [DataMember(Name = "xPathOnSubmit")]
        public string? XPathOnSubmit { get; set; }

        [DataMember(Name = "manualApproval")]
        public bool ManualApproval { get; set; }

        [DataMember(Name = "storeRecordsLocally")]
        public bool StoreRecordsLocally { get; set; }

        [DataMember(Name = "autocompleteAttribute")]
        public string? AutocompleteAttribute { get; set; }

        [DataMember(Name = "displayDefaultFields")]
        public bool DisplayDefaultFields { get; set; } = true;

        [DataMember(Name = "selectedDisplayFields")]
        public List<RecordFieldDisplay> SelectedDisplayFields { get; set; } = new List<RecordFieldDisplay>();

        [DataMember(Name = "daysToRetainSubmittedRecordsFor")]
        public int DaysToRetainSubmittedRecordsFor { get; set; }

        [DataMember(Name = "daysToRetainApprovedRecordsFor")]
        public int DaysToRetainApprovedRecordsFor { get; set; }

        [DataMember(Name = "daysToRetainRejectedRecordsFor")]
        public int DaysToRetainRejectedRecordsFor { get; set; }

        [DataMember(Name = "cssClass")]
        public string? CssClass { get; set; }

        [DataMember(Name = "disableDefaultStylesheet")]
        public bool DisableDefaultStylesheet { get; set; }

        [DataMember(Name = "datasource")]
        [JsonPropertyName("datasource")]
        public FormDataSourceDefinition? DataSource { get; set; }

        [DataMember(Name = "submitLabel")]
        public string? SubmitLabel { get; set; }

        [DataMember(Name = "nextLabel")]
        public string? NextLabel { get; set; }

        [DataMember(Name = "prevLabel")]
        public string? PrevLabel { get; set; }

        [DataMember(Name = "folderId")]
        public Guid? FolderId { get; set; }

        [DataMember(Name = "nodeId")]
        public int NodeId { get; set; }

        [DataMember(Name = "showPagingOnMultiPageForms")]
        public MultiPageNavigationOption ShowPagingOnMultiPageForms { get; set; }

        [DataMember(Name = "pagingDetailsFormat")]
        public string PagingDetailsFormat { get; set; } = "Page {0} of {1}";

        [DataMember(Name = "pageCaptionFormat")]
        public string PageCaptionFormat { get; set; } = "Page {0}";

        [DataMember(Name = "showSummaryPageOnMultiPageForms")]
        public bool ShowSummaryPageOnMultiPageForms { get; set; }

        [DataMember(Name = "summaryLabel")]
        public string? SummaryLabel { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        [JsonIgnore]
        public List<FieldSet> AllFieldSets
        {
            get
            {
                List<FieldSet> allFieldSets = new List<FieldSet>();
                foreach (Page page in this.Pages)
                {
                    foreach (FieldSet fieldSet in page.FieldSets)
                        allFieldSets.Add(fieldSet);
                }
                return allFieldSets;
            }
        }

        [XmlIgnore]
        [IgnoreDataMember]
        [JsonIgnore]
        public List<Field> AllFields
        {
            get
            {
                List<Field> allFields = new List<Field>();
                foreach (FieldSet allFieldSet in this.AllFieldSets)
                {
                    foreach (FieldsetContainer container in allFieldSet.Containers)
                    {
                        foreach (Field field in container.Fields)
                            allFields.Add(field);
                    }
                }
                return allFields;
            }
        }

        public string GetFormFieldSummary()
        {
            string[] strArray = new string[10]
            {
        "one",
        "two",
        "three",
        "four",
        "five",
        "six",
        "seven",
        "eight",
        "nine",
        "ten"
            };
            StringBuilder stringBuilder = new StringBuilder();
            if (this.AllFields.Count <= 4)
            {
                int num = 1;
                foreach (Field allField in this.AllFields)
                {
                    stringBuilder.Append(allField.Caption);
                    if (num == this.AllFields.Count - 1)
                        stringBuilder.Append(" and ");
                    else if (num < this.AllFields.Count)
                        stringBuilder.Append(", ");
                    ++num;
                }
            }
            else
            {
                for (int index = 0; index < 3; ++index)
                    stringBuilder.Append(this.AllFields[index].Caption + ", ");
                stringBuilder.Append(this.AllFields[3].Caption + " and ");
                if (this.AllFields.Count > 14)
                {
                    stringBuilder.Append(" and a lot more");
                }
                else
                {
                    stringBuilder.Append(strArray[this.AllFields.Count - 5] + " additional field");
                    if (this.AllFields.Count != 1)
                        stringBuilder.Append("s");
                }
            }
            return stringBuilder.ToString();
        }

        public void SetFieldAliases(IShortStringHelper shortStringHelper)
        {
            var allFields = this.Pages.SelectMany(x => x.AllFields());
            foreach (Field field in allFields)
            {
                if (string.IsNullOrEmpty(field.Alias))
                    field.Alias = this.GenerateFieldAlias(shortStringHelper, field.Caption);
                else continue;
            }
        }

        private string GenerateFieldAlias(IShortStringHelper shortStringHelper, string caption)
        {
            string cleanString = caption.ToCleanString(shortStringHelper, CleanStringType.CamelCase);
            return cleanString.Length > byte.MaxValue ? cleanString.Substring(0, byte.MaxValue) : cleanString;
        }

        public bool HasDataSource() => this.DataSource != null && !(this.DataSource.Id == Guid.Empty);

        public void EnsureFormStructureIds()
        {
            foreach (Page page in this.Pages)
            {
                this.EnsureFormObjectId(page);
                if (page.ButtonCondition != null)
                    this.EnsureFormObjectId(page.ButtonCondition);
                foreach (FieldSet fieldSet in page.FieldSets)
                {
                    this.EnsureFormObjectId(fieldSet);
                    fieldSet.Page = page.Id;
                    if (fieldSet.Condition != null)
                        this.EnsureFormObjectId(fieldSet.Condition);
                    foreach (FieldsetContainer container in fieldSet.Containers)
                    {
                        this.EnsureFormObjectId(container);
                        foreach (Field field in container.Fields)
                        {
                            this.EnsureFormObjectId(field);
                            if (field.Condition != null)
                                this.EnsureFormObjectId(field.Condition);
                        }
                    }
                }
            }
        }

        private void EnsureFormObjectId(IFormObject formObject)
        {
            if (!(formObject.Id == Guid.Empty))
                return;
            formObject.Id = Guid.NewGuid();
        }

        public RegenerateFormStructureIdsResult RegenerateFormStructureIds()
        {
            Dictionary<Guid, Guid> fieldIdMapping = new Dictionary<Guid, Guid>();
            foreach (Page page in this.Pages)
            {
                page.Id = Guid.NewGuid();
                if (page.ButtonCondition != null)
                    page.ButtonCondition.Id = Guid.NewGuid();
                foreach (FieldSet fieldSet in page.FieldSets)
                {
                    fieldSet.Id = Guid.NewGuid();
                    fieldSet.Page = page.Id;
                    if (fieldSet.Condition != null)
                        fieldSet.Condition.Id = Guid.NewGuid();
                    foreach (FieldsetContainer container in fieldSet.Containers)
                    {
                        container.Id = Guid.NewGuid();
                        foreach (Field field in container.Fields)
                        {
                            Guid guid = Guid.NewGuid();
                            fieldIdMapping.Add(field.Id, guid);
                            field.Id = guid;
                            if (field.Condition != null)
                                field.Condition.Id = Guid.NewGuid();
                        }
                    }
                }
            }
            this.ApplyNewFieldIdsToConditions(fieldIdMapping);
            return new RegenerateFormStructureIdsResult(fieldIdMapping);
        }

        private void ApplyNewFieldIdsToConditions(Dictionary<Guid, Guid> fieldIdMapping)
        {
            foreach (Page page in this.Pages)
            {
                if (page.ButtonCondition != null)
                    Form.ApplyNewFieldIdsToConditionRules(fieldIdMapping, page.ButtonCondition);
                foreach (FieldSet fieldSet in page.FieldSets)
                {
                    if (fieldSet.Condition != null)
                        Form.ApplyNewFieldIdsToConditionRules(fieldIdMapping, fieldSet.Condition);
                    foreach (Field field in fieldSet.Containers.SelectMany<FieldsetContainer, Field>(x => x.Fields).Where<Field>(x => x.Condition != null && x.Condition.Rules.Any<FieldConditionRule>()))
                        Form.ApplyNewFieldIdsToConditionRules(fieldIdMapping, field.Condition);
                }
            }
        }

        private static void ApplyNewFieldIdsToConditionRules(
          Dictionary<Guid, Guid> fieldIdMapping,
          FieldCondition condition)
        {
            foreach (FieldConditionRule rule in condition.Rules)
            {
                if (fieldIdMapping.ContainsKey(rule.Field))
                    rule.Field = fieldIdMapping[rule.Field];
            }
        }

        public void ApplyDictionaryTranslationsToPrevalueCaptions(IDictionaryHelper dictionaryHelper)
        {
            foreach (Field allField in this.AllFields)
            {
                if (allField.PreValues != null)
                {
                    foreach (FieldPrevalue preValue in allField.PreValues)
                    {
                        if (!string.IsNullOrEmpty(preValue.Caption) && preValue.Caption.StartsWith("#"))
                            preValue.Caption = dictionaryHelper.GetText(preValue.Caption);
                    }
                }
            }
        }

        public object Clone() => this.MemberwiseClone();
    }
}
