using Examine.Lucene;
using Examine.Lucene.Analyzers;

using FormBuilder.Examine.Validators;

using Microsoft.Extensions.Options;

using Umbraco.Cms.Infrastructure.Examine;

namespace FormBuilder.Examine
{
    public class ConfigureFormBuilderIndexOptions :
      IConfigureNamedOptions<LuceneDirectoryIndexOptions>,
      IConfigureOptions<LuceneDirectoryIndexOptions>
    {
        public void Configure(LuceneDirectoryIndexOptions options) => throw new NotSupportedException("This is never called and is just part of the interface");

        public void Configure(string? name, LuceneDirectoryIndexOptions options)
        {
            if (!(name == "FormBuildersRecordsIndex"))
                return;
            options.Analyzer = new CultureInvariantWhitespaceAnalyzer();
            options.Validator = new RecordValueSetValidator();
            options.FieldDefinitions = new UmbracoFieldDefinitionCollection();
        }
    }
}