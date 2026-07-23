
// Type: Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource.CreateOrUpdatePrevalueSourceControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Umbraco.Cms.Api.Common.Builders;
using Umbraco.Cms.Core.Models.TemporaryFile;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Services.OperationStatus;
using Umbraco.Cms.Core.Strings;
using Umbraco.Extensions;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Data;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.PrevalueSource
{
  [Authorize(Policy = "ManagePrevalueSources")]
  public abstract class CreateOrUpdatePrevalueSourceControllerBase : PrevalueSourceControllerBase
  {
    private readonly ITemporaryFileService _temporaryFileService;
    private readonly IShortStringHelper _shortStringHelper;
    private readonly IFileStreamSecurityValidator _fileStreamSecurityValidator;
    private readonly IPreValueTextFileStorage _preValueTextFileStorage;

    protected CreateOrUpdatePrevalueSourceControllerBase(
      IPrevalueSourceService prevalueSourceService,
      FieldPreValueSourceCollection fieldPreValueSources,
      IFieldPreValueSourceTypeService fieldPreValueSourceTypeService,
      ITemporaryFileService temporaryFileService,
      IShortStringHelper shortStringHelper,
      IFileStreamSecurityValidator fileStreamSecurityValidator,
      IPreValueTextFileStorage preValueTextFileStorage)
      : base(fieldPreValueSources, prevalueSourceService, fieldPreValueSourceTypeService)
    {
      this._temporaryFileService = temporaryFileService;
      this._shortStringHelper = shortStringHelper;
      this._fileStreamSecurityValidator = fileStreamSecurityValidator;
      this._preValueTextFileStorage = preValueTextFileStorage;
    }

    protected ProviderValidationResult TryValidateProviderType(
      FieldPreValueSource preValueSource,
      out ProblemDetails? problemDetails)
    {
      problemDetails = (ProblemDetails) null;
      FieldPreValueSourceType byId = this.FieldPreValueSourceTypeService.GetById(preValueSource.FieldPreValueSourceTypeId);
      if (byId == null)
        return ProviderValidationResult.FailedTypeNotFound;
      byId.LoadSettings(preValueSource);
      List<Exception> exceptionList = byId.ValidateSettings();
      if (!exceptionList.Any<Exception>())
        return ProviderValidationResult.Success;
      problemDetails = FormsManagementApiControllerBase.BuildSettingsValidationProblemDetails(exceptionList);
      return ProviderValidationResult.FailedValidation;
    }

    protected async Task<CreateOrUpdatePrevalueSourceControllerBase.TransformSettingsResult> TransformSettings(
      FieldPreValueSource preValueSource)
    {
      CreateOrUpdatePrevalueSourceControllerBase sourceControllerBase = this;
      FieldPreValueSourceType byId = sourceControllerBase.FieldPreValueSourceTypeService.GetById(preValueSource.FieldPreValueSourceTypeId);
      if (byId == null)
      {
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(48, 1);
        interpolatedStringHandler.AppendLiteral("Could not find prevalue source type with id of: ");
        interpolatedStringHandler.AppendFormatted<Guid>(preValueSource.FieldPreValueSourceTypeId);
        throw new InvalidOperationException(interpolatedStringHandler.ToStringAndClear());
      }
      Dictionary<string, SettingAttribute> typeSettings = byId.Settings();
      Dictionary<string, string> providedSettings = preValueSource.Settings;
      foreach (KeyValuePair<string, string> keyValuePair in providedSettings)
      {
        KeyValuePair<string, string> providedSetting = keyValuePair;
        SettingAttribute settingAttribute;
        typeSettings.TryGetValue(providedSetting.Key, out settingAttribute);
        if (settingAttribute != null)
        {
          Guid temporaryFileKey;
          if (settingAttribute.View == "Umb.PropertyEditorUi.UploadField" && Guid.TryParse(providedSetting.Value, out temporaryFileKey))
          {
            TemporaryFileModel temporaryFileModel = await sourceControllerBase._temporaryFileService.GetAsync(temporaryFileKey).ConfigureAwait(false) ?? throw new InvalidOperationException("Could not get temporary uploaded file for setting.");
            string safeFileName = temporaryFileModel.FileName.ToSafeFileName(sourceControllerBase._shortStringHelper);
            string str = safeFileName;
            int startIndex = safeFileName.LastIndexOf('.') + 1;
            if (str.Substring(startIndex, str.Length - startIndex).ToLower() != "txt")
              return CreateOrUpdatePrevalueSourceControllerBase.TransformSettingsResult.WithFailure("Invalid file extension", "Only .txt files can be uploaded.");
            Guid fileId = Guid.NewGuid();
            using (Stream stream = temporaryFileModel.OpenReadStream())
            {
              if (!sourceControllerBase._fileStreamSecurityValidator.IsConsideredSafe(stream))
                return CreateOrUpdatePrevalueSourceControllerBase.TransformSettingsResult.WithFailure("Invalid file", "The provided file is not allowed.");
              sourceControllerBase._preValueTextFileStorage.SaveTextFile(stream, safeFileName, fileId);
            }
            Umbraco.Cms.Core.Attempt<TemporaryFileModel, TemporaryFileOperationStatus> attempt = await sourceControllerBase._temporaryFileService.DeleteAsync(temporaryFileKey).ConfigureAwait(false);
            providedSettings[providedSetting.Key] = sourceControllerBase._preValueTextFileStorage.GenerateFilePath(safeFileName, fileId);
            safeFileName = (string) null;
            fileId = new Guid();
          }
          temporaryFileKey = new Guid();
          providedSetting = new KeyValuePair<string, string>();
        }
      }
      return CreateOrUpdatePrevalueSourceControllerBase.TransformSettingsResult.WithSuccess();
    }

    protected class TransformSettingsResult
    {
      private TransformSettingsResult()
      {
      }

      public bool Success { get; set; }

      public ProblemDetails? ProblemDetails { get; set; }

      public static CreateOrUpdatePrevalueSourceControllerBase.TransformSettingsResult WithSuccess() => new CreateOrUpdatePrevalueSourceControllerBase.TransformSettingsResult()
      {
        Success = true
      };

      public static CreateOrUpdatePrevalueSourceControllerBase.TransformSettingsResult WithFailure(
        string title,
        string detail)
      {
        ProblemDetails problemDetails = new ProblemDetailsBuilder().WithTitle(title).WithDetail(detail).Build();
        return new CreateOrUpdatePrevalueSourceControllerBase.TransformSettingsResult()
        {
          Success = false,
          ProblemDetails = problemDetails
        };
      }
    }
  }
}
