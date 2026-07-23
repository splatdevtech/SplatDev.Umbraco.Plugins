
// Type: Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource.UpdatePrevalueSourceController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using System;
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
  public class UpdatePrevalueSourceController : CreateOrUpdatePrevalueSourceControllerBase
  {
    public UpdatePrevalueSourceController(
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

    [HttpPut("{id:guid}")]
    [ProducesResponseType(200)]
    [ProducesResponseType(typeof (ProblemDetails), 400)]
    [ProducesResponseType(typeof (ProblemDetails), 403)]
    [ProducesResponseType(typeof (ProblemDetails), 500)]
    public async Task<IActionResult> Update(
      Guid id,
      FieldPreValueSource preValueSource)
    {
      UpdatePrevalueSourceController sourceController = this;
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
          sourceController.PrevalueSourceService.Update(preValueSource);
          return (IActionResult) sourceController.Ok();
      }
    }
  }
}
