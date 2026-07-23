
// Type: Umbraco.Forms.Web.Api.ManagementApi.Picker.GetAllDocumentTypesController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Picker
{
  public class GetAllDocumentTypesController : DocumentTypePickerControllerBase
  {
    public GetAllDocumentTypesController(IContentTypeService contentTypeService)
      : base(contentTypeService)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof (IEnumerable<PickerItem>), 200)]
    public IActionResult GetAll()
    {
      List<PickerItem> source = new List<PickerItem>();
      foreach (IContentType contentType in this.ContentTypeService.GetAll().Where<IContentType>((Func<IContentType, bool>) (x => !x.IsElement)))
      {
        PickerItem pickerItem = new PickerItem()
        {
          Id = contentType.Alias,
          Value = contentType.Name ?? string.Empty
        };
        source.Add(pickerItem);
      }
      return (IActionResult) this.Ok((object) source.OrderBy<PickerItem, string>((Func<PickerItem, string>) (x => x.Value)).ToList<PickerItem>());
    }
  }
}
