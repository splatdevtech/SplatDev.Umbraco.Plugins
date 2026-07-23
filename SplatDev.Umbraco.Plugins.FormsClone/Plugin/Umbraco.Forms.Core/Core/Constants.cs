// Type: Umbraco.Forms.Core.Constants
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389


#nullable enable
namespace Umbraco.Forms.Core
{
    public static class Constants
    {
        public static class System
        {
            public const string ApplicationAlias = "forms";
            public const string ApplicationName = "Forms";
            public const string AreaName = "UmbracoForms";
            public const string MigrationPlanName = "UmbracoForms";
            public const string PluginPath = "/App_Plugins/UmbracoForms/";
            public const string EmailTemplatesPath = "/Views/Partials/Forms/Emails";
            public const string ThemesPath = "/Views/Partials/Forms/Themes";
            public const string ViewsPath = "/Views/Partials/Forms";
            public const string FormsFileSystemViewsPath = "Forms";
            public const string ThemesFileSystemViewsPath = "Forms/Themes";
            public const string DefaultEmailFileName = "Example-Template.cshtml";
            public const string DefaultEmailTemplatePath = "Forms/Emails/Example-Template.cshtml";
        }

        public static class Trees
        {
            public const string Form = "Form";
            public const string PreValueSource = "PreValueSource";
            public const string DataSource = "DataSource";
            public const string FormSecurity = "FormSecurity";
            public const string EmailTemplates = "EmailTemplates";
        }

        public static class Macros
        {
            public const string FormScripts = "8828F4AF-6736-4B04-BBAA-8B46BB3201ED";
            public const string InsertFormWithTheme = "480075D7-3F9D-4190-9356-11978DDBB8CF";
            public const string InsertFormWithThemePropPicker = "6AEA3D9A-EEA6-47AF-8C58-FA37CC49F414";
            public const string InsertFormWithThemePropTheme = "83C70339-98A0-4111-853C-2F0D4014A4FC";
            public const string InsertFormWithThemePropExcludeScripts = "2D5FDDA2-E19F-4103-B43E-269186FDA969";
            public const string InsertFormWithThemePropRedirectToPageId = "2B134A1A-AAEE-42B1-8277-AD8FFDCC70A3";
        }

        public static class FieldTypes
        {
            public const string CheckBox = "D5C0C390-AE9A-11DE-A69E-666455D89593";
            public const string CheckBoxList = "FAB43F20-A6BF-11DE-A28F-9B5755D89593";
            public const string DatePicker = "F8B4C3B8-AF28-11DE-9DD8-EF5956D89593";
            public const string DropDownList = "0DD29D42-A6A5-11DE-A2F2-222256D89593";
            public const string Upload = "84A17CF8-B711-46A6-9840-0E4A072AD000";
            public const string HiddenField = "DA206CAE-1C52-434E-B21A-4A7C198AF877";
            public const string Password = "FB37BC60-D41E-11DE-AEAE-37C155D89593";
            public const string RadioButtonList = "903DF9B0-A78C-11DE-9FC1-DB7A56D89593";
            public const string Recaptcha2 = "B69DEAEB-ED75-4DC9-BFB8-D036BF9D3730";
            public const string Recaptcha3 = "663AA19B-423D-4F38-A1D6-C840C926EF86";
            public const string Text = "E3FBF6C4-F46C-495E-AFF8-4B3C227B4A98";
            public const string RichText = "1F8D45F8-76E6-4550-A0F5-9637B8454619";
            public const string Textarea = "023F09AC-1445-4BCB-B8FA-AB49F33BD046";
            public const string Textfield = "3F92E01B-29E2-4A30-BF33-9DF5580ED52C";
            public const string DataConsentField = "A72C9DF9-3847-47CF-AFB8-B86773FD12CD";
        }

        public static class RecordSetActionTypes
        {
            public const string ApproveRecordSet = "CB126B79-9011-11DF-A4EE-0800200C9A66";
            public const string DeleteRecordSet = "CB126B70-9011-11DF-A4EE-0800200C9A66";
            public const string RejectRecordSet = "84cd75a7-d3d9-4551-9c1a-3f478b4ec9ed";
        }

        public static class FormDataSourceTypes
        {
            public const string MsSql = "F19506F3-EFEA-4B13-A308-89348F69DF91";
            public const string Webservice = "7EDF567C-4230-4079-B3FB-CCA44BAF6B75";
        }

        public static class FieldPreValueSourceTypes
        {
            public const string DataSource = "CC9F9B2A-A746-11DE-9E17-681B56D89593";
            public const string GetValuesFromTextFile = "35C2053E-CBF7-4793-B27C-6E97B7671A2D";
            public const string NodePreValues = "DE996870-C45A-11DE-8A39-0800200C9A66";
            public const string ReadOnlySql = "F1F5BD4D-E6AE-44ED-86CB-97661E4660B2";
            public const string UmbracoPreValuesReadOnly = "EA773CAF-FEF2-491B-B5B7-6A3552B1A0E2";
        }

        public static class WorkflowTypes
        {
            public const string ChangeRecordState = "4C40A092-0CB5-481D-96A7-A02D8E7CDB2F";
            public const string PostAsXml = "470EEB3A-CB15-4B08-9FC0-A2F091583332";
            public const string PostToUrl = "FD02C929-4E7D-4F90-B9FA-13D074A76688";
            public const string SaveAsFile = "9CC5854D-61A2-48F6-9F4A-8F3BDFAFB521";
            public const string SaveAsUmbracoNode = "89FB1E31-9F36-4E08-9D1B-AF1180D340DB";
            public const string SendEmail = "E96BADD7-05BE-4978-B8D9-B3D733DE70A5";
            public const string SendRazorEmail = "17C61629-D984-4E86-B43B-A8407B3EFEA9";
            public const string SendXsltEmail = "616EDFEB-BADF-414B-89DC-D8655EB85998";
            public const string Slack = "CCBFB0D5-ADAA-4729-8B4C-4BB439DC0202";
            public const string SlackV2 = "BC52AB28-D3FF-42EE-AF75-A5D49BE83040";
        }

        public static class DataConsent
        {
            public const string Alias = "dataConsent";
            public const string FieldDefaultValue = "Consent for storing submitted data";
            public const string AcceptDefaultValue = "Yes, I give permission to store and process my data";
            public const string RequiredDefaultValue = "Consent is required to store and process the data in this form.";
        }

        public static class ExportTypes
        {
            public const string Excel = "94ED105A-87B3-4e1f-97CB-9A320AEE2745";
            public const string ExcelDisplayValues = "688711A2-DC6F-4B51-B8D2-0BB177BB0499";
            public const string SaveAllUploadedFiles = "08479664-4FD9-4C7E-9504-77B764878E86";
            public const string SaveAllUploadedFilesByEntry = "fa7ae082-5c6a-4fdc-babd-162c9607b343";
        }

        public static class ExamineIndex
        {
            public const string UmbracoFormsIndexItemType = "UmbracoForms";
            public const string RecordIndexType = "Record";
            public const string RecordIndexPath = "UmbracoFormsRecords";
            public const string RecordIndexName = "UmbracoFormsRecordsIndex";
        }

        public static class FileSystemRoots
        {
            public const string RootForSavedData = "~/umbraco/Data/UmbracoForms/";
            public const string RootForPackageData = "~/App_Plugins/UmbracoForms/Data/";
        }

        public static class RecordIndex
        {
            public const string Fields = "fields";
            public const string Blob = "blob";
            public const string State = "State";
            public const string Ip = "Ip";
            public const string UniqueId = "UniqueId";
            public const string Updated = "Updated";
            public const string Created = "Created";
            public const string Form = "Form";
            public const string MemberKey = "MemberKey";
            public const string CurrentPage = "CurrentPage";
            public const string UmbracoPageId = "UmbracoPageId";
            public const string RecordFields = "RecordFields";
        }

        public static class Tables
        {
            public const string Record = "UFRecords";
            public const string RecordField = "UFRecordFields";
            public const string RecordDataBit = "UFRecordDataBit";
            public const string RecordDataDateTime = "UFRecordDataDateTime";
            public const string RecordDataInteger = "UFRecordDataInteger";
            public const string RecordDataLongString = "UFRecordDataLongString";
            public const string RecordDataString = "UFRecordDataString";
            public const string RecordAudit = "UFRecordAudit";
            public const string RecordWorkflowAudit = "UFRecordWorkflowAudit";
            public const string UserSecurity = "UFUserSecurity";
            public const string UserGroupSecurity = "UFUserGroupSecurity";
            public const string UserFormSecurity = "UFUserFormSecurity";
            public const string UserGroupFormSecurity = "UFUserGroupFormSecurity";
            public const string Form = "UFForms";
            public const string Folder = "UFFolders";
            public const string Workflow = "UFWorkflows";
            public const string PrevalueSource = "UFPrevalueSource";
            public const string DataSource = "UFDataSource";
            public const string UserStartFolder = "UFUserStartFolders";
            public const string UserGroupStartFolder = "UFUserGroupStartFolders";
        }

        public static class DefaultValues
        {
            public const int MaxNumberOfColumnsInFormGroup = 12;
        }

        public static class Formats
        {
            public const string DefaultDatePickerFormat = "LL";
        }

        public static class Configuration
        {
            public const string Path = "~/App_Plugins/UmbracoForms";
            public const string UploadTempPath = "~/umbraco/Data/TEMP/FileUploads";
            public const string UploadPath = "forms/upload";
            public const string VersionMarkerFileName = "version";

            public static class SectionKeys
            {
                internal const string Root = "Umbraco:Forms";
                public static readonly string FormDesign = "Umbraco:Forms:FormDesign";
                public static readonly string PackageOptions = "Umbraco:Forms:Options";
                public static readonly string Security = "Umbraco:Forms:Security";

                public static class FieldTypes
                {
                    private static readonly string s_root = "Umbraco:Forms:FieldTypes";
                    public static readonly string DatePicker = Constants.Configuration.SectionKeys.FieldTypes.s_root + ":DatePicker";
                    public static readonly string Recaptcha2 = Constants.Configuration.SectionKeys.FieldTypes.s_root + ":Recaptcha2";
                    public static readonly string Recaptcha3 = Constants.Configuration.SectionKeys.FieldTypes.s_root + ":Recaptcha3";
                    public static readonly string RichText = Constants.Configuration.SectionKeys.FieldTypes.s_root + ":RichText";
                    public static readonly string TitleAndDescription = Constants.Configuration.SectionKeys.FieldTypes.s_root + ":TitleAndDescription";
                }
            }
        }

        public static class WebhookEvents
        {
            public static class Aliases
            {
                public const string WorkflowExecutionCancelled = "Umbraco.Forms.WorkflowExecutionCancelled";
                public const string WorkflowExecutionCompleted = "Umbraco.Forms.WorkflowExecutionCompleted";
                public const string WorkflowExecutionFailed = "Umbraco.Forms.WorkflowExecutionFailed";
            }

            public static class Types
            {
                public const string WorkflowExecution = "Umbraco.Forms.Task";
            }
        }

        public static class Relations
        {
            public static class FormToContent
            {
                public static readonly Guid Id = Guid.Parse("be98a22d-d0f8-4615-b2e4-1aae86bf89be");
                public const string Alias = "umbForm";
            }
        }

        public static class HttpContextKeys
        {
            public static class ItemKeys
            {
                public const string PageElements = "pageElements";
            }
        }

        public static class EmailTypes
        {
            public const string Workflow = "UmbracoFormsWorkflow";
        }

        public static class FormSubmission
        {
            public const string EncryptedFilePathAndFileNameSeparator = "***|***";
        }

        public static class EventAdditionalData
        {
            public const string MovedFromFolderId = "MovedFromFolderId";
        }

        public static class ItemKeys
        {
            public const string RedirectAfterFormSubmitUrl = "FormsRedirectAfterFormSubmitUrl";
            public const string ApiSubmittedFormId = "ApiSubmittedFormId";
            public const string ApiSubmittedFormValues = "ApiSubmittedFormValues";
        }

        public static class PropertyEditors
        {
            public const string FormPickerEditorAlias = "UmbracoForms.FormPicker";
            public const string FormDetailsPickerEditorAlias = "UmbracoForms.FormDetailsPicker";
            public const string FormPickerEditorUiAlias = "Forms.PropertyEditorUi.FormPicker.Single";
            public const string ThemePickerEditorAlias = "UmbracoForms.ThemePicker";
            public const string ThemePickerEditorUiAlias = "Forms.PropertyEditorUi.ThemePicker";
        }

        [Obsolete("Not used and will be removed in a future version. Use registered ThemeCollection entries instead.")]
        public static class Themes
        {
            private const string DefaultTheme = "default";
            private const string Bootstrap3HorizontalTheme = "bootstrap3-horizontal";
            [Obsolete("Not used and will be removed in a future version. Use registered ThemeCollection entries instead.")]
            public static string[] CoreThemes = new string[2]
            {
        "default",
        "bootstrap3-horizontal"
            };

            [Obsolete("Not used and will be removed in a future version. Use registered ThemeCollection entries instead.")]
            public static IEnumerable<string> GetFilesForTheme(string theme)
            {
                if (string.IsNullOrWhiteSpace(theme))
                    return Enumerable.Empty<string>();
                string lowerInvariant = theme.ToLowerInvariant();
                if (!(lowerInvariant == "default"))
                {
                    if (!(lowerInvariant == "bootstrap3-horizontal"))
                        return Enumerable.Empty<string>();
                    return new string[3]
                    {
            string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "bootstrap3-horizontal",  "Form"),
            string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "bootstrap3-horizontal",  "Fieldtypes/FieldType.RadioButtonList"),
            string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "bootstrap3-horizontal",  "Fieldtypes/FieldType.CheckBoxList")
                    };
                }
                return new string[21]
                {
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "DatePicker"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Form"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Render"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Script"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "ScrollToFormScript"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Submitted"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.CheckBox"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.CheckBoxList"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.DataConsent"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.DatePicker"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.DropDownList"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.FileUpload"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.HiddenField"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.PasswordField"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.RadioButtonList"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.Recaptcha2"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.Recaptcha3"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.RichText"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.Text"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.Textarea"),
          string.Format("{0}/{1}/{2}.cshtml",  "/Views/Partials/Forms/Themes",  "default",  "Fieldtypes/FieldType.Textfield")
                };
            }
        }

        public static class EmailTemplates
        {
            public static string[] CoreEmailTemplates = new string[1]
            {
        "Example-Template.cshtml"
            };
        }

        public static class PartialViewMacros
        {
            public static string[] PartialViewMacroFiles = new string[2]
            {
        "InsertUmbracoFormWithTheme.cshtml",
        "RenderUmbracoFormScripts.cshtml"
            };
        }

        public static class FormTemplates
        {
            public static string[] CoreFormTemplates = new string[2]
            {
        "commentform.json",
        "contactform.json"
            };
        }

        public static class HttpClients
        {
            public const string Recaptcha3 = "Umbraco:Forms:HttpClients:Recaptcha3";
        }

        internal static class DataProtectionPurposes
        {
            private const string Prefix = "Umbraco.Forms.";
            public const string RecordState = "Umbraco.Forms.RecordState";
            public const string FileUpload = "Umbraco.Forms.FileUpload";
            public const string AdditionalData = "Umbraco.Forms.AdditionalData";
        }

        public static class Labels
        {
            public const string DefaultPagingDetailsFormat = "Page {0} of {1}";
            public const string DefaultPageCaptionFormat = "Page {0}";
        }
    }
}
