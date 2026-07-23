using FormBuilder.Core.FileSystem;
using FormBuilder.Core.Models;
using FormBuilder.Core.Storage.Interfaces;

using Umbraco.Cms.Core;
using Umbraco.Extensions;

namespace FormBuilder.Web.Extensions
{
    /// <summary>
    /// Provides extension methods on     /// </summary>
    public static class FieldViewModelExtensions
    {
        /// <summary>
        /// Fetches a setting by its alias, and tries to cast the setting value to a specific type.
        /// </summary>
        /// <typeparam name="T">Type of setting value</typeparam>
        /// <param name="fieldViewModel">The         /// <param name="settingAlias">Alias of the setting to retrieve.</param>
        /// <param name="defaultValue">Value returned in case no value is provided.</param>
        public static T? GetSettingValue<T>(
          this FieldViewModel fieldViewModel,
          string settingAlias,
          T? defaultValue = default)
        {
            if (!fieldViewModel.AdditionalSettings.Any() || !fieldViewModel.AdditionalSettings.TryGetValue(settingAlias, out string? value) || string.IsNullOrEmpty(value))
                return defaultValue;
            Attempt<object?> attempt = value.TryConvertTo(typeof(T));
            return !attempt.Success ? defaultValue : (T?)attempt.Result;
        }

        /// <summary>
        /// Fetches a file stored on a field settings collection as a string.
        /// </summary>
        /// <param name="fieldViewModel">The         /// <param name="fileSystem">The file system.</param>
        /// <param name="settingAlias">Alias of the setting to retrieve.</param>
        /// <param name="defaultValue">Value returned in case no value is provided.</param>
        public static string GetFileUploadAsString(
          this FieldViewModel fieldViewModel,
          FormsFileSystemForSavedData fileSystem,
          string settingAlias,
          string defaultValue = "")
        {
            string? settingValue = fieldViewModel.GetSettingValue<string>(settingAlias);
            if (string.IsNullOrEmpty(settingValue) || !fileSystem.FileExists(settingValue))
                return defaultValue;
            using Stream stream = fileSystem.OpenFile(settingValue);
            using StreamReader streamReader = new(stream);
            return streamReader.ReadToEnd();
        }

        /// <summary>
        /// Helper to retrieve a stored txt file as a list of prevalues, splitting the file per-line into seperate prevalues.
        /// </summary>
        /// <param name="fieldViewModel">The         /// <param name="settingAlias">Alias of the setting to retrieve.</param>
        /// <param name="preValueTextFileStorage">the pre value text file storage.</param>
        /// <param name="defaultValue">Value returned in case no value is provided.</param>
        public static IEnumerable<Prevalue> GetFileUploadAsPreValues(
          this FieldViewModel fieldViewModel,
          string settingAlias,
          IPreValueTextFileStorage preValueTextFileStorage,
          IEnumerable<Prevalue> defaultValue)
        {
            string? settingValue = fieldViewModel.GetSettingValue<string>(settingAlias);
            return string.IsNullOrEmpty(settingValue) ? defaultValue : preValueTextFileStorage.GetTextFilePreValues(settingValue);
        }
    }
}