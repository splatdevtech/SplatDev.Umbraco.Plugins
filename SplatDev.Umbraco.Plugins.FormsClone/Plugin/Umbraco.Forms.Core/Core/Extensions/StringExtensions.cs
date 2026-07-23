
// Type: Umbraco.Forms.Core.Extensions.StringExtensions
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Extensions
{
  public static class StringExtensions
  {
    public static string ParsePlaceHolders(
      this string text,
      IPlaceholderParsingService placeholderParsingService,
      bool htmlEncodeReplacedValues,
      Record? record = null,
      Form? form = null,
      Hashtable? pageElements = null,
      IDictionary<string, string?>? additionalData = null)
    {
      return placeholderParsingService.ParsePlaceHolders(text, htmlEncodeReplacedValues, record, form, pageElements, additionalData);
    }

    public static string ApplyPrevalueCaptions(
      this string value,
      Guid fieldId,
      Dictionary<Guid, Dictionary<string, string?>> prevalueMaps)
    {
      return string.IsNullOrWhiteSpace(value) || !prevalueMaps.ContainsKey(fieldId) ? value : StringExtensions.ReplaceValueWithPrevalueCaptions(value, prevalueMaps[fieldId]);
    }

    private static string ReplaceValueWithPrevalueCaptions(
      string value,
      Dictionary<string, string?> prevalues)
    {
      foreach (KeyValuePair<string, string> prevalue in prevalues)
      {
        if (prevalue.Key == value)
          return string.IsNullOrWhiteSpace(prevalue.Value) ? prevalue.Key : prevalue.Value;
      }
      if (!value.Contains(", "))
        return value;
      string[] strArray = value.Split(new string[1]{ ", " }, StringSplitOptions.RemoveEmptyEntries);
      List<string> values = new List<string>();
      foreach (string str in strArray)
      {
        bool flag = false;
        foreach (KeyValuePair<string, string> prevalue in prevalues)
        {
          if (prevalue.Key == str)
          {
            values.Add(string.IsNullOrWhiteSpace(prevalue.Value) ? prevalue.Key : prevalue.Value);
            flag = true;
            break;
          }
        }
        if (!flag)
          values.Add(str);
      }
      return string.Join(", ", (IEnumerable<string>) values);
    }

    public static string EncryptFilePath(
      this string path,
      IDataProtector dataProtector,
      string filePathAndFileNameSeparator)
    {
      string str = ((IEnumerable<string>) path.Split('/')).Last<string>();
      return dataProtector.Protect(path) + filePathAndFileNameSeparator + str;
    }

    public static string DecryptFilePath(
      this string path,
      IDataProtector dataProtector,
      string filePathAndFileNameSeparator)
    {
      string protectedData = ((IEnumerable<string>) path.Split(new string[1]
      {
        filePathAndFileNameSeparator
      }, StringSplitOptions.None)).First<string>();
      return dataProtector.Unprotect(protectedData);
    }

    internal static string ReplaceNewlines(this string input, string replacement) => input.Replace("\r\n", replacement).Replace("\n", replacement).Replace("\r", replacement);
  }
}
