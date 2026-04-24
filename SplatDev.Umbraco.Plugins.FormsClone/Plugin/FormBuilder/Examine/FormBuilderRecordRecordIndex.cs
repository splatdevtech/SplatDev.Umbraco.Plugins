using Examine;
using Examine.Lucene;

using FormBuilder.Examine.Interfaces;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Infrastructure.Examine;

namespace FormBuilder.Examine
{
    public class FormBuilderRecordRecordIndex(
      ILoggerFactory loggerFactory,
      string name,
      IOptionsMonitor<LuceneDirectoryIndexOptions> indexOptions,
      IHostingEnvironment hostingEnvironment,
      IRuntimeState runtimeState) : UmbracoExamineIndex(loggerFactory, name, indexOptions, hostingEnvironment, runtimeState), IFormBuilderRecordIndex, IIndex
    {
    }
}