
// Type: Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource.GetValuesByKeyPrevalueSourceController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

using System.Runtime.CompilerServices;

using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource
{
    [Authorize(Policy = "SectionAccessForms")]
    public class GetValuesByKeyPrevalueSourceController : PrevalueSourceControllerBase
    {
        private readonly IFormService _formService;

        [ActivatorUtilitiesConstructor]
        public GetValuesByKeyPrevalueSourceController(
          IPrevalueSourceService prevalueSourceService,
          FieldPreValueSourceCollection fieldPreValueSources,
          IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
          IFormService formService)
          : base(fieldPreValueSources, prevalueSourceService, fieldPreValueSourceTypeService)
        {
            this._formService = formService;
        }

        [HttpGet("{id:guid}/values")]
        [ProducesResponseType(typeof(IEnumerable<PreValue>), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetValues(
          Guid id,
          Guid? formId = null,
          Guid? fieldId = null)
        {
            GetValuesByKeyPrevalueSourceController sourceController = this;
            Umbraco.Forms.Core.Models.Form form = null;
            Field field = null;
            if (formId.HasValue)
            {
                form = sourceController._formService.Get(formId.Value);
                if (fieldId.HasValue)
                    field = form != null ? form.AllFields.FirstOrDefault<Field>(f =>
                    {
                        Guid id1 = f.Id;
                        Guid? nullable = fieldId;
                        return nullable.HasValue && id1 == nullable.GetValueOrDefault();
                    }) : null;
            }
            FieldPreValueSource fieldPreValueSource = sourceController.PrevalueSourceService.Get(id);
            if (fieldPreValueSource == null)
                return sourceController.NotFound();
            FieldPreValueSourceType byId = sourceController.FieldPreValueSourceTypeService.GetById(fieldPreValueSource.FieldPreValueSourceTypeId);
            if (byId == null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(44, 1);
                interpolatedStringHandler.AppendLiteral("Could not load prevalue source type with Id ");
                interpolatedStringHandler.AppendFormatted<Guid>(fieldPreValueSource.FieldPreValueSourceTypeId);
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            byId.LoadSettings(fieldPreValueSource);
            List<PreValue> preValuesAsync = await byId.GetPreValuesAsync(field, form);
            return sourceController.Ok(preValuesAsync);
        }
    }
}
