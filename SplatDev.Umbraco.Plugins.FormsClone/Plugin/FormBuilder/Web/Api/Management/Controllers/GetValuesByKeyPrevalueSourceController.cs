using FormBuilder.Core.Models;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using System.Runtime.CompilerServices;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving the values of a single prevalue source by Id.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "SectionAccessForms")]
    [method: ActivatorUtilitiesConstructor]
    public class GetValuesByKeyPrevalueSourceController(
      IPrevalueSourceService prevalueSourceService,
      FieldPrevalueSourceCollection fieldPreValueSources,
      IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService,
      IFormService formService) : PrevalueSourceControllerBase(fieldPreValueSources, prevalueSourceService, fieldPreValueSourceTypeService)
    {
        private readonly IFormService _formService = formService;

        /// <summary>
        /// Management API endpoint for retrieving the values of a single prevalue source by Id.
        /// </summary>
        [HttpGet("{id:guid}/values")]
        [ProducesResponseType(typeof(IEnumerable<Prevalue>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetValues(
          Guid id,
          Guid? formId = null,
          Guid? fieldId = null)
        {
            GetValuesByKeyPrevalueSourceController sourceController = this;
            Form? form = null;
            Field? field = null;
            if (formId.HasValue)
            {
                form = sourceController._formService.Get(formId.Value);
                if (fieldId.HasValue)
                    field = form?.AllFields.FirstOrDefault(f =>
                    {
                        Guid id1 = f.Id;
                        Guid? nullable = fieldId;
                        return nullable.HasValue && id1 == nullable.GetValueOrDefault();
                    });
            }
            FieldPrevalueSource? fieldPreValueSource = sourceController.PrevalueSourceService.Get(id);
            if (fieldPreValueSource is null)
                return sourceController.NotFound();
            FieldPrevalueSourceType? byId = sourceController.FieldPrevalueSourceTypeService.GetById(fieldPreValueSource.FieldPrevalueSourceTypeId);
            if (byId is null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(44, 1);
                interpolatedStringHandler.AppendLiteral("Could not load prevalue source type with Id ");
                interpolatedStringHandler.AppendFormatted(fieldPreValueSource.FieldPrevalueSourceTypeId);
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            byId.LoadSettings(fieldPreValueSource);
            List<Prevalue> preValuesAsync = await byId.GetPreValuesAsync(field, form);
            return sourceController.Ok(preValuesAsync);
        }
    }
}