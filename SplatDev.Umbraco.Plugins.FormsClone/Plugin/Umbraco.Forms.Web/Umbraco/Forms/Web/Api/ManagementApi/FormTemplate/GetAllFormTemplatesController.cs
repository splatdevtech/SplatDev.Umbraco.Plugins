
// Type: Umbraco.Forms.Web.Api.ManagementApi.FormTemplate.GetAllFormTemplatesController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Data.Storage;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.FormTemplate
{
  public class GetAllFormTemplatesController : FormTemplateControllerBase
  {
    public GetAllFormTemplatesController(IFormTemplateStorage formTemplateStorage)
      : base(formTemplateStorage)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof (IEnumerable<FormTemplateBase>), 200)]
    public IActionResult GetAll() => (IActionResult) this.Ok((object) this.FormTemplateStorage.GetAllTemplates());
  }
}
