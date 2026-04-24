
// Type: Umbraco.Forms.Examine.Indexes.UmbracoFormsRecordRecordIndex
// Assembly: Umbraco.Forms.Examine, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF5A33E-94A1-42C9-B681-695454D27A51

using Examine;
using Examine.Lucene;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;


#nullable enable
namespace Umbraco.Forms.Examine.Indexes
{
  public class UmbracoFormsRecordRecordIndex : UmbracoExamineIndex, IUmbracoFormsRecordIndex, IIndex
  {
    public UmbracoFormsRecordRecordIndex(
      ILoggerFactory loggerFactory,
      string name,
      IOptionsMonitor<LuceneDirectoryIndexOptions> indexOptions,
      IHostingEnvironment hostingEnvironment,
      IRuntimeState runtimeState)
      : base(loggerFactory, name, indexOptions, hostingEnvironment, runtimeState)
    {
    }
  }
}
