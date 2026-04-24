
// Type: Umbraco.Forms.Core.Providers.Export.ExportToExcel
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using System;
using Umbraco.Forms.Core.Searchers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Providers.Export
{
  public class ExportToExcel : BaseExportToExcel
  {
    public ExportToExcel(
      IHostEnvironment hostEnvironment,
      IHttpContextAccessor httpContextAccessor,
      IFormRecordSearcher formRecordSearcher,
      IFormService formService,
      IFieldTypeStorage fieldTypeStorage,
      IPrevalueSourceService prevalueSourceService,
      IFieldPreValueSourceTypeService fieldPreValueSourceTypeService)
      : base(hostEnvironment, httpContextAccessor, formRecordSearcher, formService, fieldTypeStorage, prevalueSourceService, fieldPreValueSourceTypeService)
    {
      this.Description = "Exports all submitted values for the form to Excel, in a format useful for integration with other systems.";
      this.Name = "Excel File (submitted values)";
      this.Alias = "excelFileSubmittedValues";
      this.Id = new Guid("94ED105A-87B3-4e1f-97CB-9A320AEE2745");
    }

    protected override bool ReplacePrevalueCaptions => false;
  }
}
