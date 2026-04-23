using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving the available field validation patterns.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "ManageForms")]
    public class GetValidationPatternsController(
      IFieldTypeStorage fieldTypeStorage,
      ValidationPatternCollection validationPatterns) : FieldTypeControllerBase(fieldTypeStorage)
    {
        private readonly ValidationPatternCollection _validationPatterns = validationPatterns;

        /// <summary>
        /// Management API endpoint for retrieving the available field validation patterns.
        /// </summary>
        [HttpGet("validation-pattern")]
        [ProducesResponseType(typeof(IEnumerable<ValidationPattern>), 200)]
        public IActionResult GetValidationPatterns() => Ok(_validationPatterns.Select(x => new ValidationPattern()
        {
            Name = x.Name,
            Pattern = x.Pattern,
            LabelKey = x.LabelKey
        }));
    }
}