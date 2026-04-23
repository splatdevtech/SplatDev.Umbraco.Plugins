using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with prevalue sources.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Prevalue Source")]
    [Route("/formBuilder/management/api/v1/prevalue-source")]
    [Authorize(Policy = "SectionAccessForms")]
    public abstract class PrevalueSourceControllerBase(
      FieldPrevalueSourceCollection fieldPreValueSources,
      IPrevalueSourceService prevalueSourceService,
      IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService) : FormsManagementApiControllerBase
    {
        /// <summary>
        /// Gets the         /// </summary>
        protected IPrevalueSourceService PrevalueSourceService { get; } = prevalueSourceService;

        /// <summary>
        /// Gets the         /// </summary>
        protected FieldPrevalueSourceCollection FieldPrevalueSourceCollection { get; } = fieldPreValueSources;

        /// <summary>
        /// Gets the         /// </summary>
        protected IFieldPrevalueSourceTypeService FieldPrevalueSourceTypeService { get; } = fieldPreValueSourceTypeService;
    }
}