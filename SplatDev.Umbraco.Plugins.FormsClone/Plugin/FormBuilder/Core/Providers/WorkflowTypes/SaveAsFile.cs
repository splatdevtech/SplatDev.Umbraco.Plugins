using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Helpers;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Workflows;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using System.Xml;

using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.IO;

namespace FormBuilder.Core.Providers.WorkflowTypes
{
    /// <summary>
    /// Provides a     /// </summary>
    public class SaveAsFile : WorkflowType
    {
        private readonly IXmlService _xmlService;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly ILogger<SaveAsFile> _logger;
        private readonly MediaFileManager _mediaFileManager;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public SaveAsFile(
          IXmlService xmlService,
          IHostEnvironment hostEnvironment,
          ILogger<SaveAsFile> logger,
          MediaFileManager mediaFileManager)
        {
            Id = new Guid("9CC5854D-61A2-48F6-9F4A-8F3BDFAFB521");
            Name = "Save as an XML file";
            Alias = "saveAsAnXmlFile";
            Description = "Saves the result of the form as an XML file via XSLT";
            Icon = "icon-download-alt";
            Group = "Legacy";
            _xmlService = xmlService;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _mediaFileManager = mediaFileManager;
        }

        /// <summary>Gets or sets the path to save the file to.</summary>
        [Setting("Path", Description = "Path to place the file", DisplayOrder = 10, IsMandatory = true, PreValues = "/data/records", SupportsPlaceholders = true)]
        public virtual string Path { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the file extension to use for the saved file.
        /// </summary>
        [Setting("Extension", Description = "File extension", DisplayOrder = 20, IsMandatory = true, PreValues = "xml", SupportsPlaceholders = true)]
        public virtual string Extension { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the path to an XSLT file used to transform the XML contents of the file before saving it.
        /// </summary>
        [Setting("XSLT File", Description = "Transform the XML before saving it", DisplayOrder = 30, View = "File")]
        public virtual string XsltFile { get; set; } = string.Empty;

        /// <inheritdoc />
        public override async Task<WorkflowExecutionStatus> ExecuteAsync(
          WorkflowExecutionContext context)
        {
            try
            {
                string str = _xmlService.ToXml(context.Record, new XmlDocument()).OuterXml;
                string path = _hostEnvironment.MapPathContentRoot(Path);
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                if (!string.IsNullOrEmpty(XsltFile))
                    str = XsltHelper.TransformXML(str, XsltFile, _mediaFileManager);
                await File.WriteAllTextAsync(path.Trim('\\') + "\\" + context.Record.UniqueId.ToString() + "." + (string.IsNullOrEmpty(Extension) ? "xml" : Extension.TrimStart('.')), str).ConfigureAwait(false);
                return WorkflowExecutionStatus.Completed;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem trying to save the Record with unique id {RecordId} for Form {FormName} with id {FormId} entry to a file at {Path} using the XSLT file {XsltFile}", context.Record.UniqueId, context.Form.Name, context.Form.Id, Path, XsltFile);
                return WorkflowExecutionStatus.Failed;
            }
        }

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = [];
            if (string.IsNullOrEmpty(Path))
                exceptionList.Add(new Exception("'Path' setting has not been set"));
            if (string.IsNullOrEmpty(Extension))
                exceptionList.Add(new Exception("'Extension' setting has not been set"));
            if (!string.IsNullOrEmpty(XsltFile) && !XsltFile.EndsWith(".xslt", StringComparison.OrdinalIgnoreCase))
                exceptionList.Add(new Exception("'XSLT File' setting has not been set correctly (a file with an .xslt extension must be selected)."));
            return exceptionList;
        }
    }
}