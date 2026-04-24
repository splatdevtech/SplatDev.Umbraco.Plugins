using System.Runtime.CompilerServices;

namespace FormBuilder
{
    public static class Constants
    {
        public const string ApiName = "formBuilder";
        public static class System
        {
            public const string ApplicationAlias = "formBuilder";
            public const string ApplicationName = "Form Builder";
            public const string AreaName = "FormBuilder";
            public const string MigrationPlanName = "FormBuilder";
            public const string PluginPath = "/App_Plugins/FormBuilder/";
            public const string EmailTemplatesPath = "/Views/Partials/Forms/Emails";
            public const string ThemesPath = "/Views/Partials/Forms/Themes";
            public const string ViewsPath = "/Views/Partials/Forms";
            public const string FormsFileSystemViewsPath = "Forms";
            public const string ThemesFileSystemViewsPath = "Forms/Themes";
            public const string DefaultEmailFileName = "Example-Template.cshtml";
            public const string DefaultEmailTemplatePath = "Forms/Emails/Example-Template.cshtml";
        }

        public static class FormBuilderCacheKeys
        {
            public const string FoldersDbCacheRefresherId = "74318b85-f97d-49af-ba15-caf9e0ba4d5a";
            public static readonly Guid FoldersDbCacheRefresherGuid = new("74318b85-f97d-49af-ba15-caf9e0ba4d5a");
            public const string FormsDbCacheRefresherId = "8ad0c841-02c9-4460-8627-562beba6a36a";
            public static readonly Guid FormsDbCacheRefresherGuid = new("8ad0c841-02c9-4460-8627-562beba6a36a");
            public const string WorkDbflowCacheRefresherId = "bd86d2b0-d738-4dbc-be69-87a74b67760c";
            public static readonly Guid WorkflowDbCacheRefresherGuid = new("bd86d2b0-d738-4dbc-be69-87a74b67760c");
            public const string PreValueDbCacheRefresherId = "628a5766-5823-49b1-9269-a1b1df7c798c";
            public static readonly Guid PreValueDbCacheRefresherGuid = new("628a5766-5823-49b1-9269-a1b1df7c798c");
            public const string DataSourceDbCacheRefresherId = "174f7b86-0b49-43e6-8cee-fa28fdeb2fec";
            public static readonly Guid DataSourceDbCacheRefresherGuid = new("174f7b86-0b49-43e6-8cee-fa28fdeb2fec");
            public const string FormStorageAllFormsKey = "Forms.FormStorage.All";
            public const string FolderPrefix = "Forms.Folder.";
            public const string PreValuePrefix = "Forms.PreValues.";
            public const string PreValueSourcePrevaluesFormat = "Forms.PreValues.{0}.{1}.{2}.{3}";
            public const string DataSourcePrefix = "Forms.DataSource.";
            public const string WorkflowPrefix = "Forms.Workflow.";
            public const string FormsVersion = "Forms.Version";
            public const string FormsSettingsAll = "Forms.Setting.All";

            public static string GetMemberCacheKey(Guid memberKey)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(12, 1);
                interpolatedStringHandler.AppendLiteral("Forms.Member");
                interpolatedStringHandler.AppendFormatted(memberKey);
                return interpolatedStringHandler.ToStringAndClear();
            }

            public static string GetMemberValuesCacheKey(Guid memberKey)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(19, 1);
                interpolatedStringHandler.AppendLiteral("Forms.Member");
                interpolatedStringHandler.AppendFormatted(memberKey);
                interpolatedStringHandler.AppendLiteral(".Values");
                return interpolatedStringHandler.ToStringAndClear();
            }
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

        public static class FieldPrevalueSourceTypes
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
            public const string FormBuildersIndexItemType = "FormBuilder";
            public const string RecordIndexType = "Record";
            public const string RecordIndexPath = "FormBuilderRecords";
            public const string RecordIndexName = "FormBuilderRecordsIndex";
        }

        public static class FileSystemRoots
        {
            public const string RootForSavedData = "~/umbraco/Data/FormBuilder/";
            public const string RootForPackageData = "~/App_Plugins/FormBuilder/Data/";
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
            public const string Record = "FormBuilderRecords";
            public const string RecordField = "FormBuilderRecordFields";
            public const string RecordDataBit = "FormBuilderRecordDataBit";
            public const string RecordDataDateTime = "FormBuilderRecordDataDateTime";
            public const string RecordDataInteger = "FormBuilderRecordDataInteger";
            public const string RecordDataLongString = "FormBuilderRecordDataLongString";
            public const string RecordDataString = "FormBuilderRecordDataString";
            public const string RecordAudit = "FormBuilderRecordAudit";
            public const string RecordWorkflowAudit = "FormBuilderRecordWorkflowAudit";
            public const string UserSecurity = "FormBuilderUserSecurity";
            public const string UserGroupSecurity = "FormBuilderUserGroupSecurity";
            public const string UserFormSecurity = "FormBuilderUserFormSecurity";
            public const string UserGroupFormSecurity = "FormBuilderUserGroupFormSecurity";
            public const string Form = "FormBuilderForms";
            public const string Folder = "FormBuilderFolders";
            public const string Workflow = "FormBuilderWorkflows";
            public const string PrevalueSource = "FormBuilderPrevalueSource";
            public const string DataSource = "FormBuilderDataSource";
            public const string UserStartFolder = "FormBuilderUserStartFolders";
            public const string UserGroupStartFolder = "FormBuilderUserGroupStartFolders";
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
            public const string Path = "~/App_Plugins/FormBuilder";
            public const string UploadTempPath = "~/umbraco/Data/TEMP/FileUploads";
            public const string UploadPath = "forms/upload";
            public const string VersionMarkerFileName = "version";

            public static class SectionKeys
            {
                internal const string Root = "Umbraco:FormBuilder";
                public static readonly string FormDesign = "Umbraco:FormBuilder:FormDesign";
                public static readonly string PackageOptions = "Umbraco:FormBuilder:Options";
                public static readonly string Security = "Umbraco:FormBuilder:Security";

                public static class FieldTypes
                {
                    private static readonly string s_root = "Umbraco:FormBuilder:FieldTypes";
                    public static readonly string DatePicker = s_root + ":DatePicker";
                    public static readonly string Recaptcha2 = s_root + ":Recaptcha2";
                    public static readonly string Recaptcha3 = s_root + ":Recaptcha3";
                    public static readonly string RichText = s_root + ":RichText";
                    public static readonly string TitleAndDescription = s_root + ":TitleAndDescription";
                }
            }
        }

        public static class WebhookEvents
        {
            public static class Aliases
            {
                public const string WorkflowExecutionCancelled = "FormBuilderWorkflowExecutionCancelled";
                public const string WorkflowExecutionCompleted = "FormBuilderWorkflowExecutionCompleted";
                public const string WorkflowExecutionFailed = "FormBuilderWorkflowExecutionFailed";
            }

            public static class Types
            {
                public const string WorkflowExecution = "FormBuilderTask";
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
            public const string Workflow = "FormBuildersWorkflow";
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
            public const string FormPickerEditorAlias = "FormBuilder.FormPicker";
            public const string FormDetailsPickerEditorAlias = "FormBuilder.FormDetailsPicker";
            public const string FormPickerEditorUiAlias = "Forms.PropertyEditorUi.FormPicker.Single";
            public const string ThemePickerEditorAlias = "FormBuilder.ThemePicker";
            public const string ThemePickerEditorUiAlias = "Forms.PropertyEditorUi.ThemePicker";
        }

        public static class EmailTemplates
        {
            private static string[] coreEmailTemplates =
            [
                "Example-Template.cshtml"
            ];

            public static string[] CoreEmailTemplates { get => coreEmailTemplates; set => coreEmailTemplates = value; }
        }

        public static class PartialViewMacros
        {
            private static string[] partialViewMacroFiles =
            [
                "InsertFormBuilderWithTheme.cshtml",
                "RenderFormBuilderScripts.cshtml"
            ];

            public static string[] PartialViewMacroFiles { get => partialViewMacroFiles; set => partialViewMacroFiles = value; }
        }

        public static class FormTemplates
        {
            private static string[] coreFormTemplates =
            [
                "commentform.json",
                "contactform.json"
            ];

            public static string[] CoreFormTemplates { get => coreFormTemplates; set => coreFormTemplates = value; }
        }

        public static class HttpClients
        {
            public const string Recaptcha3 = "Umbraco:FormBuilder:HttpClients:Recaptcha3";
        }

        internal static class DataProtectionPurposes
        {
            private const string Prefix = "FormBuilder";
            public const string RecordState = "FormBuilderRecordState";
            public const string FileUpload = "FormBuilderFileUpload";
            public const string AdditionalData = "FormBuilderAdditionalData";
        }

        public static class Labels
        {
            public const string DefaultPagingDetailsFormat = "Page {0} of {1}";
            public const string DefaultPageCaptionFormat = "Page {0}";
        }

        /// <summary>
        ///     Char Arrays to avoid allocations
        /// </summary>
        public static class CharArrays
        {
            /// <summary>
            ///     Char array containing only /
            /// </summary>
            public static readonly char[] ForwardSlash = ['/'];

            /// <summary>
            ///     Char array containing only \
            /// </summary>
            public static readonly char[] Backslash = ['\\'];

            /// <summary>
            ///     Char array containing only '
            /// </summary>
            public static readonly char[] SingleQuote = ['\''];

            /// <summary>
            ///     Char array containing only "
            /// </summary>
            public static readonly char[] DoubleQuote = ['\"'];

            /// <summary>
            ///     Char array containing ' "
            /// </summary>
            public static readonly char[] DoubleQuoteSingleQuote = ['\"', '\''];

            /// <summary>
            ///     Char array containing only _
            /// </summary>
            public static readonly char[] Underscore = ['_'];

            /// <summary>
            ///     Char array containing \n \r
            /// </summary>
            public static readonly char[] LineFeedCarriageReturn = ['\n', '\r'];

            /// <summary>
            ///     Char array containing \n
            /// </summary>
            public static readonly char[] LineFeed = ['\n'];

            /// <summary>
            ///     Char array containing only ,
            /// </summary>
            public static readonly char[] Comma = [','];

            /// <summary>
            ///     Char array containing only &amp;
            /// </summary>
            public static readonly char[] Ampersand = ['&'];

            /// <summary>
            ///     Char array containing only \0
            /// </summary>
            public static readonly char[] NullTerminator = ['\0'];

            /// <summary>
            ///     Char array containing only .
            /// </summary>
            public static readonly char[] Period = ['.'];

            /// <summary>
            ///     Char array containing only ~
            /// </summary>
            public static readonly char[] Tilde = ['~'];

            /// <summary>
            ///     Char array containing ~ /
            /// </summary>
            public static readonly char[] TildeForwardSlash = ['~', '/'];

            /// <summary>
            ///     Char array containing ~ / \
            /// </summary>
            public static readonly char[] TildeForwardSlashBackSlash = ['~', '/', '\\'];

            /// <summary>
            ///     Char array containing only ?
            /// </summary>
            public static readonly char[] QuestionMark = ['?'];

            /// <summary>
            ///     Char array containing ? &amp;
            /// </summary>
            public static readonly char[] QuestionMarkAmpersand = ['?', '&'];

            /// <summary>
            ///     Char array containing XML 1.1 whitespace chars
            /// </summary>
            public static readonly char[] XmlWhitespaceChars = [' ', '\t', '\r', '\n'];

            /// <summary>
            ///     Char array containing only the Space char
            /// </summary>
            public static readonly char[] Space = [' '];

            /// <summary>
            ///     Char array containing only ;
            /// </summary>
            public static readonly char[] Semicolon = [';'];

            /// <summary>
            ///     Char array containing a comma and a space
            /// </summary>
            public static readonly char[] CommaSpace = [',', ' '];

            /// <summary>
            ///     Char array containing  _ -
            /// </summary>
            public static readonly char[] UnderscoreDash = ['_', '-'];

            /// <summary>
            ///     Char array containing =
            /// </summary>
            public static readonly char[] EqualsChar = ['='];

            /// <summary>
            ///     Char array containing >
            /// </summary>
            public static readonly char[] GreaterThan = ['>'];

            /// <summary>
            ///     Char array containing |
            /// </summary>
            public static readonly char[] VerticalTab = ['|'];
        }

    }
}
