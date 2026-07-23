using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core.Security;

using Umbraco.Cms.Core.Services;

using Umbraco.Cms.Core.Strings;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for updating a prevalue source.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    public class UpdatePrevalueSourceController(
      IPrevalueSourceService prevalueSourceService,
      FieldPrevalueSourceCollection fieldPreValueSources,
      IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService,
      ITemporaryFileService temporaryFileService,
      IShortStringHelper shortStringHelper,
      IFileStreamSecurityValidator fileStreamSecurityValidator,
      IPreValueTextFileStorage preValueTextFileStorage) : CreateOrUpdatePrevalueSourceControllerBase(prevalueSourceService, fieldPreValueSources, fieldPreValueSourceTypeService, temporaryFileService, shortStringHelper, fileStreamSecurityValidator, preValueTextFileStorage)
    {

        /// <summary>Management API endpoint for updating a prevalue source.</summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(typeof(ProblemDetails), 403)]
        [ProducesResponseType(typeof(ProblemDetails), 500)]
        public async Task<IActionResult> Update(
          Guid id,
          FieldPrevalueSource preValueSource)
        {
            _ = id;
            UpdatePrevalueSourceController sourceController = this;
            switch (sourceController.TryValidateProviderType(preValueSource, out ProblemDetails? problemDetails))
            {
                case ProviderValidationResult.FailedTypeNotFound:
                    return sourceController.NotFound();

                case ProviderValidationResult.FailedValidation:
                    return sourceController.BadRequest(problemDetails);

                default:
                    TransformSettingsResult transformSettingsResult = await sourceController.TransformSettings(preValueSource);
                    if (!transformSettingsResult.Success)
                        return sourceController.BadRequest(transformSettingsResult.ProblemDetails);
                    sourceController.PrevalueSourceService.Update(preValueSource);
                    return sourceController.Ok();
            }
        }
    }
}