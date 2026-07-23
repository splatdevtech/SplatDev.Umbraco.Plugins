
// Type: Umbraco.Forms.Core.Providers.WorkflowTypes.PostToUrl
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Web;

using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Providers.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.WorkflowTypes
{
    public class PostToUrl : WorkflowType
    {
        private const string AvailableHttpMethods = "POST,GET,PUT,DELETE";
        private readonly ILogger<PostToUrl> _logger;
        private readonly IUmbracoHelperAccessor _umbracoHelperAccessor;
        private readonly IPlaceholderParsingService _placeholderParsingService;
        private readonly IPublishedUrlProvider _publishedUrlProvider;
        private readonly IHttpClientFactory _httpClientFactory;

        [Obsolete("Use the constructor with all parameters. This will be removed in a future version.")]
        public PostToUrl(
          ILogger<PostToUrl> logger,
          IUmbracoHelperAccessor umbracoHelperAccessor,
          IPublishedUrlProvider publishedUrlProvider,
          IHttpClientFactory httpClientFactory)
          : this(logger, umbracoHelperAccessor, ServiceProviderServiceExtensions.GetRequiredService<IPlaceholderParsingService>(StaticServiceProvider.Instance), publishedUrlProvider, httpClientFactory)
        {
        }

        public PostToUrl(
          ILogger<PostToUrl> logger,
          IUmbracoHelperAccessor umbracoHelperAccessor,
          IPlaceholderParsingService placeholderParsingService,
          IPublishedUrlProvider publishedUrlProvider,
          IHttpClientFactory httpClientFactory)
        {
            this._logger = logger;
            this._umbracoHelperAccessor = umbracoHelperAccessor;
            this._placeholderParsingService = placeholderParsingService;
            this._publishedUrlProvider = publishedUrlProvider;
            this.Id = new Guid("FD02C929-4E7D-4F90-B9FA-13D074A76688");
            this.Name = "Send form to URL";
            this.Alias = "sendFormToUrl";
            this.Description = "Sends the form to a URL, either as a HTTP POST or GET";
            this.Icon = "icon-terminal";
            this.Group = "Legacy";
            this._httpClientFactory = httpClientFactory;
        }

        [Setting("Url", Description = "Enter the URL to post to", DisplayOrder = 10, IsMandatory = true, SupportsPlaceholders = true)]
        public virtual string Url { get; set; } = string.Empty;

        [Setting("Method", Description = "POST or GET", DisplayOrder = 20, IsMandatory = true, PreValues = "POST,GET,PUT,DELETE", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string Method { get; set; } = string.Empty;

        [Setting("Standard Fields", Description = "Map any standard form information to send.", DisplayOrder = 30, View = "Forms.PropertyEditorUi.StandardFieldMapper")]
        public virtual string StandardFields { get; set; } = string.Empty;

        [Setting("Fields", Description = "Map the needed fields (if not mapped all fields will be sent)", DisplayOrder = 40, View = "Forms.PropertyEditorUi.FieldMapper")]
        public virtual string Fields { get; set; } = string.Empty;

        [Setting("Default Element For Fields", Description = "When no fields are specifically mapped, all fields are provided in the request as XML, using the selected field attribute as the element name.", DisplayOrder = 50, PreValues = "Caption,Alias", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string DefaultElementForFields { get; set; } = "Caption";

        [Setting("User", Description = "(optional)", DisplayOrder = 60, SupportsPlaceholders = true)]
        public virtual string Username { get; set; } = string.Empty;

        [Setting("Password", Description = "(optional)", DisplayOrder = 70, SupportsPlaceholders = true, View = "Forms.PropertyEditorUi.Password")]
        public virtual string Password { get; set; } = string.Empty;

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = new List<Exception>();
            if (string.IsNullOrEmpty(this.Url))
                exceptionList.Add(new Exception("'Url' setting has not been set"));
            if (string.IsNullOrEmpty(this.Method) || !"POST,GET,PUT,DELETE".Split(',').Contains<string>(this.Method))
                exceptionList.Add(new Exception("'Method' setting has not been set"));
            return exceptionList;
        }

        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            List<StandardFieldMapping> standardFieldMappings = new List<StandardFieldMapping>();
            if (!string.IsNullOrEmpty(this.StandardFields))
            {
                IEnumerable<StandardFieldMapping> source = JsonSerializer.Deserialize<IEnumerable<StandardFieldMapping>>(this.StandardFields, FormsJsonSerializerOptions.Default);
                if (source != null)
                    standardFieldMappings = source.ToList<StandardFieldMapping>();
            }
            List<FieldMapping> mappings = new List<FieldMapping>();
            if (!string.IsNullOrEmpty(this.Fields))
            {
                IEnumerable<FieldMapping> source = JsonSerializer.Deserialize<IEnumerable<FieldMapping>>(this.Fields, FormsJsonSerializerOptions.Default);
                if (source != null)
                    mappings = source.Select<FieldMapping, FieldMapping>(x =>
                    {
                        x.StaticValue = this._placeholderParsingService.ParsePlaceHolders(x.StaticValue, false, context);
                        return x;
                    }).ToList<FieldMapping>();
            }
            NameValueCollection values = new NameValueCollection();
            this.AddStandardFieldMappings(values, standardFieldMappings, context);
            this.AddFieldMappings(values, mappings, context.Record);
            HttpClient client = this._httpClientFactory.CreateClient();
            HttpMethod method = new HttpMethod(this.Method);
            HttpRequestMessage request = new HttpRequestMessage(method, new Uri(this.Url));
            if (!string.IsNullOrEmpty(this.Username) && !string.IsNullOrEmpty(this.Password))
            {
                string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(this.Username + ":" + this.Password));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
            }
            if (method == HttpMethod.Get)
            {
                UriBuilder uriBuilder = new UriBuilder(this.Url);
                NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
                foreach (string allKey in values.AllKeys)
                    queryString.Add(allKey, values[allKey]);
                uriBuilder.Query = queryString.ToString();
                request.RequestUri = uriBuilder.Uri;
            }
            else
                request.Content = new FormUrlEncodedContent(values.AllKeys.Where<string>(key => key != null).Select<string, KeyValuePair<string, string>>(key => new KeyValuePair<string, string>(key, values[key])));
            try
            {
                HttpResponseMessage httpResponseMessage = await client.SendAsync(request).ConfigureAwait(false);
                if (httpResponseMessage.IsSuccessStatusCode)
                    return WorkflowExecutionStatus.Completed;
                ILogger logger = _logger;
                object obj1 = context.Record.UniqueId;
                object obj2 = context.Form.Name;
                object obj3 = context.Form.Id;
                object obj4 = Url;
                object obj5 = Method;
                object obj6 = httpResponseMessage.StatusCode;
                object obj7 = httpResponseMessage.ReasonPhrase;
                string str = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogError("There was a problem sending the record with unique id {RecordId} from the Form {FormName} with id {FormId} to the URL Endpoint {Url} with the method {Method}. Status code received from request: {StatusCode}, reason: {Reason}, content: {Content} ", obj1, obj2, obj3, obj4, obj5, obj6, obj7, str);
                logger = null;
                obj1 = null;
                obj2 = null;
                obj3 = null;
                obj4 = null;
                obj5 = null;
                obj6 = null;
                obj7 = null;
                return WorkflowExecutionStatus.Failed;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "There was a problem sending the record with unique id {RecordId} from the Form {FormName} with id {FormId} to the URL Endpoint {Url} with the method {Method}", context.Record.UniqueId, context.Form.Name, context.Form.Id, Url, Method);
                return WorkflowExecutionStatus.Failed;
            }
        }

        private void AddStandardFieldMappings(
          NameValueCollection values,
          List<StandardFieldMapping> standardFieldMappings,
          WorkflowExecutionContext context)
        {
            foreach (StandardFieldMapping standardFieldMapping in standardFieldMappings.Where<StandardFieldMapping>(x => x.Include))
            {
                switch (standardFieldMapping.Field)
                {
                    case StandardField.FormId:
                        values.Add(standardFieldMapping.KeyName, context.Form.Id.ToString());
                        continue;
                    case StandardField.FormName:
                        values.Add(standardFieldMapping.KeyName, context.Form.Name);
                        continue;
                    case StandardField.PageUrl:
                        UmbracoHelper umbracoHelper;
                        if (this._umbracoHelperAccessor.TryGetUmbracoHelper(out umbracoHelper))
                        {
                            IPublishedContent content = umbracoHelper.Content(context.Record.UmbracoPageId);
                            if (content != null)
                            {
                                string str = content.Url(this._publishedUrlProvider, mode: UrlMode.Absolute);
                                values.Add(standardFieldMapping.KeyName, str);
                                continue;
                            }
                            continue;
                        }
                        continue;
                    case StandardField.SubmissionDate:
                        values.Add(standardFieldMapping.KeyName, context.Record.Created.ToString());
                        continue;
                    default:
                        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(62, 1);
                        interpolatedStringHandler.AppendLiteral("The field '");
                        interpolatedStringHandler.AppendFormatted<StandardField>(standardFieldMapping.Field);
                        interpolatedStringHandler.AppendLiteral("' is not supported for including in the collection.");
                        throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
                }
            }
        }

        private void AddFieldMappings(
          NameValueCollection values,
          List<FieldMapping> mappings,
          Record record)
        {
            if (mappings.Any<FieldMapping>())
            {
                foreach (FieldMapping mapping in mappings)
                {
                    string alias = mapping.Alias;
                    if (!string.IsNullOrEmpty(mapping.StaticValue))
                    {
                        string staticValue = mapping.StaticValue;
                        values.Add(alias, staticValue);
                    }
                    else if (!string.IsNullOrEmpty(mapping.Value))
                    {
                        RecordField recordField = record.GetRecordField(new Guid(mapping.Value));
                        if (recordField != null)
                        {
                            string str = recordField.ValuesAsString(false);
                            values.Add(alias, str);
                        }
                        else
                            this._logger.LogWarning("Workflow {WorkflowName}: The field mapping with alias, {FieldMappingAlias}, did not match any record fields. This is probably caused by the record field being marked as sensitive and the workflow has been set not to include sensitive data", this.Workflow?.Name, mapping.Alias);
                    }
                    else
                        values.Add(alias, string.Empty);
                }
            }
            else
            {
                foreach (RecordField recordField in record.RecordFields.Values)
                {
                    if (recordField.Field != null)
                        values.Add(this.GetElementName(recordField.Field), recordField.ValuesAsString());
                }
            }
        }

        private string GetElementName(Field field)
        {
            string elementForFields = this.DefaultElementForFields;
            if (elementForFields == "Alias")
                return field.Alias;
            if (elementForFields == "Caption")
                ;
            return field.Caption.Replace(" ", string.Empty);
        }
    }
}
