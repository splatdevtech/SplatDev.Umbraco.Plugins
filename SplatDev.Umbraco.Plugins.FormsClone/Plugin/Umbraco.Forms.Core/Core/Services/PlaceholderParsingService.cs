
// Type: Umbraco.Forms.Core.Services.PlaceholderParsingService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using System.Web;
using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Cache;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.PublishedContent;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Data.Helpers;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  public class PlaceholderParsingService : IPlaceholderParsingService
  {
    private const char PlaceholderAndFormatFunctionSeparator = '|';
    private const string FormatFunctionArgsSeparator = ": ";
    private const string ContextPlaceholderPattern = "\\[(.+?)\\]";
    private const string RecordPlaceholderPattern = "\\{(.+?)\\}";
    private const string MemberPlaceholderPattern = "\\{member\\.(.+?)\\}";
    private const string ContextPlaceholderPatternForJson = "\\[.[\\w]+.\\]";
    private const string RecordPlaceholderPatternForJson = "\\{.[\\w\\.]+.\\}";
    private const string MemberPlaceholderPatternForJson = "\\{member\\.[\\w\\.]+.\\}";
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<PlaceholderParsingService> _logger;
    private readonly IMemberService _memberService;
    private readonly AppCaches _appCaches;
    private readonly IPublishedValueFallback _publishedValueFallback;
    private readonly IDictionaryHelper _dictionaryHelper;
    private readonly ParsedPlaceholderFormatterCollection _parsedPlaceholderFormatterCollection;

    public PlaceholderParsingService(
      IHttpContextAccessor httpContextAccessor,
      ILogger<PlaceholderParsingService> logger,
      IMemberService memberService,
      AppCaches appCaches,
      IPublishedValueFallback publishedValueFallback,
      IDictionaryHelper dictionaryHelper,
      ParsedPlaceholderFormatterCollection parsedPlaceholderFormatterCollection)
    {
      this._httpContextAccessor = httpContextAccessor;
      this._logger = logger;
      this._memberService = memberService;
      this._appCaches = appCaches;
      this._publishedValueFallback = publishedValueFallback;
      this._dictionaryHelper = dictionaryHelper;
      this._parsedPlaceholderFormatterCollection = parsedPlaceholderFormatterCollection;
    }

    public string ParsePlaceHolders(
      string value,
      bool htmlEncodeReplacedValues,
      Record? record = null,
      Form? form = null,
      Hashtable? pageElements = null,
      IDictionary<string, string?>? additionalData = null)
    {
      if (string.IsNullOrWhiteSpace(value))
        return value;
      string text = value;
      Microsoft.AspNetCore.Http.HttpContext httpContext = this._httpContextAccessor.HttpContext;
      if (httpContext == null)
        return text;
      string str1 = this._dictionaryHelper.GetText(text);
      if (pageElements != null)
        str1 = this.ParseContextPlaceholders(httpContext, str1, pageElements, additionalData, htmlEncodeReplacedValues);
      if (httpContext != null && httpContext.Items.ContainsKey((object) nameof (pageElements)))
        pageElements = (Hashtable) httpContext.Items[(object) nameof (pageElements)];
      if (httpContext != null)
        str1 = this.ParseContextPlaceholders(httpContext, str1, pageElements, additionalData, htmlEncodeReplacedValues);
      if (record != null)
        str1 = this.ParseRecordPlaceholders(str1, record, htmlEncodeReplacedValues);
      if (record != null && !string.IsNullOrWhiteSpace(str1) && str1.Contains("{form_name}"))
      {
        string str2 = form?.Name ?? string.Empty;
        str1 = str1.Replace("{form_name}", htmlEncodeReplacedValues ? PlaceholderParsingService.HtmlEncode(str2) : str2);
      }
      Guid key;
      return record == null || record.MemberKey == null ? (httpContext?.User?.Identity == null || !httpContext.User.Identity.IsAuthenticated || !PlaceholderParsingService.TryGetAuthenticatedMemberKey(httpContext, out key) ? this.ParseMemberPlaceholders(str1, Guid.Empty.ToString(), htmlEncodeReplacedValues) : this.ParseMemberPlaceholders(str1, key.ToString(), htmlEncodeReplacedValues)) : this.ParseMemberPlaceholders(str1, record.MemberKey, htmlEncodeReplacedValues);
    }

    private string ParseContextPlaceholders(
      Microsoft.AspNetCore.Http.HttpContext context,
      string value,
      Hashtable? pageElements,
      IDictionary<string, string?>? additionalData,
      bool htmlEncodeReplacedValues)
    {
      if (string.IsNullOrEmpty(value))
        return value;
      string contextPlaceholders = value;
      if (context == null)
        return contextPlaceholders;
      string regexPattern = PlaceholderParsingService.GetRegexPattern(value, "\\[(.+?)\\]", "\\[.[\\w]+.\\]");
      MatchCollection placeholderTagMatches = PlaceholderParsingService.GetPlaceholderTagMatches(value, regexPattern);
      IPublishedContentQuery requiredService = ServiceProviderServiceExtensions.GetRequiredService<IPublishedContentQuery>(context.RequestServices);
      foreach (Match match in placeholderTagMatches)
      {
        string str1 = match.Value.Trim('[', ']');
        string str2 = str1.Substring(0, 1);
        string str3 = str1;
        string keyName;
        string formatToApply;
        PlaceholderParsingService.GetKeyNameAndFormat(str3.Substring(1, str3.Length - 1), out keyName, out formatToApply);
        string str4 = match.Value;
        if (!(str2 == "@"))
        {
          if (!(str2 == "%"))
          {
            if (!(str2 == "#"))
            {
              if (!(str2 == "$"))
              {
                if (str2 == "+")
                {
                  string str5;
                  str4 = additionalData == null || !additionalData.TryGetValue(keyName, out str5) || string.IsNullOrEmpty(str5) ? string.Empty : str5;
                }
              }
              else if (pageElements != null && pageElements[(object) keyName] != null && pageElements[(object) keyName].ToString() != string.Empty)
                str4 = pageElements[(object) keyName].ToString() ?? string.Empty;
              else if (pageElements != null)
              {
                if (pageElements[(object) "splitpath"] is string[] pageElement1)
                {
                  for (int index = 0; index < pageElement1.Length - 1; ++index)
                  {
                    IPublishedProperty property = requiredService.Content((object) pageElement1[pageElement1.Length - index - 1])?.GetProperty(keyName);
                    if (property != null && property.Value(this._publishedValueFallback) != null && property.HasValue() && !string.IsNullOrEmpty(property != null ? property.Value(this._publishedValueFallback)?.ToString() : (string) null))
                    {
                      this._logger.LogDebug("Item loaded from " + pageElement1[pageElement1.Length - index - 1]);
                      str4 = property.Value(this._publishedValueFallback)?.ToString() ?? string.Empty;
                      break;
                    }
                  }
                }
              }
              else
                str4 = string.Empty;
            }
            else if (pageElements != null && pageElements[(object) keyName] != null)
            {
              str4 = pageElements[(object) keyName].ToString() ?? string.Empty;
            }
            else
            {
              str4 = string.Empty;
              object pageElement2 = pageElements?[(object) "pageID"];
              if (pageElement2 != null)
              {
                IPublishedContent publishedContent = requiredService.Content(pageElement2);
                if (publishedContent != null)
                {
                  IPublishedProperty property = publishedContent.GetProperty(keyName);
                  if (property != null)
                    str4 = property.Value(this._publishedValueFallback)?.ToString() ?? string.Empty;
                }
              }
            }
          }
          else
          {
            str4 = string.Empty;
            if (context.Session.GetString(keyName) != null)
              str4 = context.Session.GetString(keyName) ?? string.Empty;
            if (string.IsNullOrWhiteSpace(str4))
              str4 = context.Request.Cookies[keyName] ?? string.Empty;
          }
        }
        else
        {
          StringValues header = context.Request.Query[keyName];
          string result = header.ToString();
          if (string.IsNullOrWhiteSpace(result) && context.Request.HasFormContentType)
          {
            header = context.Request.Form[keyName];
            result = header.ToString();
          }
          if (string.IsNullOrWhiteSpace(result))
          {
            header = context.Request.Headers[keyName];
            result = header.ToString();
          }
          str4 = PlaceholderParsingService.ParseSpecificContextPlaceholders(context, keyName, result);
        }
        if (str4 != match.Value)
        {
          string str6 = this.ApplyFormat(str4, formatToApply);
          contextPlaceholders = contextPlaceholders.Replace(match.Value, htmlEncodeReplacedValues ? PlaceholderParsingService.HtmlEncode(str6) : str6);
        }
      }
      return contextPlaceholders;
    }

    private static string HtmlEncode(string value) => HttpUtility.HtmlEncode(value);

    private static string GetRegexPattern(string value, string standardPattern, string jsonPattern)
    {
      string regexPattern = standardPattern;
      if (PlaceholderParsingService.IsJson(value))
        regexPattern = jsonPattern;
      return regexPattern;
    }

    private static MatchCollection GetPlaceholderTagMatches(
      string value,
      string pattern)
    {
      return new Regex(pattern).Matches(value);
    }

    private static void GetKeyNameAndFormat(
      string value,
      out string keyName,
      out string formatToApply)
    {
      if (value.Contains('|'))
      {
        string[] strArray = value.Split('|');
        keyName = strArray[0].Trim();
        formatToApply = strArray[1].Trim();
      }
      else
      {
        keyName = value;
        formatToApply = string.Empty;
      }
    }

    private string ApplyFormat(string value, string formatToApply)
    {
      if (string.IsNullOrWhiteSpace(formatToApply))
        return value;
      string[] formatArgs;
      string formatFunction;
      PlaceholderParsingService.GetFormatFunctionAndArgs(formatToApply, out formatFunction, out formatArgs);
      IParsedPlaceholderFormatter placeholderFormatter = this._parsedPlaceholderFormatterCollection.FirstOrDefault<IParsedPlaceholderFormatter>((Func<IParsedPlaceholderFormatter, bool>) (x => x.FunctionName.InvariantEquals(formatFunction)));
      if (placeholderFormatter != null)
        return placeholderFormatter.FormatValue(value, formatArgs);
      ILogger<PlaceholderParsingService> logger = this._logger;
      DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(130, 2);
      interpolatedStringHandler.AppendLiteral("Attempted to format a parsed placeholder value of ");
      interpolatedStringHandler.AppendFormatted(value);
      interpolatedStringHandler.AppendLiteral(" using a function named ");
      interpolatedStringHandler.AppendFormatted(formatFunction);
      interpolatedStringHandler.AppendLiteral(", but an implementation for this function was not found.");
      string stringAndClear = interpolatedStringHandler.ToStringAndClear();
      object[] objArray = Array.Empty<object>();
      logger.LogDebug(stringAndClear, objArray);
      return value;
    }

    private static void GetFormatFunctionAndArgs(
      string formatToApply,
      out string formatFunction,
      out string[] formatArgs)
    {
      if (formatToApply.Contains(": "))
      {
        string[] source = formatToApply.Split(": ");
        formatFunction = source[0].Trim();
        formatArgs = ((IEnumerable<string>) source).Skip<string>(1).Select<string, string>((Func<string, string>) (x => x.Trim())).ToArray<string>();
      }
      else
      {
        formatFunction = formatToApply;
        formatArgs = Array.Empty<string>();
      }
    }

    private static string ParseSpecificContextPlaceholders(
      Microsoft.AspNetCore.Http.HttpContext context,
      string keyName,
      string result)
    {
      result = PlaceholderParsingService.ParseRemoteAddressPlaceholder(context, keyName, result);
      result = PlaceholderParsingService.ParseUrlPlaceholder(context, keyName, result);
      result = PlaceholderParsingService.ParseRefererPlaceholder(context, keyName, result);
      result = PlaceholderParsingService.ParseUserAgentPlaceholder(context, keyName, result);
      return result;
    }

    private static string ParseRemoteAddressPlaceholder(
      Microsoft.AspNetCore.Http.HttpContext context,
      string keyName,
      string result)
    {
      if (PlaceholderParsingService.ShouldParsePlaceholderKey(keyName, "Remote_Addr", result))
        result = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
      return result;
    }

    private static string ParseUrlPlaceholder(Microsoft.AspNetCore.Http.HttpContext context, string keyName, string result)
    {
      if (PlaceholderParsingService.ShouldParsePlaceholderKey(keyName, "Url", result))
        result = context.Request.GetDisplayUrl();
      return result;
    }

    private static string ParseRefererPlaceholder(
      Microsoft.AspNetCore.Http.HttpContext context,
      string keyName,
      string result)
    {
      if (PlaceholderParsingService.ShouldParsePlaceholderKey(keyName, "Http_Referer", result))
        result = context.Request.GetTypedHeaders().Referer?.ToString() ?? string.Empty;
      return result;
    }

    private static string ParseUserAgentPlaceholder(
      Microsoft.AspNetCore.Http.HttpContext context,
      string keyName,
      string result)
    {
      if (PlaceholderParsingService.ShouldParsePlaceholderKey(keyName, "Http_User_Agent", result))
        result = context.Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.UserAgent].ToString();
      return result;
    }

    private static bool ShouldParsePlaceholderKey(string keyName, string key, string result) => string.IsNullOrWhiteSpace(result) && string.Equals(keyName, key, StringComparison.OrdinalIgnoreCase);

    private string ParseRecordPlaceholders(
      string value,
      Record record,
      bool htmlEncodeReplacedValues)
    {
      Dictionary<string, string> dictionary1 = new Dictionary<string, string>();
      if (record.RecordFields != null)
      {
        foreach (RecordField recordField in record.RecordFields.Values)
        {
          if (recordField.Field != null)
          {
            string recordFieldValue = recordField.ValuesAsString(false);
            if (PlaceholderParsingService.DoesFieldHavePlaceholder(value, recordField.Field.Alias))
            {
              PlaceholderParsingService.AddValue(dictionary1, recordField.Field.Alias, recordFieldValue);
            }
            else
            {
              string str = XmlHelper.XmlName(recordField.Field.Caption);
              if (PlaceholderParsingService.DoesFieldHavePlaceholder(value, str))
                PlaceholderParsingService.AddValue(dictionary1, str, recordFieldValue);
            }
          }
        }
      }
      int num;
      if (!dictionary1.ContainsKey(XmlHelper.XmlName("record.id")))
      {
        Dictionary<string, string> dictionary2 = dictionary1;
        string key = XmlHelper.XmlName("record.id");
        num = record.Id;
        string str = num.ToString();
        dictionary2.Add(key, str);
      }
      dictionary1.Add(XmlHelper.XmlName("record.updated"), record.Updated.ToString());
      dictionary1.Add(XmlHelper.XmlName("record.created"), record.Created.ToString());
      Dictionary<string, string> dictionary3 = dictionary1;
      string key1 = XmlHelper.XmlName("record.umbracopageid");
      num = record.UmbracoPageId;
      string str1 = num.ToString();
      dictionary3.Add(key1, str1);
      if (!dictionary1.ContainsKey(XmlHelper.XmlName("record.uniqueid")))
        dictionary1.Add(XmlHelper.XmlName("record.uniqueid"), record.UniqueId.ToString());
      if (!string.IsNullOrEmpty(record.IP))
        dictionary1.Add(XmlHelper.XmlName("record.ip"), record.IP);
      if (!string.IsNullOrEmpty(record.MemberKey))
        dictionary1.Add(XmlHelper.XmlName("record.memberkey"), record.MemberKey);
      string recordPlaceholders = this.ParseRecordPlaceholders(value, dictionary1, htmlEncodeReplacedValues);
      return string.IsNullOrWhiteSpace(recordPlaceholders) ? value : recordPlaceholders;
    }

    private static bool DoesFieldHavePlaceholder(string value, string alias) => value.IndexOf("{" + alias + "}", StringComparison.InvariantCultureIgnoreCase) > -1 || value.IndexOf("{" + alias + " |", StringComparison.InvariantCultureIgnoreCase) > -1;

    private static void AddValue(
      Dictionary<string, string> values,
      string key,
      string recordFieldValue)
    {
      if (values.ContainsKey(key) && values[key] == recordFieldValue)
        return;
      if (values.ContainsKey(key))
        values[key] = values[key] + ", " + recordFieldValue;
      else
        values.Add(key, recordFieldValue);
    }

    private string ParseRecordPlaceholders(
      string value,
      Dictionary<string, string> recordFields,
      bool htmlEncodeReplacedValues)
    {
      if (string.IsNullOrEmpty(value))
        return string.Empty;
      string recordPlaceholders = string.Empty;
      string regexPattern = PlaceholderParsingService.GetRegexPattern(value, "\\{(.+?)\\}", "\\{.[\\w\\.]+.\\}");
      foreach (Match placeholderTagMatch in PlaceholderParsingService.GetPlaceholderTagMatches(value, regexPattern))
      {
        string formatToApply;
        string keyName;
        PlaceholderParsingService.GetKeyNameAndFormat(placeholderTagMatch.Value.Trim('{', '}'), out keyName, out formatToApply);
        keyName = XmlHelper.XmlName(keyName);
        if (recordFields.Any<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (i => string.Equals(i.Key, keyName, StringComparison.InvariantCultureIgnoreCase))))
        {
          string str = this.ApplyFormat(recordFields.First<KeyValuePair<string, string>>((Func<KeyValuePair<string, string>, bool>) (i => string.Equals(i.Key, keyName, StringComparison.InvariantCultureIgnoreCase))).Value, formatToApply);
          value = value.Replace(placeholderTagMatch.Value, htmlEncodeReplacedValues ? PlaceholderParsingService.HtmlEncode(str) : str);
          recordPlaceholders = value;
        }
      }
      return recordPlaceholders;
    }

    private static bool TryGetAuthenticatedMemberKey(Microsoft.AspNetCore.Http.HttpContext context, out Guid key)
    {
      key = Guid.Empty;
      MemberIdentityUser result = ServiceProviderServiceExtensions.GetRequiredService<IMemberManager>(context.RequestServices).GetCurrentMemberAsync().GetAwaiter().GetResult();
      if (result == null)
        return false;
      key = result.Key;
      return true;
    }

    private string ParseMemberPlaceholders(
      string value,
      string? memberKey,
      bool htmlEncodeReplacedValues)
    {
      if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(memberKey))
        return value;
      Guid memberKeyAsGuid;
      if (Guid.TryParse(memberKey.ToString(), out memberKeyAsGuid))
      {
        string regexPattern = PlaceholderParsingService.GetRegexPattern(value, "\\{member\\.(.+?)\\}", "\\{member\\.[\\w\\.]+.\\}");
        MatchCollection placeholderTagMatches = PlaceholderParsingService.GetPlaceholderTagMatches(value, regexPattern);
        if (placeholderTagMatches.Count > 0)
        {
          if (memberKeyAsGuid == Guid.Empty)
          {
            string memberPlaceholders = value;
            foreach (Match match in placeholderTagMatches)
              memberPlaceholders = memberPlaceholders.Replace(match.Value, string.Empty);
            return memberPlaceholders;
          }
          IMember member = this._appCaches.RuntimeCache.GetCacheItem<IMember>(Umbraco.Forms.Core.Cache.CacheKeys.GetMemberCacheKey(memberKeyAsGuid), new Func<IMember>(GetMember), new TimeSpan?(TimeSpan.FromHours(1)));
          if (member != null)
          {
            Dictionary<string, string> recordFields = this._appCaches.RuntimeCache.GetCacheItem<Dictionary<string, string>>(Umbraco.Forms.Core.Cache.CacheKeys.GetMemberValuesCacheKey(memberKeyAsGuid), new Func<Dictionary<string, string>>(GetMemberValues), new TimeSpan?(TimeSpan.FromHours(1))) ?? new Dictionary<string, string>();
            return this.ParseMemberPlaceholders(value, recordFields, htmlEncodeReplacedValues);
          }

          Dictionary<string, string> GetMemberValues()
          {
            Dictionary<string, string> memberValues = new Dictionary<string, string>()
            {
              {
                XmlHelper.XmlName("member.name"),
                member.Name ?? string.Empty
              },
              {
                XmlHelper.XmlName("member.id"),
                member.Id.ToString()
              },
              {
                XmlHelper.XmlName("member.loginname"),
                member.Username
              },
              {
                XmlHelper.XmlName("member.email"),
                member.Email
              }
            };
            foreach (Property property in (IEnumerable<IProperty>) member.Properties)
            {
              string str = string.Empty;
              if (property.GetValue((string) null, (string) null, false) != null)
                str = property.GetValue((string) null, (string) null, false)?.ToString() ?? string.Empty;
              memberValues.Add(XmlHelper.XmlName("member." + property.Alias), str);
            }
            return memberValues;
          }
        }
      }
      this._logger.LogDebug("Parsing Member placeholders stop");
      return value;

      IMember? GetMember() => this._memberService.GetByKey(memberKeyAsGuid);
    }

    private string ParseMemberPlaceholders(
      string value,
      Dictionary<string, string> recordFields,
      bool htmlEncodeReplacedValues)
    {
      if (string.IsNullOrEmpty(value))
        return value;
      string memberPlaceholders = value;
      string regexPattern = PlaceholderParsingService.GetRegexPattern(value, "\\{member\\.(.+?)\\}", "\\{member\\.[\\w\\.]+.\\}");
      foreach (Match placeholderTagMatch in PlaceholderParsingService.GetPlaceholderTagMatches(value, regexPattern))
      {
        string keyName;
        string formatToApply;
        PlaceholderParsingService.GetKeyNameAndFormat(placeholderTagMatch.Value.Trim('{', '}'), out keyName, out formatToApply);
        keyName = XmlHelper.XmlName(keyName);
        string str = recordFields.ContainsKey(keyName) ? this.ApplyFormat(recordFields[keyName], formatToApply) : string.Empty;
        memberPlaceholders = memberPlaceholders.Replace(placeholderTagMatch.Value, htmlEncodeReplacedValues ? PlaceholderParsingService.HtmlEncode(str) : str);
      }
      return memberPlaceholders;
    }

    private static bool IsJson(string value)
    {
      if (!value.DetectIsJson())
        return false;
      try
      {
        JsonNode.Parse(value);
        return true;
      }
      catch
      {
        return false;
      }
    }
  }
}
