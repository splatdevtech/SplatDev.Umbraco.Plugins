
// Type: Umbraco.Forms.Web.Api.ManagementApi.FieldType.GetDataTypeForRichTextController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.FieldType
{
  [Authorize(Policy = "ManageForms")]
  public class GetDataTypeForRichTextController : FieldTypeControllerBase
  {
    private readonly IDataTypeService _dataTypeService;
    private readonly IUmbracoMapper _umbracoMapper;
    private readonly RichTextSettings _richTextSettings;

    public GetDataTypeForRichTextController(
      IFieldTypeStorage fieldTypeStorage,
      IDataTypeService dataTypeService,
      IUmbracoMapper umbracoMapper,
      IOptions<RichTextSettings> richTextSettings)
      : base(fieldTypeStorage)
    {
      this._dataTypeService = dataTypeService;
      this._umbracoMapper = umbracoMapper;
      this._richTextSettings = richTextSettings.Value;
    }

    [HttpGet("richtext-datatype")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof (ProblemDetails), 404)]
    public async Task<IActionResult> GetDataTypeForRichTextField()
    {
      GetDataTypeForRichTextController richTextController = this;
      IDataType async = await richTextController._dataTypeService.GetAsync(richTextController._richTextSettings.DataTypeId);
      if (!(async?.EditorAlias == "Umbraco.RichText"))
        return (IActionResult) richTextController.NotFound();
      IDictionary<string, object> dictionary = async.Editor?.GetConfigurationEditor()?.ToConfigurationEditor(async.ConfigurationData) ?? (IDictionary<string, object>) new Dictionary<string, object>();
      return (IActionResult) richTextController.Ok((object) new DataTypeDetail()
      {
        Id = async.Id,
        Key = async.Key,
        Name = (async.Name ?? string.Empty),
        ConfigurationData = dictionary
      });
    }
  }
}
