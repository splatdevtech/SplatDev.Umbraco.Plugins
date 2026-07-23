
// Type: Umbraco.Forms.Core.Providers.ParsedPlaceholderFormatterCollectionBuilder
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Umbraco.Cms.Core.Composing;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Providers
{
  public class ParsedPlaceholderFormatterCollectionBuilder : 
    LazyCollectionBuilderBase<ParsedPlaceholderFormatterCollectionBuilder, ParsedPlaceholderFormatterCollection, IParsedPlaceholderFormatter>
  {
    protected override ParsedPlaceholderFormatterCollectionBuilder This => this;
  }
}
