using FormBuilder.Core.Helpers;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Extension.Forms.Core.Helpers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

using System.Collections;
using System.Runtime.CompilerServices;
using System.Text;
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

using static FormBuilder.Constants;

namespace FormBuilder.Core.Services
{
    public class PlaceholderParsingService(
      IHttpContextAccessor httpContextAccessor,
      ILogger<PlaceholderParsingService> logger,
      IMemberService memberService,
      AppCaches appCaches,
      IPublishedValueFallback publishedValueFallback,
      IDictionaryHelper dictionaryHelper,
      ParsedPlaceholderFormatterCollection parsedPlaceholderFormatterCollection) : IPlaceholderParsingService
    {
        private const char PlaceholderAndFormatFunctionSeparator = '|';
        private const string? FormatFunctionArgsSeparator = ": ";
        private const string? ContextPlaceholderPattern = "\\[(.+?)\\]";
        private const string? RecordPlaceholderPattern = "\\{(.+?)\\}";
        private const string? MemberPlaceholderPattern = "\\{member\\.(.+?)\\}";
        private const string? ContextPlaceholderPatternForJson = "\\[.[\\w]+.\\]";
        private const string? RecordPlaceholderPatternForJson = "\\{.[\\w\\.]+.\\}";
        private const string? MemberPlaceholderPatternForJson = "\\{member\\.[\\w\\.]+.\\}";
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
        private readonly ILogger<PlaceholderParsingService> _logger = logger;
        private readonly IMemberService _memberService = memberService;
        private readonly AppCaches _appCaches = appCaches;
        private readonly IPublishedValueFallback _publishedValueFallback = publishedValueFallback;
        private readonly IDictionaryHelper _dictionaryHelper = dictionaryHelper;
        private readonly ParsedPlaceholderFormatterCollection _parsedPlaceholderFormatterCollection = parsedPlaceholderFormatterCollection;

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
            string? text = value;
            HttpContext? httpContext = _httpContextAccessor.HttpContext;
            if (httpContext is null)
                return text;
            string? str1 = _dictionaryHelper.GetText(text);
            if (pageElements is not null)
                str1 = ParseContextPlaceholders(httpContext, str1, pageElements, additionalData, htmlEncodeReplacedValues);
            if (httpContext is not null && httpContext.Items.ContainsKey(nameof(pageElements)))
                pageElements = (Hashtable?)httpContext?.Items[nameof(pageElements)];
            if (httpContext is not null)
                str1 = ParseContextPlaceholders(httpContext, str1, pageElements, additionalData, htmlEncodeReplacedValues);
            if (record is not null)
                str1 = ParseRecordPlaceholders(str1, record, htmlEncodeReplacedValues);
            if (record is not null && !string.IsNullOrWhiteSpace(str1) && str1.Contains("{form_name}"))
            {
                string? str2 = form?.Name ?? string.Empty;
                str1 = str1.Replace("{form_name}", htmlEncodeReplacedValues ? HtmlEncode(str2) : str2);
            }
            return record is null || record.MemberKey is null ? httpContext?.User?.Identity is null || !httpContext.User.Identity.IsAuthenticated || !TryGetAuthenticatedMemberKey(httpContext, out Guid key) ? ParseMemberPlaceholders(str1, Guid.Empty.ToString(), htmlEncodeReplacedValues) : ParseMemberPlaceholders(str1, key.ToString(), htmlEncodeReplacedValues) : ParseMemberPlaceholders(str1, record.MemberKey, htmlEncodeReplacedValues);
        }

        private string ParseContextPlaceholders(
          HttpContext context,
          string value,
          Hashtable? pageElements,
          IDictionary<string, string?>? additionalData,
          bool htmlEncodeReplacedValues)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            string? contextPlaceholders = value;
            if (context is null)
                return contextPlaceholders;
            string? regexPattern = GetRegexPattern(value, "\\[(.+?)\\]", "\\[.[\\w]+.\\]");
            MatchCollection placeholderTagMatches = GetPlaceholderTagMatches(value, regexPattern);
            IPublishedContentQuery requiredService = context.RequestServices.GetRequiredService<IPublishedContentQuery>();
            foreach (Match match in placeholderTagMatches)
            {
                string? str1 = match.Value.Trim('[', ']');
                string? str2 = str1[..1];
                string? str3 = str1;
                GetKeyNameAndFormat(str3[1..], out string? keyName, out string? formatToApply);
                string? str4 = match.Value;
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
                                    str4 = additionalData is null || !additionalData.TryGetValue(keyName, out string? str5) || string.IsNullOrEmpty(str5) ? string.Empty : str5;
                                }
                            }
                            else if (pageElements is not null && pageElements[keyName] is not null && pageElements[keyName]!.ToString() != string.Empty)
                                str4 = pageElements[keyName]!.ToString() ?? string.Empty;
                            else if (pageElements is not null)
                            {
                                if (pageElements["splitpath"] is string[] pageElement1)
                                {
                                    for (int index = 0; index < pageElement1.Length - 1; ++index)
                                    {
                                        IPublishedProperty? property = requiredService.Content(pageElement1[pageElement1.Length - index - 1])?.GetProperty(keyName);
                                        if (property is not null && property.Value(_publishedValueFallback) is not null && property.HasValue() && !string.IsNullOrEmpty(property?.Value(_publishedValueFallback)?.ToString()))
                                        {
                                            _logger.LogDebug("Item loaded from {element}", pageElement1[pageElement1.Length - index - 1]);
                                            str4 = property?.Value(_publishedValueFallback)?.ToString() ?? string.Empty;
                                            break;
                                        }
                                    }
                                }
                            }
                            else
                                str4 = string.Empty;
                        }
                        else if (pageElements is not null && pageElements[keyName] is not null)
                        {
                            str4 = pageElements[keyName]!.ToString() ?? string.Empty;
                        }
                        else
                        {
                            str4 = string.Empty;
                            object? pageElement2 = pageElements?["pageID"];
                            if (pageElement2 is not null)
                            {
                                IPublishedContent? publishedContent = requiredService.Content(pageElement2);
                                if (publishedContent is not null)
                                {
                                    IPublishedProperty? property = publishedContent.GetProperty(keyName);
                                    if (property is not null)
                                        str4 = property.Value(_publishedValueFallback)?.ToString() ?? string.Empty;
                                }
                            }
                        }
                    }
                    else
                    {
                        str4 = string.Empty;
                        if (context.Session.GetString(keyName) is not null)
                            str4 = context.Session.GetString(keyName) ?? string.Empty;
                        if (string.IsNullOrWhiteSpace(str4))
                            str4 = context.Request.Cookies[keyName] ?? string.Empty;
                    }
                }
                else
                {
                    StringValues header = context.Request.Query[keyName];
                    string? result = header.ToString();
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
                    str4 = ParseSpecificContextPlaceholders(context, keyName, result);
                }
                if (str4 != match.Value)
                {
                    string? str6 = ApplyFormat(str4, formatToApply);
                    contextPlaceholders = contextPlaceholders.Replace(match.Value, htmlEncodeReplacedValues ? HtmlEncode(str6) : str6);
                }
            }
            return contextPlaceholders;
        }

        private static string HtmlEncode(string value) => HttpUtility.HtmlEncode(value);

        private static string GetRegexPattern(string value, string standardPattern, string jsonPattern)
        {
            string? regexPattern = standardPattern;
            if (IsJson(value))
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
            GetFormatFunctionAndArgs(formatToApply, out string? formatFunction, out string[] formatArgs);
            IParsedPlaceholderFormatter? placeholderFormatter = _parsedPlaceholderFormatterCollection.FirstOrDefault(x => x.FunctionName.InvariantEquals(formatFunction));
            if (placeholderFormatter is not null)
                return placeholderFormatter.FormatValue(value, formatArgs);
            ILogger<PlaceholderParsingService> logger = _logger;
            DefaultInterpolatedStringHandler interpolatedStringHandler = new(130, 2);
            interpolatedStringHandler.AppendLiteral("Attempted to format a parsed placeholder value of ");
            interpolatedStringHandler.AppendFormatted(value);
            interpolatedStringHandler.AppendLiteral(" using a function named ");
            interpolatedStringHandler.AppendFormatted(formatFunction);
            interpolatedStringHandler.AppendLiteral(", but an implementation for this function was not found.");
            string? stringAndClear = interpolatedStringHandler.ToStringAndClear();
            object[] objArray = [];
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
                formatArgs = [.. source.Skip(1).Select(x => x.Trim())];
            }
            else
            {
                formatFunction = formatToApply;
                formatArgs = [];
            }
        }

        private static string ParseSpecificContextPlaceholders(
          HttpContext context,
          string keyName,
          string result)
        {
            result = ParseRemoteAddressPlaceholder(context, keyName, result);
            result = ParseUrlPlaceholder(context, keyName, result);
            result = ParseRefererPlaceholder(context, keyName, result);
            result = ParseUserAgentPlaceholder(context, keyName, result);
            return result;
        }

        private static string ParseRemoteAddressPlaceholder(
          HttpContext context,
          string keyName,
          string result)
        {
            if (ShouldParsePlaceholderKey(keyName, "Remote_Addr", result))
                result = context.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            return result;
        }

        private static string ParseUrlPlaceholder(HttpContext context, string keyName, string result)
        {
            if (ShouldParsePlaceholderKey(keyName, "Url", result))
                result = context.Request.GetDisplayUrl();
            return result;
        }

        private static string ParseRefererPlaceholder(
          HttpContext context,
          string keyName,
          string result)
        {
            if (ShouldParsePlaceholderKey(keyName, "Http_Referer", result))
                result = context.Request.GetTypedHeaders().Referer?.ToString() ?? string.Empty;
            return result;
        }

        private static string ParseUserAgentPlaceholder(
          HttpContext context,
          string keyName,
          string result)
        {
            if (ShouldParsePlaceholderKey(keyName, "Http_User_Agent", result))
                result = context.Request.Headers[Microsoft.Net.Http.Headers.HeaderNames.UserAgent].ToString();
            return result;
        }

        private static bool ShouldParsePlaceholderKey(string keyName, string key, string result) => string.IsNullOrWhiteSpace(result) && string.Equals(keyName, key, StringComparison.OrdinalIgnoreCase);

        private string ParseRecordPlaceholders(
          string value,
          Record record,
          bool htmlEncodeReplacedValues)
        {
            Dictionary<string, string> dictionary1 = [];
            if (record.RecordFields is not null)
            {
                foreach (RecordField recordField in record.RecordFields.Values)
                {
                    if (recordField.Field is not null)
                    {
                        string? recordFieldValue = recordField.ValuesAsString(false);
                        if (DoesFieldHavePlaceholder(value, recordField.Field.Alias))
                        {
                            AddValue(dictionary1, recordField.Field.Alias, recordFieldValue);
                        }
                        else
                        {
                            string? str = XmlHelper.XmlName(recordField.Field.Caption);
                            if (DoesFieldHavePlaceholder(value, str))
                                AddValue(dictionary1, str, recordFieldValue);
                        }
                    }
                }
            }
            int num;
            if (!dictionary1.ContainsKey(XmlHelper.XmlName("record.id")))
            {
                Dictionary<string, string> dictionary2 = dictionary1;
                string? key = XmlHelper.XmlName("record.id");
                num = record.Id;
                string? str = num.ToString();
                dictionary2.Add(key, str);
            }
            dictionary1.Add(XmlHelper.XmlName("record.updated"), record.Updated.ToString());
            dictionary1.Add(XmlHelper.XmlName("record.created"), record.Created.ToString());
            Dictionary<string, string> dictionary3 = dictionary1;
            string? key1 = XmlHelper.XmlName("record.umbracopageid");
            num = record.UmbracoPageId;
            string? str1 = num.ToString();
            dictionary3.Add(key1, str1);
            if (!dictionary1.ContainsKey(XmlHelper.XmlName("record.uniqueid")))
                dictionary1.Add(XmlHelper.XmlName("record.uniqueid"), record.UniqueId.ToString());
            if (!string.IsNullOrEmpty(record.IP))
                dictionary1.Add(XmlHelper.XmlName("record.ip"), record.IP);
            if (!string.IsNullOrEmpty(record.MemberKey))
                dictionary1.Add(XmlHelper.XmlName("record.memberkey"), record.MemberKey);
            string? recordPlaceholders = ParseRecordPlaceholders(value, dictionary1, htmlEncodeReplacedValues);
            return string.IsNullOrWhiteSpace(recordPlaceholders) ? value : recordPlaceholders;
        }

        private static bool DoesFieldHavePlaceholder(string value, string alias) => value.IndexOf("{" + alias + "}", StringComparison.InvariantCultureIgnoreCase) > -1 || value.IndexOf("{" + alias + " |", StringComparison.InvariantCultureIgnoreCase) > -1;

        private static void AddValue(
          Dictionary<string, string> values,
          string key,
          string recordFieldValue)
        {
            if (values.TryGetValue(key, out string? value) && value == recordFieldValue)
                return;
            if (values.TryGetValue(key, out string? value1))
                values[key] = value1 + ", " + recordFieldValue;
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
            string? recordPlaceholders = string.Empty;
            string? regexPattern = GetRegexPattern(value, "\\{(.+?)\\}", "\\{.[\\w\\.]+.\\}");
            foreach (Match placeholderTagMatch in GetPlaceholderTagMatches(value, regexPattern))
            {
                GetKeyNameAndFormat(placeholderTagMatch.Value.Trim('{', '}'), out string? keyName, out string? formatToApply);
                keyName = XmlHelper.XmlName(keyName);
                if (recordFields.Any(i => string.Equals(i.Key, keyName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    string? str = ApplyFormat(recordFields.First(i => string.Equals(i.Key, keyName, StringComparison.InvariantCultureIgnoreCase)).Value, formatToApply);
                    value = value.Replace(placeholderTagMatch.Value, htmlEncodeReplacedValues ? HtmlEncode(str) : str);
                    recordPlaceholders = value;
                }
            }
            return recordPlaceholders;
        }

        private static bool TryGetAuthenticatedMemberKey(HttpContext context, out Guid key)
        {
            key = Guid.Empty;
            MemberIdentityUser? result = context.RequestServices.GetRequiredService<IMemberManager>().GetCurrentMemberAsync().GetAwaiter().GetResult();
            if (result is null)
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
            if (Guid.TryParse(memberKey.ToString(), out Guid memberKeyAsGuid))
            {
                string? regexPattern = GetRegexPattern(value, "\\{member\\.(.+?)\\}", "\\{member\\.[\\w\\.]+.\\}");
                MatchCollection placeholderTagMatches = GetPlaceholderTagMatches(value, regexPattern);
                if (placeholderTagMatches.Count > 0)
                {
                    if (memberKeyAsGuid == Guid.Empty)
                    {
                        string? memberPlaceholders = value;
                        foreach (Match match in placeholderTagMatches)
                            memberPlaceholders = memberPlaceholders.Replace(match.Value, string.Empty);
                        return memberPlaceholders;
                    }
                    IMember? member = _appCaches.RuntimeCache.GetCacheItem(FormBuilderCacheKeys.GetMemberCacheKey(memberKeyAsGuid), new Func<IMember>(GetMember!), new TimeSpan?(TimeSpan.FromHours(1)));
                    if (member is not null)
                    {
                        Dictionary<string, string> recordFields = _appCaches.RuntimeCache.GetCacheItem(FormBuilderCacheKeys.GetMemberValuesCacheKey(memberKeyAsGuid), new Func<Dictionary<string, string>>(GetMemberValues), new TimeSpan?(TimeSpan.FromHours(1))) ?? [];
                        return ParseMemberPlaceholders(value, recordFields, htmlEncodeReplacedValues);
                    }

                    Dictionary<string, string> GetMemberValues()
                    {
                        Dictionary<string, string> memberValues = new()
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
                        foreach (Property property in member.Properties.Cast<Property>())
                        {
                            string? str = string.Empty;
                            if (property.GetValue(null, null, false) is not null)
                                str = property.GetValue(null, null, false)?.ToString() ?? string.Empty;
                            memberValues.Add(XmlHelper.XmlName("member." + property.Alias), str);
                        }
                        return memberValues;
                    }
                }
            }
            _logger.LogDebug("Parsing Member placeholders stop");
            return value;

            IMember? GetMember() => _memberService.GetByKey(memberKeyAsGuid);
        }

        private string ParseMemberPlaceholders(
          string value,
          Dictionary<string, string> recordFields,
          bool htmlEncodeReplacedValues)
        {
            if (string.IsNullOrEmpty(value))
                return value;
            string? memberPlaceholders = value;
            string? regexPattern = GetRegexPattern(value, "\\{member\\.(.+?)\\}", "\\{member\\.[\\w\\.]+.\\}");
            foreach (Match placeholderTagMatch in GetPlaceholderTagMatches(value, regexPattern))
            {
                GetKeyNameAndFormat(placeholderTagMatch.Value.Trim('{', '}'), out string? keyName, out string? formatToApply);
                keyName = XmlHelper.XmlName(keyName);
                string? str = recordFields.TryGetValue(keyName, out string? value2) ? ApplyFormat(value2, formatToApply) : string.Empty;
                memberPlaceholders = memberPlaceholders.Replace(placeholderTagMatch.Value, htmlEncodeReplacedValues ? HtmlEncode(str) : str);
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