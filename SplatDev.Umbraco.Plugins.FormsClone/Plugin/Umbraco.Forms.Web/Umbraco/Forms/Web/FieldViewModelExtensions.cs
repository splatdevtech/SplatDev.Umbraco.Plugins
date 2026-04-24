
// Type: Umbraco.Forms.Web.FieldViewModelExtensions
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Umbraco.Cms.Core;
using Umbraco.Extensions;
using Umbraco.Forms.Core.Data;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Data.FileSystem;
using Umbraco.Forms.Web.Models;


#nullable enable
namespace Umbraco.Forms.Web
{
    public static class FieldViewModelExtensions
    {
        public static T? GetSettingValue<T>(
          this FieldViewModel fieldViewModel,
          string settingAlias,
          T? defaultValue = default)
        {
            if (!fieldViewModel.AdditionalSettings.Any<KeyValuePair<string, string>>() || !fieldViewModel.AdditionalSettings.ContainsKey(settingAlias) || string.IsNullOrEmpty(fieldViewModel.AdditionalSettings[settingAlias]))
                return defaultValue;
            Attempt<object> attempt = fieldViewModel.AdditionalSettings[settingAlias].TryConvertTo(typeof(T));
            return !attempt.Success ? defaultValue : (T)attempt.Result;
        }

        public static string GetFileUploadAsString(
          this FieldViewModel fieldViewModel,
          FormsFileSystemForSavedData fileSystem,
          string settingAlias,
          string defaultValue = "")
        {
            string settingValue = fieldViewModel.GetSettingValue<string>(settingAlias);
            if (string.IsNullOrEmpty(settingValue) || !fileSystem.FileExists(settingValue))
                return defaultValue;
            using (Stream stream = fileSystem.OpenFile(settingValue))
            {
                using (StreamReader streamReader = new StreamReader(stream))
                    return streamReader.ReadToEnd();
            }
        }

        public static IEnumerable<PreValue> GetFileUploadAsPreValues(
          this FieldViewModel fieldViewModel,
          string settingAlias,
          IPreValueTextFileStorage preValueTextFileStorage,
          IEnumerable<PreValue> defaultValue)
        {
            string settingValue = fieldViewModel.GetSettingValue<string>(settingAlias);
            return string.IsNullOrEmpty(settingValue) ? defaultValue : preValueTextFileStorage.GetTextFilePreValues(settingValue);
        }
    }
}
