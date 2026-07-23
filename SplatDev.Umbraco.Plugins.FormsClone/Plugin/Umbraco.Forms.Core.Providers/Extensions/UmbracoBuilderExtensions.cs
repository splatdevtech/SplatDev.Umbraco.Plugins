
// Type: Umbraco.Forms.Core.Providers.Extensions.UmbracoBuilderExtensions
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers.DatasourceTypes;
using Umbraco.Forms.Core.Providers.EmailTemplates;
using Umbraco.Forms.Core.Providers.Export;
using Umbraco.Forms.Core.Providers.FieldTypes;
using Umbraco.Forms.Core.Providers.ParsedPlacholderFormatters;
using Umbraco.Forms.Core.Providers.PreValues;
using Umbraco.Forms.Core.Providers.RecordActions.Recordsets;
using Umbraco.Forms.Core.Providers.RecordActions.RecordSets;
using Umbraco.Forms.Core.Providers.Themes;
using Umbraco.Forms.Core.Providers.ValidationPatterns;
using Umbraco.Forms.Core.Providers.WorkflowTypes;


#nullable enable
namespace Umbraco.Forms.Core.Providers.Extensions
{
  public static class UmbracoBuilderExtensions
  {
    public static IUmbracoBuilder AddUmbracoFormsCoreProviders(
      this IUmbracoBuilder builder)
    {
      builder.FormsDataSources().Add<MsSql>();
      builder.FormsExporters().Add<ExportToExcel>().Add<ExportToExcelWithDisplayValues>().Add<SaveAllUploadedFiles>().Add<SaveAllUploadedFilesByEntry>();
      builder.FormsFieldPreValueSources().Add<DataSource>().Add<GetValuesFromTextFile>().Add<NodePreValues>().Add<ReadOnlySql>().Add<UmbracoPreValuesReadOnly>();
      builder.FormsFields().Add<CheckBox>().Add<CheckBoxList>().Add<Umbraco.Forms.Core.Providers.FieldTypes.DataConsent>().Add<DatePicker>().Add<DropDownList>().Add<FileUpload>().Add<HiddenField>().Add<Password>().Add<RadioButtonList>().Add<Recaptcha2>().Add<Recaptcha3>().Add<RichText>().Add<Text>().Add<Textarea>().Add<Textfield>();
      builder.FormsRecordSetActions().Add<ApproveRecordSet>().Add<DeleteRecordSet>().Add<RejectRecordSet>();
      builder.FormsWorkflows().Add<ChangeRecordState>().Add<PostAsXml>().Add<PostToUrl>().Add<SaveAsFile>().Add<SaveAsUmbracoNode>().Add<SendEmail>().Add<SendRazorEmail>().Add<SendXsltEmail>().Add<Slack>().Add<SlackV2>();
      builder.FormsParsedPlaceholderFormatters().Add<BoundNumber>().Add<Currency>().Add<FormatDate>().Add<FormatNumber>().Add<HtmlEncode>().Add<ToLowerCaseString>().Add<ToUpperCaseString>().Add<TruncateString>();
      builder.FormsValidationPatterns().Append<Email>().Append<Number>().Append<Url>();
      builder.Themes().Add<DefaultTheme>().Add<BootstrapHorizontalTheme>();
      builder.EmailTemplates().Add<DefaultEmailTemplate>();
      return builder;
    }

    public static IUmbracoBuilder AddFormsDataSource<T>(this IUmbracoBuilder builder) where T : FormDataSourceType
    {
      builder.FormsDataSources().Add<T>();
      return builder;
    }

    public static DataSourceTypeCollectionBuilder FormsDataSources(
      this IUmbracoBuilder builder)
    {
      return builder.WithCollectionBuilder<DataSourceTypeCollectionBuilder>();
    }

    public static IUmbracoBuilder AddFormsExporter<T>(this IUmbracoBuilder builder) where T : ExportType
    {
      builder.FormsExporters().Add<T>();
      return builder;
    }

    public static ExportCollectionBuilder FormsExporters(
      this IUmbracoBuilder builder)
    {
      return builder.WithCollectionBuilder<ExportCollectionBuilder>();
    }

    public static IUmbracoBuilder AddFormsFieldPreValueSource<T>(
      this IUmbracoBuilder builder)
      where T : FieldPreValueSourceType
    {
      builder.FormsFieldPreValueSources().Add<T>();
      return builder;
    }

    public static FieldPreValueSourceCollectionBuilder FormsFieldPreValueSources(
      this IUmbracoBuilder builder)
    {
      return builder.WithCollectionBuilder<FieldPreValueSourceCollectionBuilder>();
    }

    public static IUmbracoBuilder AddFormsField<T>(this IUmbracoBuilder builder) where T : FieldType
    {
      builder.FormsFields().Add<T>();
      return builder;
    }

    public static FieldCollectionBuilder FormsFields(
      this IUmbracoBuilder builder)
    {
      return builder.WithCollectionBuilder<FieldCollectionBuilder>();
    }

    public static IUmbracoBuilder AddFormsRecordSetAction<T>(
      this IUmbracoBuilder builder)
      where T : RecordSetActionType
    {
      builder.FormsRecordSetActions().Add<T>();
      return builder;
    }

    public static RecordSetActionCollectionBuilder FormsRecordSetActions(
      this IUmbracoBuilder builder)
    {
      return builder.WithCollectionBuilder<RecordSetActionCollectionBuilder>();
    }

    public static IUmbracoBuilder AddFormsWorkflow<T>(this IUmbracoBuilder builder) where T : WorkflowType
    {
      builder.FormsWorkflows().Add<T>();
      return builder;
    }

    public static WorkflowCollectionBuilder FormsWorkflows(
      this IUmbracoBuilder builder)
    {
      return builder.WithCollectionBuilder<WorkflowCollectionBuilder>();
    }

    public static IUmbracoBuilder AddFormsParsedPlaceholderFormatter<T>(
      this IUmbracoBuilder builder)
      where T : IParsedPlaceholderFormatter
    {
      builder.FormsParsedPlaceholderFormatters().Add<T>();
      return builder;
    }

    public static ParsedPlaceholderFormatterCollectionBuilder FormsParsedPlaceholderFormatters(
      this IUmbracoBuilder builder)
    {
      return builder.WithCollectionBuilder<ParsedPlaceholderFormatterCollectionBuilder>();
    }

    public static IUmbracoBuilder ApppendFormsValidationPattern<T>(
      this IUmbracoBuilder builder)
      where T : IValidationPattern
    {
      builder.FormsValidationPatterns().Append<T>();
      return builder;
    }

    public static ValidationPatternCollectionBuilder FormsValidationPatterns(
      this IUmbracoBuilder builder)
    {
      return builder.WithCollectionBuilder<ValidationPatternCollectionBuilder>();
    }

    public static ThemeCollectionBuilder Themes(this IUmbracoBuilder builder) => builder.WithCollectionBuilder<ThemeCollectionBuilder>();

    public static EmailTemplateCollectionBuilder EmailTemplates(
      this IUmbracoBuilder builder)
    {
      return builder.WithCollectionBuilder<EmailTemplateCollectionBuilder>();
    }
  }
}
