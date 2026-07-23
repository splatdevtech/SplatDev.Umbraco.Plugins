
// Type: Umbraco.Forms.Core.Data.Helpers.DictionaryHelper
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Umbraco.Cms.Core.Dictionary;


#nullable enable
namespace Umbraco.Forms.Core.Data.Helpers
{
  public class DictionaryHelper : IDictionaryHelper
  {
    private readonly ICultureDictionaryFactory _cultureDictionaryFactory;

    public DictionaryHelper(ICultureDictionaryFactory cultureDictionaryFactory) => this._cultureDictionaryFactory = cultureDictionaryFactory;

    public string GetText(string text)
    {
      if (DictionaryHelper.IsSingleDictionaryReplacement(text))
        return this.ReplaceSingleDictionaryKey(text);
      MatchCollection matches;
      return DictionaryHelper.IsHtmlWithDictionaryReplacements(text, out matches) ? this.ReplaceMatches(text, matches) : text;
    }

    private static bool IsSingleDictionaryReplacement(string text) => text.Trim().StartsWith("#");

    private string ReplaceSingleDictionaryKey(string text)
    {
      string key = text.Trim().TrimStart('#');
      string str = this._cultureDictionaryFactory.CreateDictionary()[key] ?? string.Empty;
      return str.Length <= 0 ? key : str;
    }

    private static bool IsHtmlWithDictionaryReplacements(string text, [NotNullWhen(true)] out MatchCollection? matches)
    {
      if (string.IsNullOrWhiteSpace(text))
      {
        matches = (MatchCollection) null;
        return false;
      }
      Regex regex = new Regex("\\>#(.+?)\\<");
      matches = regex.Matches(text);
      return matches.Count > 0;
    }

    private string ReplaceMatches(string text, MatchCollection matches)
    {
      string str1 = text;
      foreach (Match match in matches)
      {
        string str2 = this.ReplaceSingleDictionaryKey(match.Value.Trim('>', '<', '#'));
        str1 = str1.Replace(match.Value, ">" + str2 + "<");
      }
      return str1;
    }
  }
}
