using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Options;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Providers.Models;
using FormBuilder.Core.Workflows;

using Microsoft.Extensions.Logging;

using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Web;

using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Routing;
using Umbraco.Cms.Web.Common;
using Umbraco.Extensions;

namespace FormBuilder.Core.Providers.WorkflowTypes
{
    /// <summary>
    /// Provides a     /// </summary>
    public class PostToUrl : WorkflowType
    {
        private const string AvailableHttpMethods = "POST,GET,PUT,DELETE";
        private readonly ILogger<PostToUrl> _logger;
        private readonly IUmbracoHelperAccessor _umbracoHelperAccessor;
        private readonly IPublishedUrlProvider _publishedUrlProvider;
        private readonly IHttpClientFactory _httpClientFactory;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public PostToUrl(
          ILogger<PostToUrl> logger,
          IUmbracoHelperAccessor umbracoHelperAccessor,
          IPublishedUrlProvider publishedUrlProvider,
          IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _umbracoHelperAccessor = umbracoHelperAccessor;
            _publishedUrlProvider = publishedUrlProvider;
            Id = new Guid("FD02C929-4E7D-4F90-B9FA-13D074A76688");
            Name = "Send form to URL";
            Alias = "sendFormToUrl";
            Description = "Sends the form to a URL, either as a HTTP POST or GET";
            Icon = "icon-terminal";
            Group = "Legacy";
            _httpClientFactory = httpClientFactory;
        }

        /// <summary>Gets or sets the URL to post to.</summary>
        [Setting("Url", Description = "Enter the URL to post to", DisplayOrder = 10, IsMandatory = true, SupportsPlaceholders = true)]
        public virtual string Url { get; set; } = string.Empty;

        /// <summary>Gets or sets the HTTP method (POST or GET).</summary>
        [Setting("Method", Description = "POST or GET", DisplayOrder = 20, IsMandatory = true, PreValues = "POST,GET,PUT,DELETE", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string Method { get; set; } = string.Empty;

        /// <summary>Gets or sets the standard fields to include.</summary>
        [Setting("Standard Fields", Description = "Map any standard form information to send.", DisplayOrder = 30, SupportsPlaceholders = true, View = "Forms.PropertyEditorUi.StandardFieldMapper")]
        public virtual string StandardFields { get; set; } = string.Empty;

        /// <summary>Gets or sets the fields to include.</summary>
        [Setting("Fields", Description = "Map the needed fields (if not mapped all fields will be sent)", DisplayOrder = 40, SupportsPlaceholders = true, View = "Forms.PropertyEditorUi.FieldMapper")]
        public virtual string Fields { get; set; } = string.Empty;

        /// <summary>Gets or sets the default element name for fields.</summary>
        [Setting("Default Element For Fields", Description = "When no fields are specifically mapped, all fields are provided in the request as XML, using the selected field attribute as the element name.", DisplayOrder = 50, PreValues = "Caption,Alias", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string DefaultElementForFields { get; set; } = "Caption";

        /// <summary>
        /// Gets or sets the username for authentication to the URL the XML is posted to.
        /// </summary>
        [Setting("User", Description = "(optional)", DisplayOrder = 60, SupportsPlaceholders = true)]
        public virtual string Username { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the password for authentication to the URL the XML is posted to.
        /// </summary>
        [Setting("Password", Description = "(optional)", DisplayOrder = 70, SupportsPlaceholders = true, View = "Forms.PropertyEditorUi.Password")]
        public virtual string Password { get; set; } = string.Empty;

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = [];
            if (string.IsNullOrEmpty(Url))
                exceptionList.Add(new Exception("'Url' setting has not been set"));
            if (string.IsNullOrEmpty(Method) || !"POST,GET,PUT,DELETE".Split(',').Contains(Method))
                exceptionList.Add(new Exception("'Method' setting has not been set"));
            return exceptionList;
        }

        /// <inheritdoc />
        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            List<StandardFieldMapping> standardFieldMappings = [];
            if (!string.IsNullOrEmpty(StandardFields))
            {
                IEnumerable<StandardFieldMapping>? source = JsonSerializer.Deserialize<IEnumerable<StandardFieldMapping>>(StandardFields, FormsJsonSerializerOptions.Default);
                if (source is not null)
                    standardFieldMappings = [.. source];
            }
            List<FieldMapping> mappings = [];
            if (!string.IsNullOrEmpty(Fields))
            {
                IEnumerable<FieldMapping>? source = JsonSerializer.Deserialize<IEnumerable<FieldMapping>>(Fields, FormsJsonSerializerOptions.Default);
                if (source is not null)
                    mappings = [.. source];
            }
            NameValueCollection? values = [];
            AddStandardFieldMappings(values, standardFieldMappings, context);
            AddFieldMappings(values, mappings, context.Record);
            HttpClient client = _httpClientFactory.CreateClient();
            HttpMethod method = new(Method);
            HttpRequestMessage request = new(method, new Uri(Url));
            if (!string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password))
            {
                string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(Username + ":" + Password));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
            }
            if (method == HttpMethod.Get)
            {
                UriBuilder uriBuilder = new(Url);
                NameValueCollection queryString = HttpUtility.ParseQueryString(uriBuilder.Query);
                foreach (var allKey in values.AllKeys)
                    queryString.Add(allKey, values[allKey]);
                uriBuilder.Query = queryString.ToString();
                request.RequestUri = uriBuilder.Uri;
            }
            else
                request.Content = new FormUrlEncodedContent(values.AllKeys!.Where(key => key is not null).Select(key => new KeyValuePair<string, string>(key!, values[key]!)));
            try
            {
                HttpResponseMessage httpResponseMessage = await client.SendAsync(request).ConfigureAwait(false);
                if (httpResponseMessage.IsSuccessStatusCode)
                    return WorkflowExecutionStatus.Completed;
                ILogger? logger = _logger;
                object? obj1 = context.Record.UniqueId;
                object? obj2 = context.Form.Name;
                object? obj3 = context.Form.Id;
                object? obj4 = Url;
                object? obj5 = Method;
                object? obj6 = httpResponseMessage.StatusCode;
                object? obj7 = httpResponseMessage.ReasonPhrase;
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
                _logger.LogError(ex, "There was a problem sending the record with unique id {RecordId} from the Form {FormName} with id {FormId} to the URL Endpoint {Url} with the method {Method}", context.Record.UniqueId, context.Form.Name, context.Form.Id, Url, Method);
                return WorkflowExecutionStatus.Failed;
            }
        }

        private void AddStandardFieldMappings(
          NameValueCollection values,
          List<StandardFieldMapping> standardFieldMappings,
          WorkflowExecutionContext context)
        {
            foreach (StandardFieldMapping standardFieldMapping in standardFieldMappings.Where(x => x.Include))
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
                        if (_umbracoHelperAccessor.TryGetUmbracoHelper(out UmbracoHelper? umbracoHelper))
                        {
                            IPublishedContent? content = umbracoHelper.Content(context.Record.UmbracoPageId);
                            if (content is not null)
                            {
                                string str = content.Url(_publishedUrlProvider, mode: UrlMode.Absolute);
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
                        DefaultInterpolatedStringHandler interpolatedStringHandler = new(62, 1);
                        interpolatedStringHandler.AppendLiteral("The field '");
                        interpolatedStringHandler.AppendFormatted(standardFieldMapping.Field);
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
            if (mappings.Count != 0)
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
                        RecordField? recordField = record.GetRecordField(new Guid(mapping.Value));
                        if (recordField is not null)
                        {
                            string str = recordField.ValuesAsString(false);
                            values.Add(alias, str);
                        }
                        else
                            _logger.LogWarning("Workflow {WorkflowName}: The field mapping with alias, {FieldMappingAlias}, did not match any record fields. This is probably caused by the record field being marked as sensitive and the workflow has been set not to include sensitive data", Workflow?.Name, mapping.Alias);
                    }
                    else
                        values.Add(alias, string.Empty);
                }
            }
            else
            {
                foreach (RecordField recordField in record.RecordFields.Values)
                {
                    if (recordField.Field is not null)
                        values.Add(GetElementName(recordField.Field), recordField.ValuesAsString());
                }
            }
        }

        private string GetElementName(Field field)
        {
            string elementForFields = DefaultElementForFields;
            if (elementForFields == "Alias")
                return field.Alias;
            if (elementForFields == "Caption") { }
            return field.Caption.Replace(" ", string.Empty);
        }
    }
}