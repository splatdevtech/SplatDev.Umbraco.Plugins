
// Type: Umbraco.Forms.Core.Providers.WorkflowTypes.PostAsXml
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Logging;

using System.Collections.Specialized;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Xml;

using Umbraco.Cms.Core.IO;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Data.Helpers;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Providers.Extensions;
using Umbraco.Forms.Core.Providers.Models;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.WorkflowTypes
{
    public class PostAsXml : WorkflowType
    {
        private readonly IXmlService _xmlService;
        private readonly ILogger<PostAsXml> _logger;
        private readonly MediaFileManager _mediaFileManager;
        private readonly IHttpClientFactory _httpClientFactory;

        public PostAsXml(
          IXmlService xmlService,
          ILogger<PostAsXml> logger,
          MediaFileManager mediaFileManager,
          IHttpClientFactory httpClientFactory)
        {
            this.Id = new Guid("470EEB3A-CB15-4B08-9FC0-A2F091583332");
            this.Name = "Post as XML";
            this.Alias = "postAsXml";
            this.Description = "Posts the form as XML to a URL";
            this.Icon = "icon-code";
            this.Group = "Legacy";
            this._xmlService = xmlService;
            this._logger = logger;
            this._mediaFileManager = mediaFileManager;
            this._httpClientFactory = httpClientFactory;
        }

        [Setting("Url", Description = "Enter the URL to post to", DisplayOrder = 10, IsMandatory = true, SupportsPlaceholders = true)]
        public virtual string Url { get; set; } = string.Empty;

        [Setting("Method", Description = "POST or GET", DisplayOrder = 20, PreValues = "POST,GET,PUT,DELETE", View = "Umb.PropertyEditorUi.Dropdown")]
        public virtual string Method { get; set; } = string.Empty;

        [Setting("XSLT File", Description = "Transform the XML before posting it", DisplayOrder = 30, View = "Umb.PropertyEditorUi.MediaEntityPicker")]
        public virtual string XsltFile { get; set; } = string.Empty;

        [Setting("Fields", Description = "Map the needed fields", DisplayOrder = 40, SupportsPlaceholders = true, View = "Forms.PropertyEditorUi.FieldMapper")]
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
            if (!string.IsNullOrEmpty(this.XsltFile) && !this.XsltFile.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase))
                exceptionList.Add(new Exception("'XSLT File' setting has not been set correctly (a file with an .xslt extension must be selected)"));
            return exceptionList;
        }

        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            PostAsXml postAsXml = this;
            List<FieldMapping> fieldMappingList = new List<FieldMapping>();
            if (!string.IsNullOrEmpty(postAsXml.Fields))
            {
                IEnumerable<FieldMapping> source = JsonSerializer.Deserialize<IEnumerable<FieldMapping>>(postAsXml.Fields, FormsJsonSerializerOptions.Default);
                if (source != null)
                    fieldMappingList = source.ToList<FieldMapping>();
            }
            NameValueCollection nameValueCollection = new NameValueCollection();
            if (fieldMappingList.Any<FieldMapping>())
            {
                // ISSUE: explicit non-virtual call
                nameValueCollection = fieldMappingList.ToNameValueCollection(context, postAsXml._logger, postAsXml.Workflow?.Name ?? "Workflow");
            }
            string str1 = postAsXml._xmlService.ToXml(context.Record, new XmlDocument(), postAsXml.DefaultElementForFields).OuterXml;
            if (!string.IsNullOrEmpty(postAsXml.XsltFile))
                str1 = XsltHelper.TransformXML(str1, postAsXml.XsltFile, postAsXml._mediaFileManager);
            HttpClient client = postAsXml._httpClientFactory.CreateClient();
            HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(postAsXml.Method), new Uri(postAsXml.Url));
            if (!string.IsNullOrEmpty(postAsXml.Username) && !string.IsNullOrEmpty(postAsXml.Password))
            {
                string base64String = Convert.ToBase64String(Encoding.UTF8.GetBytes(postAsXml.Username + ":" + postAsXml.Password));
                request.Headers.Authorization = new AuthenticationHeaderValue("Basic", base64String);
            }
            foreach (string name in nameValueCollection.AllKeys.Where<string>(x => x != null))
                request.Headers.Add(name, nameValueCollection[name]);
            request.Content = new StringContent(str1, Encoding.UTF8, "text/xml");
            try
            {
                HttpResponseMessage httpResponseMessage = await client.SendAsync(request).ConfigureAwait(false);
                if (httpResponseMessage.IsSuccessStatusCode)
                    return WorkflowExecutionStatus.Completed;
                ILogger logger = postAsXml._logger;
                object obj1 = context.Record.UniqueId;
                object obj2 = context.Form.Name;
                object obj3 = context.Form.Id;
                object obj4 = postAsXml.Url;
                object obj5 = postAsXml.Method;
                object obj6 = httpResponseMessage.StatusCode;
                object obj7 = httpResponseMessage.ReasonPhrase;
                string str2 = await httpResponseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
                logger.LogError("There was a problem sending the record as XML with unique id {RecordId} from the Form {FormName} with id {FormId} to the URL Endpoint {Url} with the method {Method}. Status code received from request: {StatusCode}, reason: {Reason}, content: {Content} ", obj1, obj2, obj3, obj4, obj5, obj6, obj7, str2);
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
                postAsXml._logger.LogError(ex, "There was a problem sending the record as XML with unique id {RecordId} from the Form {FormName} with id {FormId} to the URL Endpoint {Url} with the method {Method}", context.Record.UniqueId, context.Form.Name, context.Form.Id, postAsXml.Url, postAsXml.Method);
                return WorkflowExecutionStatus.Failed;
            }
        }
    }
}
