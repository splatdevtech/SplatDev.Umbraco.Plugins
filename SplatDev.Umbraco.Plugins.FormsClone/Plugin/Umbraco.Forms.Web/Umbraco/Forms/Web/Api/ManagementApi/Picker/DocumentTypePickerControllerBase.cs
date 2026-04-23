
// Type: Umbraco.Forms.Web.Api.ManagementApi.Picker.DocumentTypePickerControllerBase
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
  [Route("/umbraco/forms/management/api/v1/picker/document-type")]
  public abstract class DocumentTypePickerControllerBase : PickerControllerBase
  {
    public DocumentTypePickerControllerBase(IContentTypeService contentTypeService) => this.ContentTypeService = contentTypeService;

    public IContentTypeService ContentTypeService { get; }

    protected static IList<PickerItem> GetPropertiesForDocumentType(
      IContentType docType)
    {
      List<PickerItem> source = new List<PickerItem>();
      foreach (PropertyType compositionPropertyType in docType.CompositionPropertyTypes)
      {
        PickerItem pickerItem = new PickerItem()
        {
          Id = compositionPropertyType.Alias,
          Value = compositionPropertyType.Name
        };
        if (!source.Contains(pickerItem))
          source.Add(pickerItem);
      }
      return (IList<PickerItem>) source.OrderBy<PickerItem, string>((Func<PickerItem, string>) (x => x.Value)).ToList<PickerItem>();
    }
  }
}
