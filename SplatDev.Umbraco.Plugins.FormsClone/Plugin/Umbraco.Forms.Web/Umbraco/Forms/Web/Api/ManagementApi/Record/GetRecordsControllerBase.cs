
// Type: Umbraco.Forms.Web.Api.ManagementApi.Record.GetRecordsControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Umbraco.Forms.Core.Data.Storage;
using Umbraco.Forms.Core.Searchers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Record
{
  public abstract class GetRecordsControllerBase : RecordControllerBase
  {
    protected GetRecordsControllerBase(
      IFormService formService,
      IRecordStorage recordStorage,
      IFormsSecurity formsSecurity,
      IPlaceholderParsingService placeholderParsingService,
      IFormRecordSearcher formRecordSearcher)
      : base(formService, recordStorage, formsSecurity, placeholderParsingService)
    {
      this.FormRecordSearcher = formRecordSearcher;
    }

    protected IFormRecordSearcher FormRecordSearcher { get; }
  }
}
