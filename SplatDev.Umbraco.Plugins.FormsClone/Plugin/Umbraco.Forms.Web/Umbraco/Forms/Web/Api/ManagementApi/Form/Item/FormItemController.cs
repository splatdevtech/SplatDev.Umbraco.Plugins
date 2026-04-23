
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.Item.FormItemController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.ManagementApi;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form.Item
{
  [ApiExplorerSettings(GroupName = "Form")]
  [Authorize(Policy = "BackOfficeAccess")]
  [Route("/umbraco/forms/management/api/v1/item/form")]
  public class FormItemController : FormsManagementApiControllerBase
  {
    private readonly IFormService _formService;

    public FormItemController(IFormService formService) => this._formService = formService;

    [HttpGet]
    [ProducesResponseType(typeof (IEnumerable<FormItemResponseModel>), 200)]
    public IActionResult Item([FromQuery(Name = "id")] HashSet<Guid> ids)
    {
      List<FormSlim> source = new List<FormSlim>();
      foreach (Guid id in ids)
      {
        FormSlim slim = this._formService.GetSlim(id);
        if (slim != null)
          source.Add(slim);
      }
      return (IActionResult) this.Ok((object) source.Select<FormSlim, FormItemResponseModel>((Func<FormSlim, FormItemResponseModel>) (x =>
      {
        return new FormItemResponseModel()
        {
          Id = x.Id,
          Name = x.Name
        };
      })).ToList<FormItemResponseModel>());
    }
  }
}
