using FormBuilder.Core.Configuration;
using FormBuilder.Core.Definitions;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Helpers;
using FormBuilder.Core.Interfaces;

using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;

using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace FormBuilder.Core.Models
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
            Pages = [];
            ValidationRules = [];
            SetFromDefaults(defaultSettings);
        }

        protected void SetFromDefaults(DefaultFormSettings defaultSettings)
        {
            if (defaultSettings == null)
                return;
            ManualApproval = defaultSettings.ManualApproval;
            DisableDefaultStylesheet = defaultSettings.DisableStylesheet;
            FieldIndicationType = defaultSettings.MarkFieldsIndicator;
            Indicator = defaultSettings.Indicator;
            RequiredErrorMessage = defaultSettings.RequiredErrorMessage;
            InvalidErrorMessage = defaultSettings.InvalidErrorMessage;
            ShowValidationSummary = defaultSettings.ShowValidationSummary;
            HideFieldValidation = defaultSettings.HideFieldValidationLabels;
            NextLabel = defaultSettings.NextPageButtonLabel;
            PrevLabel = defaultSettings.PreviousPageButtonLabel;
            SubmitLabel = defaultSettings.SubmitButtonLabel;
            MessageOnSubmit = defaultSettings.MessageOnSubmit;
            MessageOnSubmitIsHtml = defaultSettings.MessageOnSubmitIsHtml;
            StoreRecordsLocally = defaultSettings.StoreRecordsLocally;
            AutocompleteAttribute = defaultSettings.AutocompleteAttribute;
            DaysToRetainSubmittedRecordsFor = defaultSettings.DaysToRetainSubmittedRecordsFor;
            DaysToRetainApprovedRecordsFor = defaultSettings.DaysToRetainApprovedRecordsFor;
            DaysToRetainRejectedRecordsFor = defaultSettings.DaysToRetainRejectedRecordsFor;
            ShowPagingOnMultiPageForms = defaultSettings.ShowPagingOnMultiPageForms;
            PagingDetailsFormat = defaultSettings.PagingDetailsFormat;
            PageCaptionFormat = defaultSettings.PageCaptionFormat;
            ShowSummaryPageOnMultiPageForms = defaultSettings.ShowSummaryPageOnMultiPageForms;
            SummaryLabel = defaultSettings.SummaryLabel;
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
        public Guid Unique => Id;

        [DataMember(Name = "parentUnique")]
        public Guid? ParentUnique => FolderId;

        [DataMember(Name = "entityType")]
        public static string EntityType => "form";

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
        public List<RecordFieldDisplay> SelectedDisplayFields { get; set; } = [];

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
                List<FieldSet> allFieldSets = [];
                foreach (Page page in Pages)
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
                List<Field> allFields = [];
                foreach (FieldSet allFieldSet in AllFieldSets)
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
            string[] strArray =
            [
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
            ];
            StringBuilder stringBuilder = new();
            if (AllFields.Count <= 4)
            {
                int num = 1;
                foreach (Field allField in AllFields)
                {
                    stringBuilder.Append(allField.Caption);
                    if (num == AllFields.Count - 1)
                        stringBuilder.Append(" and ");
                    else if (num < AllFields.Count)
                        stringBuilder.Append(", ");
                    ++num;
                }
            }
            else
            {
                for (int index = 0; index < 3; ++index)
                    stringBuilder.Append(AllFields[index].Caption + ", ");
                stringBuilder.Append(AllFields[3].Caption + " and ");
                if (AllFields.Count > 14)
                {
                    stringBuilder.Append(" and a lot more");
                }
                else
                {
                    stringBuilder.Append(strArray[AllFields.Count - 5] + " additional field");
                    if (AllFields.Count != 1)
                        stringBuilder.Append('s');
                }
            }
            return stringBuilder.ToString();
        }

        public void SetFieldAliases(IShortStringHelper shortStringHelper)
        {
            foreach (Field field in Pages.SelectMany(x => x.AllFields()))
            {
                if (string.IsNullOrEmpty(field.Alias))
                    field.Alias = GenerateFieldAlias(shortStringHelper, field.Caption);
            }
        }

        private static string GenerateFieldAlias(IShortStringHelper shortStringHelper, string caption)
        {
            string cleanString = caption.ToCleanString(shortStringHelper, CleanStringType.CamelCase);
            return cleanString.Length > byte.MaxValue ? cleanString[..byte.MaxValue] : cleanString;
        }

        public bool HasDataSource() => DataSource is not null && !(DataSource.Id == Guid.Empty);

        public void EnsureFormStructureIds()
        {
            foreach (Page page in Pages)
            {
                EnsureFormObjectId(page);
                if (page.ButtonCondition is not null)
                    EnsureFormObjectId(page.ButtonCondition);
                foreach (FieldSet fieldSet in page.FieldSets)
                {
                    EnsureFormObjectId(fieldSet);
                    fieldSet.Page = page.Id;
                    if (fieldSet.Condition is not null)
                        EnsureFormObjectId(fieldSet.Condition);
                    foreach (FieldsetContainer container in fieldSet.Containers)
                    {
                        EnsureFormObjectId(container);
                        foreach (Field field in container.Fields)
                        {
                            EnsureFormObjectId(field);
                            if (field.Condition is not null)
                                EnsureFormObjectId(field.Condition);
                        }
                    }
                }
            }
        }

        private static void EnsureFormObjectId(IFormObject formObject)
        {
            if (!(formObject.Id == Guid.Empty))
                return;
            formObject.Id = Guid.NewGuid();
        }

        public RegenerateFormStructureIdsResult RegenerateFormStructureIds()
        {
            Dictionary<Guid, Guid> fieldIdMapping = [];
            foreach (Page page in Pages)
            {
                page.Id = Guid.NewGuid();
                if (page.ButtonCondition is not null)
                    page.ButtonCondition.Id = Guid.NewGuid();
                foreach (FieldSet fieldSet in page.FieldSets)
                {
                    fieldSet.Id = Guid.NewGuid();
                    fieldSet.Page = page.Id;
                    if (fieldSet.Condition is not null)
                        fieldSet.Condition.Id = Guid.NewGuid();
                    foreach (FieldsetContainer container in fieldSet.Containers)
                    {
                        container.Id = Guid.NewGuid();
                        foreach (Field field in container.Fields)
                        {
                            Guid guid = Guid.NewGuid();
                            fieldIdMapping.Add(field.Id, guid);
                            field.Id = guid;
                            if (field.Condition is not null)
                                field.Condition.Id = Guid.NewGuid();
                        }
                    }
                }
            }
            ApplyNewFieldIdsToConditions(fieldIdMapping);
            return new RegenerateFormStructureIdsResult(fieldIdMapping);
        }

        private void ApplyNewFieldIdsToConditions(Dictionary<Guid, Guid> fieldIdMapping)
        {
            foreach (Page page in Pages)
            {
                if (page.ButtonCondition is not null)
                    ApplyNewFieldIdsToConditionRules(fieldIdMapping, page.ButtonCondition);
                foreach (FieldSet fieldSet in page.FieldSets)
                {
                    if (fieldSet.Condition is not null)
                        ApplyNewFieldIdsToConditionRules(fieldIdMapping, fieldSet.Condition);
                    foreach (Field field in fieldSet.Containers.SelectMany(x => x.Fields).Where(x => x.Condition is not null && x.Condition.Rules.Any()))
                        ApplyNewFieldIdsToConditionRules(fieldIdMapping, field.Condition!);
                }
            }
        }

        private static void ApplyNewFieldIdsToConditionRules(
          Dictionary<Guid, Guid> fieldIdMapping,
          FieldCondition condition)
        {
            foreach (FieldConditionRule rule in condition.Rules)
            {
                if (fieldIdMapping.TryGetValue(rule.Field, out Guid value))
                    rule.Field = value;
            }
        }

        public void ApplyDictionaryTranslationsToPrevalueCaptions(IDictionaryHelper dictionaryHelper)
        {
            foreach (Field allField in AllFields)
            {
                if (allField.PreValues is not null)
                {
                    foreach (FieldPrevalue preValue in allField.PreValues)
                    {
                        if (!string.IsNullOrEmpty(preValue.Caption) && preValue.Caption.StartsWith('#'))
                            preValue.Caption = dictionaryHelper.GetText(preValue.Caption);
                    }
                }
            }
        }

        public object Clone() => MemberwiseClone();
    }
}