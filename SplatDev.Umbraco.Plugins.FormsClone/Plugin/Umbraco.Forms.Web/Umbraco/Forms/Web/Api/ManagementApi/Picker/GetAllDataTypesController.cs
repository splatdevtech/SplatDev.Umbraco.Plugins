
// Type: Umbraco.Forms.Web.Api.ManagementApi.Picker.GetAllDataTypesController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Picker
{
  [Route("/umbraco/forms/management/api/v1/picker/data-type")]
  public class GetAllDataTypesController : PickerControllerBase
  {
    private readonly IDataTypeService _dataTypeService;

    public GetAllDataTypesController(IDataTypeService dataTypeService) => this._dataTypeService = dataTypeService;

    [HttpGet]
    [ProducesResponseType(typeof (IEnumerable<PickerItem>), 200)]
    public async Task<IActionResult> GetAll()
    {
      GetAllDataTypesController dataTypesController = this;
      List<PickerItem> list = new List<PickerItem>();
      foreach (IDataType dataType in await dataTypesController._dataTypeService.GetAllAsync())
        list.Add(new PickerItem()
        {
          Id = dataType.Id.ToString(),
          Value = dataType.Name ?? string.Empty
        });
      IActionResult all = (IActionResult) dataTypesController.Ok((object) list.OrderBy<PickerItem, string>((Func<PickerItem, string>) (x => x.Value)).ToList<PickerItem>());
      list = (List<PickerItem>) null;
      return all;
    }
  }
}
