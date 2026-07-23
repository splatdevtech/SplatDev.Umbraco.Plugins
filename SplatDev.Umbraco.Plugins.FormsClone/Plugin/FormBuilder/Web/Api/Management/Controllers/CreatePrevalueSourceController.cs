using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Mvc;

using System.Linq.Expressions;

using Umbraco.Cms.Core.Security;

using Umbraco.Cms.Core.Services;

using Umbraco.Cms.Core.Strings;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for creating a prevalue source.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the         /// </remarks>
    public class CreatePrevalueSourceController(
      IPrevalueSourceService prevalueSourceService,
      FieldPrevalueSourceCollection fieldPreValueSources,
      IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService,
      ITemporaryFileService temporaryFileService,
      IShortStringHelper shortStringHelper,
      IFileStreamSecurityValidator fileStreamSecurityValidator,
      IPreValueTextFileStorage preValueTextFileStorage) : CreateOrUpdatePrevalueSourceControllerBase(prevalueSourceService, fieldPreValueSources, fieldPreValueSourceTypeService, temporaryFileService, shortStringHelper, fileStreamSecurityValidator, preValueTextFileStorage)
    {

        /// <summary>Management API endpoint for creating a prevalue source.</summary>
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(typeof(ProblemDetails), 400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Create(FieldPrevalueSource preValueSource)
        {
            CreatePrevalueSourceController sourceController = this;
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
                    sourceController.PrevalueSourceService.Insert(preValueSource);
                    return CreatedAtAction((Expression<Func<GetByKeyPrevalueSourceController, string>>)(controller => "GetByKey"), preValueSource.Id);
            }
        }
    }
}