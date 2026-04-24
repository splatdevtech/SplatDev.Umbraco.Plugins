
// Type: Umbraco.Forms.Core.Providers.PreValues.GetValuesFromTextFile
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.Extensions.Logging;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Data;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Providers.PreValues
{
    public class GetValuesFromTextFile : FieldPreValueSourceType
    {
        private readonly IPreValueTextFileStorage _textFileStorage;
        private readonly ILogger<GetValuesFromTextFile> _logger;

        public GetValuesFromTextFile(
          IPreValueTextFileStorage preValueTextFileStorage,
          ILogger<GetValuesFromTextFile> logger)
        {
            this.Id = new Guid("35C2053E-CBF7-4793-B27C-6E97B7671A2D");
            this.Name = "Get values from textfile";
            this.Alias = "getValuesFromTextFile";
            this.Description = "Upload textfile that contains the prevalues";
            this.Icon = "icon-files";
            this._textFileStorage = preValueTextFileStorage;
            this._logger = logger;
        }

        [Setting("Text File", Description = "File containing the prevalues (seperated by linebreak)", DisplayOrder = 10, IsMandatory = true, View = "Umb.PropertyEditorUi.UploadField")]
        public virtual string TextFile { get; set; } = string.Empty;

        public override Task<List<PreValue>> GetPreValuesAsync(Field? field, Form? form)
        {
            List<PreValue> result = new List<PreValue>();
            try
            {
                List<PreValue> textFilePreValues = this._textFileStorage.GetTextFilePreValues(this.TextFile);
                result.AddRange(textFilePreValues);
            }
            catch (Exception ex)
            {
                this._logger.LogError(ex, "There was a problem retriving PreValues from TextFile {TextFile} for Field {FieldName} in Form {FormName} with an id {FormId}", TextFile, field?.Caption, form?.Name, form?.Id);
                throw;
            }
            return Task.FromResult<List<PreValue>>(result);
        }

        public override List<Exception> ValidateSettings()
        {
            List<Exception> exceptionList = new List<Exception>();
            if (string.IsNullOrEmpty(this.TextFile))
                exceptionList.Add(new Exception("'TextFile' setting is empty'"));
            return exceptionList;
        }
    }
}
