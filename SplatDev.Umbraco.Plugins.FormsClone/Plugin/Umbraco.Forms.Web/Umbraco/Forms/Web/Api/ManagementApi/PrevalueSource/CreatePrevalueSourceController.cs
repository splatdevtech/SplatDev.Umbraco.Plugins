
// Type: Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource.CreatePrevalueSourceController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Data;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource
{
  public class CreatePrevalueSourceController : CreateOrUpdatePrevalueSourceControllerBase
  {
    public CreatePrevalueSourceController(
      IPrevalueSourceService prevalueSourceService,
      FieldPreValueSourceCollection fieldPreValueSources,
      IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
      ITemporaryFileService temporaryFileService,
      IShortStringHelper shortStringHelper,
      IFileStreamSecurityValidator fileStreamSecurityValidator,
      IPreValueTextFileStorage preValueTextFileStorage)
      : base(prevalueSourceService, fieldPreValueSources, fieldPreValueSourceTypeService, temporaryFileService, shortStringHelper, fileStreamSecurityValidator, preValueTextFileStorage)
    {
    }

    [HttpPost]
    [ProducesResponseType(201)]
    [ProducesResponseType(typeof (ProblemDetails), 400)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> Create(FieldPreValueSource preValueSource)
    {
      CreatePrevalueSourceController sourceController = this;
      ProblemDetails problemDetails;
      switch (sourceController.TryValidateProviderType(preValueSource, out problemDetails))
      {
        case ProviderValidationResult.FailedTypeNotFound:
          return (IActionResult) sourceController.NotFound();
        case ProviderValidationResult.FailedValidation:
          return (IActionResult) sourceController.BadRequest((object) problemDetails);
        default:
          CreateOrUpdatePrevalueSourceControllerBase.TransformSettingsResult transformSettingsResult = await sourceController.TransformSettings(preValueSource);
          if (!transformSettingsResult.Success)
            return (IActionResult) sourceController.BadRequest((object) transformSettingsResult.ProblemDetails);
          sourceController.PrevalueSourceService.Insert(preValueSource);
          return (IActionResult) sourceController.CreatedAtAction<GetByKeyPrevalueSourceController>((Expression<Func<GetByKeyPrevalueSourceController, string>>) (controller => "GetByKey"), preValueSource.Id);
      }
    }
  }
}
