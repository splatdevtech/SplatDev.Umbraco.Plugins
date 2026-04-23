using FormBuilder.Core.Attributes;
using FormBuilder.Core.Prevalues;
using FormBuilder.Core.Providers;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Core.Storage.Interfaces;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Api.Common.Builders;
using Umbraco.Cms.Core.Models.TemporaryFile;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.OperationStatus;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller providing common services for creating or updating prevalue sources.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [Authorize(Policy = "ManagePrevalueSources")]
    public abstract class CreateOrUpdatePrevalueSourceControllerBase(
      IPrevalueSourceService prevalueSourceService,
      FieldPrevalueSourceCollection fieldPreValueSources,
      IFieldPrevalueSourceTypeService fieldPreValueSourceTypeService,
      ITemporaryFileService temporaryFileService,
      IShortStringHelper shortStringHelper,
      IFileStreamSecurityValidator fileStreamSecurityValidator,
      IPreValueTextFileStorage preValueTextFileStorage) : PrevalueSourceControllerBase(fieldPreValueSources, prevalueSourceService, fieldPreValueSourceTypeService)
    {
        private readonly ITemporaryFileService _temporaryFileService = temporaryFileService;
        private readonly IShortStringHelper _shortStringHelper = shortStringHelper;
        private readonly IFileStreamSecurityValidator _fileStreamSecurityValidator = fileStreamSecurityValidator;
        private readonly IPreValueTextFileStorage _preValueTextFileStorage = preValueTextFileStorage;

        /// <summary>Validates a prevalue source.</summary>
        protected ProviderValidationResult TryValidateProviderType(
          FieldPrevalueSource preValueSource,
          out ProblemDetails? problemDetails)
        {
            problemDetails = null;
            FieldPrevalueSourceType? byId = FieldPrevalueSourceTypeService.GetById(preValueSource.FieldPrevalueSourceTypeId);
            if (byId is null)
                return ProviderValidationResult.FailedTypeNotFound;
            byId.LoadSettings(preValueSource);
            List<Exception> exceptionList = byId.ValidateSettings();
            if (exceptionList.Count == 0)
                return ProviderValidationResult.Success;
            problemDetails = BuildSettingsValidationProblemDetails(exceptionList);
            return ProviderValidationResult.FailedValidation;
        }

        /// <summary>
        /// Some setting values require transformation before saving, which is handled here.
        /// </summary>
        protected async Task<TransformSettingsResult> TransformSettings(
          FieldPrevalueSource preValueSource)
        {
            CreateOrUpdatePrevalueSourceControllerBase sourceControllerBase = this;
            FieldPrevalueSourceType? byId = sourceControllerBase.FieldPrevalueSourceTypeService.GetById(preValueSource.FieldPrevalueSourceTypeId);
            if (byId is null)
            {
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(48, 1);
                interpolatedStringHandler.AppendLiteral("Could not find prevalue source type with id of: ");
                interpolatedStringHandler.AppendFormatted(preValueSource.FieldPrevalueSourceTypeId);
                throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
            }
            Dictionary<string, SettingAttribute> typeSettings = byId.Settings();
            Dictionary<string, string> providedSettings = preValueSource.Settings;
            foreach (KeyValuePair<string, string> keyValuePair in providedSettings)
            {
                KeyValuePair<string, string> providedSetting = keyValuePair;
                typeSettings.TryGetValue(providedSetting.Key, out SettingAttribute? settingAttribute);
                if (settingAttribute is not null)
                {
                    if (settingAttribute.View == "Umb.PropertyEditorUi.UploadField" && Guid.TryParse(providedSetting.Value, out Guid temporaryFileKey))
                    {
                        TemporaryFileModel temporaryFileModel = await sourceControllerBase._temporaryFileService.GetAsync(temporaryFileKey).ConfigureAwait(false) ?? throw new InvalidOperationException("Could not get temporary uploaded file for setting.");
                        string? safeFileName = temporaryFileModel.FileName.ToSafeFileName(sourceControllerBase._shortStringHelper);
                        string str = safeFileName;
                        int startIndex = safeFileName.LastIndexOf('.') + 1;
                        if (!str[startIndex..].Equals("txt", StringComparison.CurrentCultureIgnoreCase))
                            return TransformSettingsResult.WithFailure("Invalid file extension", "Only .txt files can be uploaded.");
                        Guid fileId = Guid.NewGuid();
                        using (Stream stream = temporaryFileModel.OpenReadStream())
                        {
                            if (!sourceControllerBase._fileStreamSecurityValidator.IsConsideredSafe(stream))
                                return TransformSettingsResult.WithFailure("Invalid file", "The provided file is not allowed.");
                            sourceControllerBase._preValueTextFileStorage.SaveTextFile(stream, safeFileName, fileId);
                        }
                        Umbraco.Cms.Core.Attempt<TemporaryFileModel?, TemporaryFileOperationStatus> attempt = await sourceControllerBase._temporaryFileService.DeleteAsync(temporaryFileKey).ConfigureAwait(false);
                        providedSettings[providedSetting.Key] = sourceControllerBase._preValueTextFileStorage.GenerateFilePath(safeFileName, fileId);
                        safeFileName = null;
                        fileId = new Guid();
                    }
                    temporaryFileKey = new Guid();
                    providedSetting = new KeyValuePair<string, string>();
                }
            }
            return TransformSettingsResult.WithSuccess();
        }

        /// <summary>
        /// Defines a result object for the transform settings operation.
        /// </summary>
        protected class TransformSettingsResult
        {
            private TransformSettingsResult()
            {
            }

            /// <summary>
            /// Gets or sets a value indicating whether the transformation was successful.
            /// </summary>
            public bool Success { get; set; }

            /// <summary>
            /// Gets or sets the problem details for a failed operation.
            /// </summary>
            public ProblemDetails? ProblemDetails { get; set; }

            /// <summary>Creates a successful result.</summary>
            public static TransformSettingsResult WithSuccess() => new()
            {
                Success = true
            };

            /// <summary>Creates a failure result.</summary>
            public static TransformSettingsResult WithFailure(
              string title,
              string detail)
            {
                ProblemDetails problemDetails = new ProblemDetailsBuilder().WithTitle(title).WithDetail(detail).Build();
                return new TransformSettingsResult()
                {
                    Success = false,
                    ProblemDetails = problemDetails
                };
            }
        }
    }
}