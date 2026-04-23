
// Type: Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource.GetCollectionPrevalueSourceController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Api.Common.ViewModels.Pagination;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource
{
  [Authorize(Policy = "SectionAccessForms")]
  public class GetCollectionPrevalueSourceController : PrevalueSourceControllerBase
  {
    public GetCollectionPrevalueSourceController(
      IPrevalueSourceService prevalueSourceService,
      FieldPreValueSourceCollection fieldPreValueSources,
      IFieldPreValueSourceTypeService fieldPreValueSourceTypeService)
      : base(fieldPreValueSources, prevalueSourceService, fieldPreValueSourceTypeService)
    {
    }

    [HttpGet]
    [ProducesResponseType(typeof (PagedViewModel<FieldPreValueSource>), 200)]
    public IActionResult GetCollection(int skip = 0, int take = 2147483647)
    {
      List<FieldPreValueSource> list = this.PrevalueSourceService.Get().OrderBy<FieldPreValueSource, string>((Func<FieldPreValueSource, string>) (x => x.Name)).ToList<FieldPreValueSource>();
      return (IActionResult) this.Ok((object) new PagedViewModel<FieldPreValueSource>()
      {
        Total = (long) list.Count,
        Items = list.Skip<FieldPreValueSource>(skip).Take<FieldPreValueSource>(take)
      });
    }
  }
}
