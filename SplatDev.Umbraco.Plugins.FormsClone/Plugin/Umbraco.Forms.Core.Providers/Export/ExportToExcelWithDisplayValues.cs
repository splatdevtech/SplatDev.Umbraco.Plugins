
// Type: Umbraco.Forms.Core.Providers.Export.ExportToExcelWithDisplayValues
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
  public class ExportToExcelWithDisplayValues : BaseExportToExcel
  {
    public ExportToExcelWithDisplayValues(
      IHostEnvironment hostEnvironment,
      IHttpContextAccessor httpContextAccessor,
      IFormRecordSearcher formRecordSearcher,
      IFormService formService,
      IFieldTypeStorage fieldTypeStorage,
      IPrevalueSourceService prevalueSourceService,
      IFieldPreValueSourceTypeService fieldPreValueSourceTypeService)
      : base(hostEnvironment, httpContextAccessor, formRecordSearcher, formService, fieldTypeStorage, prevalueSourceService, fieldPreValueSourceTypeService)
    {
      this.Description = "Exports all values for the form to Excel, in a format useful for reviewing the data or creating a report. Captions are used for prevalue data where available.";
      this.Name = "Excel File (display values)";
      this.Alias = "excelFileDisplayValues";
      this.Id = new Guid("688711A2-DC6F-4B51-B8D2-0BB177BB0499");
    }

    protected override bool ReplacePrevalueCaptions => true;
  }
}
