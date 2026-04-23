using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with field types.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Field Type")]
    [Authorize(Policy = "SectionAccessForms")]
    [Route("/formBuilder/management/api/v1/field-type")]
    public abstract class FieldTypeControllerBase(IFieldTypeStorage fieldTypeStorage) : FormsManagementApiControllerBase
    {
        /// <summary>
        /// Gets the         /// </summary>
        protected IFieldTypeStorage FieldTypeStorage { get; } = fieldTypeStorage;
    }
}