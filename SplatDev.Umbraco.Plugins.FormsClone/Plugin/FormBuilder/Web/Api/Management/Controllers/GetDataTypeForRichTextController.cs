using FormBuilder.Core.Configuration;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Mapping;

using Umbraco.Cms.Core.Models;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving the data type to use rich text fields.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "ManageForms")]
    public class GetDataTypeForRichTextController(
      IFieldTypeStorage fieldTypeStorage,
      IDataTypeService dataTypeService,
      IUmbracoMapper umbracoMapper,
      IOptions<RichTextSettings> richTextSettings) : FieldTypeControllerBase(fieldTypeStorage)
    {
        private readonly IDataTypeService _dataTypeService = dataTypeService;
        private readonly IUmbracoMapper _umbracoMapper = umbracoMapper;
        private readonly RichTextSettings _richTextSettings = richTextSettings.Value;

        /// <summary>
        /// Management API endpoint for retrieving the data type to use rich text fields.
        /// </summary>
        [HttpGet("richtext-datatype")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 404)]
        public async Task<IActionResult> GetDataTypeForRichTextField()
        {
            GetDataTypeForRichTextController richTextController = this;
            IDataType? async = await richTextController._dataTypeService.GetAsync(richTextController._richTextSettings.DataTypeId);
            if (!(async?.EditorAlias == "Umbraco.RichText"))
                return richTextController.NotFound();
            IDictionary<string, object> dictionary = async.Editor?.GetConfigurationEditor()?.ToConfigurationEditor(async.ConfigurationData) ?? new Dictionary<string, object>();
            return richTextController.Ok(new DataTypeDetail()
            {
                Id = async.Id,
                Key = async.Key,
                Name = async.Name ?? string.Empty,
                ConfigurationData = dictionary
            });
        }
    }
}