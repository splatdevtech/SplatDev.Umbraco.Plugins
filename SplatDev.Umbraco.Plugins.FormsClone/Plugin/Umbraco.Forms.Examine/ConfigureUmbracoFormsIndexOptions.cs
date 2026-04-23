
// Type: Umbraco.Forms.Examine.ConfigureUmbracoFormsIndexOptions
// Assembly: Umbraco.Forms.Examine, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: EDF5A33E-94A1-42C9-B681-695454D27A51

using Examine;
using Examine.Lucene;
using Examine.Lucene.Analyzers;
using Lucene.Net.Analysis;
using Microsoft.Extensions.Options;
using System;
using Umbraco.Cms.Infrastructure.Examine;
using Umbraco.Forms.Examine.Indexes;


#nullable enable
namespace Umbraco.Forms.Examine
{
  public class ConfigureUmbracoFormsIndexOptions : 
    IConfigureNamedOptions<LuceneDirectoryIndexOptions>,
    IConfigureOptions<LuceneDirectoryIndexOptions>
  {
    public void Configure(LuceneDirectoryIndexOptions options) => throw new NotSupportedException("This is never called and is just part of the interface");

    public void Configure(string? name, LuceneDirectoryIndexOptions options)
    {
      if (!(name == "UmbracoFormsRecordsIndex"))
        return;
      ((LuceneIndexOptions) options).Analyzer = (Analyzer) new CultureInvariantWhitespaceAnalyzer();
      ((IndexOptions) options).Validator = (IValueSetValidator) new RecordValueSetValidator();
      ((IndexOptions) options).FieldDefinitions = (FieldDefinitionCollection) new UmbracoFieldDefinitionCollection();
    }
  }
}
