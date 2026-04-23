
// Type: Umbraco.Forms.Core.Providers.WorkflowTypes.SaveAsFile
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Xml;

using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.IO;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Data.Helpers;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.WorkflowTypes
{
    public class SaveAsFile : WorkflowType
    {
        private readonly IXmlService _xmlService;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<SaveAsFile> _logger;
        private readonly MediaFileManager _mediaFileManager;

        public SaveAsFile(
          IXmlService xmlService,
          IHostEnvironment hostEnvironment,
          ILogger<SaveAsFile> logger,
          MediaFileManager mediaFileManager)
        {
            this.Id = new Guid("9CC5854D-61A2-48F6-9F4A-8F3BDFAFB521");
            this.Name = "Save as an XML file";
            this.Alias = "saveAsAnXmlFile";
            this.Description = "Saves the result of the form as an XML file via XSLT";
            this.Icon = "icon-download-alt";
            this.Group = "Legacy";
            this._xmlService = xmlService;
            this._hostEnvironment = hostEnvironment;
            this._logger = logger;
            this._mediaFileManager = mediaFileManager;
        }

        [Setting("Path", Description = "Path to place the file", DisplayOrder = 10, IsMandatory = true, PreValues = "/data/records", SupportsPlaceholders = true)]
        public virtual string Path { get; set; } = string.Empty;

        [Setting("Extension", Description = "File extension", DisplayOrder = 20, IsMandatory = true, PreValues = "xml", SupportsPlaceholders = true)]
        public virtual string Extension { get; set; } = string.Empty;

        [Setting("XSLT File", Description = "Transform the XML before saving it", DisplayOrder = 30, View = "File")]
        public virtual string XsltFile { get; set; } = string.Empty;

        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            try
            {
                string str = this._xmlService.ToXml(context.Record, new XmlDocument()).OuterXml;
                string path = this._hostEnvironment.MapPathContentRoot(this.Path);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (!string.IsNullOrEmpty(this.XsltFile))
                    str = XsltHelper.TransformXML(str, this.XsltFile, this._mediaFileManager);
                await File.WriteAllTextAsync(path.Trim('\\') + "\\" + context.Record.UniqueId.ToString() + "." + (string.IsNullOrEmpty(this.Extension) ? "xml" : this.Extension.TrimStart('.')), str).ConfigureAwait(false);
                return WorkflowExecutionStatus.Completed;
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "There was a problem trying to save the Record with unique id {RecordId} for Form {FormName} with id {FormId} entry to a file at {Path} using the XSLT file {XsltFile}", context.Record.UniqueId, context.Form.Name, context.Form.Id, Path, XsltFile);
                return WorkflowExecutionStatus.Failed;
            }
        }

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = new List<Exception>();
            if (string.IsNullOrEmpty(this.Path))
                exceptionList.Add(new Exception("'Path' setting has not been set"));
            if (string.IsNullOrEmpty(this.Extension))
                exceptionList.Add(new Exception("'Extension' setting has not been set"));
            if (!string.IsNullOrEmpty(this.XsltFile) && !this.XsltFile.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase))
                exceptionList.Add(new Exception("'XSLT File' setting has not been set correctly (a file with an .xslt extension must be selected)."));
            return exceptionList;
        }
    }
}
