using FormBuilder.Core.DataSources;
using FormBuilder.Core.Export;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Builders;
using FormBuilder.Core.Providers.DataSourceTypes;
using FormBuilder.Core.Providers.EmailTemplates;
using FormBuilder.Core.Providers.Export;
using FormBuilder.Core.Providers.FieldTypes;
using FormBuilder.Core.Providers.ParsedPlacholderFormatters;
using FormBuilder.Core.Providers.Prevalues;
using FormBuilder.Core.Providers.RecordSets;
using FormBuilder.Core.Providers.Themes;
using FormBuilder.Core.Providers.ValidationPatterns;
using FormBuilder.Core.Providers.WorkflowTypes;
using FormBuilder.Core.Workflows;

using Umbraco.Cms.Core.DependencyInjection;

namespace FormBuilder.Core.Providers.Extensions
{
    /// <summary>
    /// Extension methods for     /// </summary>
    public static class UmbracoBuilderExtensions
    {
        /// <summary>
        /// Populates the Forms collection builders with the out-of-the-box providers.
        /// </summary>
        public static IUmbracoBuilder AddFormBuilderCoreProviders(
          this IUmbracoBuilder builder)
        {
            builder.FormsDataSources().Add<MsSql>();
            builder.FormsExporters().Add<ExportToExcel>().Add<ExportToExcelWithDisplayValues>().Add<SaveAllUploadedFiles>().Add<SaveAllUploadedFilesByEntry>();
            builder.FormsFieldPrevalueSources().Add<DataSource>().Add<GetValuesFromTextFile>().Add<NodePreValues>().Add<ReadOnlySql>().Add<UmbracoPreValuesReadOnly>();
            builder.FormsFields().Add<CheckBox>().Add<CheckBoxList>().Add<DataConsent>().Add<DatePicker>().Add<DropDownList>().Add<FileUpload>().Add<HiddenField>().Add<Password>().Add<RadioButtonList>().Add<Recaptcha2>().Add<Recaptcha3>().Add<RichText>().Add<Text>().Add<Textarea>().Add<Textfield>();
            builder.FormsRecordSetActions().Add<ApproveRecordSet>().Add<DeleteRecordSet>().Add<RejectRecordSet>();
            builder.FormsWorkflows().Add<ChangeRecordState>().Add<PostAsXml>().Add<PostToUrl>().Add<SaveAsFile>().Add<SaveAsUmbracoNode>().Add<SendEmail>().Add<SendRazorEmail>().Add<SendXsltEmail>().Add<Slack>();
            builder.FormsParsedPlaceholderFormatters().Add<BoundNumber>().Add<Currency>().Add<FormatDate>().Add<FormatNumber>().Add<HtmlEncode>().Add<ToLowerCaseString>().Add<ToUpperCaseString>().Add<TruncateString>();
            builder.FormsValidationPatterns().Append<Email>().Append<Number>().Append<Url>();
            builder.Themes().Add<DefaultTheme>().Add<BootstrapHorizontalTheme>();
            builder.EmailTemplates().Add<DefaultEmailTemplate>();
            return builder;
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder AddFormsDataSource<T>(this IUmbracoBuilder builder) where T : FormDataSourceType
        {
            builder.FormsDataSources().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static DataSourceTypeCollectionBuilder FormsDataSources(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<DataSourceTypeCollectionBuilder>();
        }

        /// <summary>
        /// Adds an         /// </summary>
        public static IUmbracoBuilder AddFormsExporter<T>(this IUmbracoBuilder builder) where T : ExportType
        {
            builder.FormsExporters().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static ExportCollectionBuilder FormsExporters(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<ExportCollectionBuilder>();
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder AddFormsFieldPrevalueSource<T>(
          this IUmbracoBuilder builder)
          where T : FieldPrevalueSourceType
        {
            builder.FormsFieldPrevalueSources().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static FieldPrevalueSourceCollectionBuilder FormsFieldPrevalueSources(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<FieldPrevalueSourceCollectionBuilder>();
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder AddFormsField<T>(this IUmbracoBuilder builder) where T : FieldType
        {
            builder.FormsFields().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static FieldCollectionBuilder FormsFields(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<FieldCollectionBuilder>();
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder AddFormsRecordSetAction<T>(
          this IUmbracoBuilder builder)
          where T : RecordSetActionType
        {
            builder.FormsRecordSetActions().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static RecordSetActionCollectionBuilder FormsRecordSetActions(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<RecordSetActionCollectionBuilder>();
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder AddFormsWorkflow<T>(this IUmbracoBuilder builder) where T : WorkflowType
        {
            builder.FormsWorkflows().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static WorkflowCollectionBuilder FormsWorkflows(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<WorkflowCollectionBuilder>();
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder AddFormsParsedPlaceholderFormatter<T>(
          this IUmbracoBuilder builder)
          where T : IParsedPlaceholderFormatter
        {
            builder.FormsParsedPlaceholderFormatters().Add<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static ParsedPlaceholderFormatterCollectionBuilder FormsParsedPlaceholderFormatters(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<ParsedPlaceholderFormatterCollectionBuilder>();
        }

        /// <summary>
        /// Adds a         /// </summary>
        public static IUmbracoBuilder ApppendFormsValidationPattern<T>(
          this IUmbracoBuilder builder)
          where T : IValidationPattern
        {
            builder.FormsValidationPatterns().Append<T>();
            return builder;
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static ValidationPatternCollectionBuilder FormsValidationPatterns(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<ValidationPatternCollectionBuilder>();
        }

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static ThemeCollectionBuilder Themes(this IUmbracoBuilder builder) => builder.WithCollectionBuilder<ThemeCollectionBuilder>();

        /// <summary>
        /// Provides access to the collection builder for instances of         /// </summary>
        public static EmailTemplateCollectionBuilder EmailTemplates(
          this IUmbracoBuilder builder)
        {
            return builder.WithCollectionBuilder<EmailTemplateCollectionBuilder>();
        }
    }
}