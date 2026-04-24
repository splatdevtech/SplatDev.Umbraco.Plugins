
// Type: Umbraco.Forms.Web.Api.ManagementApi.Picker.UpdateMappedDocumentTypePropertiesController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;
using Umbraco.Forms.Web.Models.ManagementApi.Picker;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Picker
{
  public class UpdateMappedDocumentTypePropertiesController : DocumentTypePickerControllerBase
  {
    public UpdateMappedDocumentTypePropertiesController(IContentTypeService contentTypeService)
      : base(contentTypeService)
    {
    }

    [HttpPost("mappings/refresh")]
    [ProducesResponseType(typeof (IEnumerable<MappedDocumentTypePropertyModel>), 200)]
    [ProducesResponseType(404)]
    public IActionResult RefreshMappedProperties(MappedDocumentTypeModel data)
    {
      IContentType docType = this.ContentTypeService.Get(data.DoctypeAlias);
      if (docType == null)
        return (IActionResult) this.NotFound();
      IList<PickerItem> propertiesForDocumentType = DocumentTypePickerControllerBase.GetPropertiesForDocumentType(docType);
      List<MappedDocumentTypePropertyModel> source = new List<MappedDocumentTypePropertyModel>();
      foreach (MappedDocumentTypePropertyModel currentProperty1 in data.CurrentProperties)
      {
        MappedDocumentTypePropertyModel currentProperty = currentProperty1;
        PickerItem pickerItem = propertiesForDocumentType.SingleOrDefault<PickerItem>((Func<PickerItem, bool>) (x => x.Id == currentProperty.Id));
        if (pickerItem != null)
          source.Add(new MappedDocumentTypePropertyModel()
          {
            Id = currentProperty.Id,
            Value = pickerItem.Value,
            Field = currentProperty.Field,
            StaticValue = currentProperty.StaticValue
          });
      }
      foreach (PickerItem pickerItem in (IEnumerable<PickerItem>) propertiesForDocumentType)
      {
        PickerItem docTypeProperty = pickerItem;
        if (source.SingleOrDefault<MappedDocumentTypePropertyModel>((Func<MappedDocumentTypePropertyModel, bool>) (x => x.Id == docTypeProperty.Id)) == null)
          source.Add(new MappedDocumentTypePropertyModel()
          {
            Id = docTypeProperty.Id,
            Value = docTypeProperty.Value,
            Field = string.Empty,
            StaticValue = string.Empty
          });
      }
      return (IActionResult) this.Ok((object) source.OrderBy<MappedDocumentTypePropertyModel, string>((Func<MappedDocumentTypePropertyModel, string>) (x => x.Value)).ToList<MappedDocumentTypePropertyModel>());
    }
  }
}
