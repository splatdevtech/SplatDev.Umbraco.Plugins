
// Type: Umbraco.Forms.Web.Api.ManagementApi.FieldType.GetValidationPatternsController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.FieldType
{
  [Authorize(Policy = "ManageForms")]
  public class GetValidationPatternsController : FieldTypeControllerBase
  {
    private readonly ValidationPatternCollection _validationPatterns;

    public GetValidationPatternsController(
      IFieldTypeStorage fieldTypeStorage,
      ValidationPatternCollection validationPatterns)
      : base(fieldTypeStorage)
    {
      this._validationPatterns = validationPatterns;
    }

    [HttpGet("validation-pattern")]
    [ProducesResponseType(typeof (IEnumerable<ValidationPattern>), 200)]
    public IActionResult GetValidationPatterns() => (IActionResult) this.Ok((object) this._validationPatterns.Select<IValidationPattern, ValidationPattern>((Func<IValidationPattern, ValidationPattern>) (x => new ValidationPattern()
    {
      Name = x.Name,
      Pattern = x.Pattern,
      LabelKey = x.LabelKey
    })));
  }
}
