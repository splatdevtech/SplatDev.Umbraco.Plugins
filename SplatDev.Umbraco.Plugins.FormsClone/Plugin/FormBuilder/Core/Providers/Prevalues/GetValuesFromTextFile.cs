using FormBuilder.Core.Attributes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.Extensions.Logging;

namespace FormBuilder.Core.Providers.Prevalues
{
    /// <summary>
    /// Defines a     /// </summary>
    public class GetValuesFromTextFile : FieldPrevalueSourceType
    {
        private readonly IPreValueTextFileStorage _textFileStorage;
        private readonly ILogger<GetValuesFromTextFile> _logger;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public GetValuesFromTextFile(
          IPreValueTextFileStorage preValueTextFileStorage,
          ILogger<GetValuesFromTextFile> logger)
        {
            Id = new Guid("35C2053E-CBF7-4793-B27C-6E97B7671A2D");
            Name = "Get values from textfile";
            Alias = "getValuesFromTextFile";
            Description = "Upload textfile that contains the prevalues";
            Icon = "icon-files";
            _textFileStorage = preValueTextFileStorage;
            _logger = logger;
        }

        /// <summary>
        /// Gets or sets the file containing the prevalues (seperated by linebreak).
        /// </summary>
        [Setting("Text File", Description = "File containing the prevalues (seperated by linebreak)", DisplayOrder = 10, IsMandatory = true, View = "Umb.PropertyEditorUi.UploadField")]
        public virtual string TextFile { get; set; } = string.Empty;

        /// <inheritdoc />
        public override Task<List<Prevalue>> GetPreValuesAsync(Field? field, Form? form)
        {
            List<Prevalue> result = [];
            try
            {
                List<Prevalue> textFilePreValues = _textFileStorage.GetTextFilePreValues(TextFile);
                result.AddRange(textFilePreValues);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "There was a problem retriving PreValues from TextFile {TextFile} for Field {FieldName} in Form {FormName} with an id {FormId}", TextFile, field?.Caption, form?.Name, form?.Id);
                throw;
            }
            return Task.FromResult(result);
        }

        /// <inheritdoc />
        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = [];
            if (string.IsNullOrEmpty(TextFile))
                exceptionList.Add(new Exception("'TextFile' setting is empty'"));
            return exceptionList;
        }
    }
}