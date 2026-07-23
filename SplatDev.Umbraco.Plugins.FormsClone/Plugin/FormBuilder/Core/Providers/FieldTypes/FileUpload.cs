using FormBuilder.Core.Attributes;
using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Extensions;
using FormBuilder.Core.FieldTypes;
using FormBuilder.Core.Models;
using FormBuilder.Core.Services.Interfaces;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

using System.Globalization;
using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Security;

namespace FormBuilder.Core.Providers.FieldTypes
{
    /// <summary>Provides a file upload field type for a form.</summary>
    [Serializable]
    public class FileUpload : FieldType
    {
        internal const string DefaultErrorMessageForMultipleFileUpload = "Only one file can be uploaded.";
        internal const string DefaultErrorMessageForInvalidFileExtension = "The file type ({0}) you tried to upload is not allowed.";
        internal const string DefaultErrorMessageForMissingFileExtension = "A file type without an extension is not allowed.";
        internal const string DefaultErrorMessageForInvalidCharacterInFileName = "The file you have uploaded has a name containing a character that is not allowed.";
        internal const string DefaultErrorMessageForFailedSecurityCheck = "The file uploaded is not allowed.";

        /// <summary>
        /// Exposes a separate used when rendering an encrypted temporary file path and an unencrypted file name
        /// for display when a file is already selected on a multi-page form.
        /// </summary>
        public static readonly string EncryptedFilePathAndFileNameSeparator = "***|***";

        private readonly IOptions<SecuritySettings> _securitySettings;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly MediaFileManager _mediaFileManager;
        private readonly IDataProtectionProvider _dataProtectionProvider;
        private readonly IFileStreamSecurityValidator _fileStreamSecurityValidator;
        private readonly IPlaceholderParsingService _placeholderParsingService;
        private IDataProtector? _dataProtector;

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        [Obsolete("Please use the constructor taking all parameters. This constructor will be removed in a future version.")]
        public FileUpload(
          IOptions<SecuritySettings> securitySettings,
          IHostEnvironment hostEnvironment,
          MediaFileManager mediaFileManager,
          IDataProtectionProvider dataProtectionProvider,
          IFileStreamSecurityValidator fileStreamSecurityValidator)
          : this(securitySettings, hostEnvironment, mediaFileManager, dataProtectionProvider, fileStreamSecurityValidator, StaticServiceProvider.Instance.GetRequiredService<IPlaceholderParsingService>())
        {
        }

        /// <summary>
        /// Initializes a new instance of the         /// </summary>
        public FileUpload(
          IOptions<SecuritySettings> securitySettings,
          IHostEnvironment hostEnvironment,
          MediaFileManager mediaFileManager,
          IDataProtectionProvider dataProtectionProvider,
          IFileStreamSecurityValidator fileStreamSecurityValidator,
          IPlaceholderParsingService placeholderParsingService)
        {
            _securitySettings = securitySettings;
            _hostEnvironment = hostEnvironment;
            _mediaFileManager = mediaFileManager;
            _dataProtectionProvider = dataProtectionProvider;
            _fileStreamSecurityValidator = fileStreamSecurityValidator;
            _placeholderParsingService = placeholderParsingService;
            Id = new Guid("84A17CF8-B711-46A6-9840-0E4A072AD000");
            Name = "File upload";
            Alias = "fileUpload";
            Description = "Renders an upload field, allowing files to be uploaded";
            Icon = "icon-download-alt";
            DataType = FieldDataType.String;
            RenderView = "file";
            Category = "Simple";
            SortOrder = 50;
            FieldTypeViewName = "FieldType.cshtml";
            PreviewView = "Forms.FieldPreview.FileUpload";
            ShowLabel = "True";
        }

        /// <summary>Gets the data protector.</summary>
        /// <value>The data protector.</value>
        /// <remarks>
        /// This is used to protect and unprotect the file path of uploaded files.
        /// </remarks>
        private IDataProtector DataProtector => _dataProtector ??= _dataProtectionProvider.CreateProtector("FormBuilderFileUpload");

        /// <inheritdoc />
        public override string RenderView => "file";

        /// <inheritdoc />
        public override bool SupportsUploadTypes => true;

        /// <summary>
        /// Gets or sets the text used to display the list of selected files in multi-page forms.
        /// </summary>
        [Setting("Selected Files List Heading", Alias = "selectedFilesListHeading", Description = "The heading used for the list of selected files for upload on multi-page forms", DisplayOrder = 10, SupportsPlaceholders = true)]
        public string SelectedFilesListHeading { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets a value indicating whether the field label should be shown.
        /// PreValues are a single element, a boolean indicating whether the default for the the checkbox is "checked".
        /// </summary>
        [Setting("Show Label", Description = "Indicate whether the the field's label should be shown when rendering the form.", DisplayOrder = 20, PreValues = "true", View = "Umb.PropertyEditorUi.Toggle")]
        public virtual string ShowLabel { get; set; }

        /// <inheritdoc />
        public override bool HideLabel => ShowLabel == "False";

        /// <summary>
        /// Gets or sets the error message to display if more than one file is uploaded when only single files are configured.
        /// </summary>
        [Setting("Error message (multiple file uploads)", Description = "The error message to display if more than one file is uploaded when only single files are configured. Leave empty to use a default message in English.", DisplayOrder = 30, SupportsPlaceholders = true)]
        public virtual string ErrorMessageForMultipleFileUpload { get; set; } = "Only one file can be uploaded.";

        /// <summary>
        /// Gets or sets the error message to display if a file with an extension that doesn't match the allowed list is uploaded.
        /// </summary>
        [Setting("Error message (extension does not match)", Description = "The error message to display if a file with an extension that doesn't match the allowed list is uploaded. Leave empty to use a default message in English.", DisplayOrder = 40, SupportsPlaceholders = true)]
        public virtual string ErrorMessageForInvalidFileExtension { get; set; } = "The file type ({0}) you tried to upload is not allowed.";

        /// <summary>
        /// Gets or sets the error message to display if a file without an extension is uploaded.
        /// </summary>
        [Setting("Error message (extension missing)", Description = "The error message to display if a file without an extension is uploaded. Leave empty to use a default message in English.", DisplayOrder = 50, SupportsPlaceholders = true)]
        public virtual string ErrorMessageForMissingFileExtension { get; set; } = "A file type without an extension is not allowed.";

        /// <summary>
        /// Gets or sets the error message to display if a file with an invalid character in its name is uploaded.
        /// </summary>
        [Setting("Error message (invalid character in file name)", Description = "The error message to display if a file with an invalid character in its name is uploaded. Leave empty to use a default message in English.", DisplayOrder = 60, SupportsPlaceholders = true)]
        public virtual string ErrorMessageForInvalidCharacterInFileName { get; set; } = "The file you have uploaded has a name containing a character that is not allowed.";

        /// <summary>
        /// Gets or sets the error message to display if a file with an invalid character in its name is uploaded.
        /// </summary>
        [Setting("Error message (failed security check)", Description = "The error message to display if a file failing a security check is uploaded. Leave empty to use a default message in English.", DisplayOrder = 70, SupportsPlaceholders = true)]
        public virtual string ErrorMessageForFailedSecurityCheck { get; set; } = "The file uploaded is not allowed.";

        private static readonly char[] sourceArray = [':'];

        /// <inheritdoc />
        public override IEnumerable<string> ValidateField(
            Form form,
            Field field,
            IEnumerable<object> postedValues,
            HttpContext context,
            IPlaceholderParsingService placeholderParsingService,
            IFieldTypeStorage fieldTypeStorage)
        {
            var errors = new List<string>();

            if (!context.Request.HasFormContentType)
                return base.ValidateField(form, field, postedValues, context, placeholderParsingService, fieldTypeStorage, errors);

            var formFiles = context.Request.Form.Files;
            var fieldId = field.Id.ToString();

            if (formFiles.Count == 0 || !formFiles.GetFiles(fieldId).Any())
                return base.ValidateField(form, field, postedValues, context, placeholderParsingService, fieldTypeStorage, errors);

            var uploadedFiles = formFiles.GetFiles(fieldId);

            // Validate multiple uploads
            if (!field.AllowMultipleFileUploads && uploadedFiles.Count > 1)
            {
                errors.Add(GetValidationErrorMessage(
                    ErrorMessageForMultipleFileUpload,
                    "Only one file can be uploaded."));
                return base.ValidateField(form, field, [], context,
                    placeholderParsingService, fieldTypeStorage, errors);
            }

            var securitySettings = _securitySettings.Value;
            var disallowedExtensions = securitySettings.GetDisallowedFileUploadExtensionsAsArray();
            var allowedExtensions = securitySettings.GetAllowedFileUploadExtensionsAsArray();

            var processedValues = new List<object>();

            foreach (var file in uploadedFiles)
            {
                var (isValid, error, fileName) = ValidateFile(file, field, disallowedExtensions, allowedExtensions);

                if (!isValid)
                {
                    errors.Add(error);
                    continue;
                }

                if (!string.IsNullOrEmpty(fileName))
                {
                    processedValues.Add(fileName);
                }
            }

            return base.ValidateField(form, field, processedValues, context,
                placeholderParsingService, fieldTypeStorage, errors);
        }

        private (bool IsValid, string Error, string? FileName) ValidateFile(
            IFormFile file,
            Field field,
            string[] disallowedExtensions,
            string[] allowedExtensions)
        {
            string? originalFileName = file.FileName;
            var storedValue = field.Values.FirstOrDefault()?.ToString();

            var filePath = originalFileName ?? (storedValue != null
                ? DecryptFilePath(storedValue)
                : null);

            if (string.IsNullOrEmpty(filePath))
                return (false, string.Empty, null);

            var validationResult = IsFileTypeAllowed(file, field, disallowedExtensions, allowedExtensions);

            return validationResult switch
            {
                FileUploadCheck.Allowed =>
                    (true, string.Empty, Path.GetFileName(filePath)),

                FileUploadCheck.FailedExtensionCheck =>
                    HandleInvalidExtension(filePath, originalFileName!),

                FileUploadCheck.FailedFileNameCheck =>
                    (false, GetValidationErrorMessage(
                        ErrorMessageForInvalidCharacterInFileName,
                        "The file name contains invalid characters."), null),

                FileUploadCheck.FailedFileStreamSecurityCheck =>
                    (false, GetValidationErrorMessage(
                        ErrorMessageForFailedSecurityCheck,
                        "File failed security checks."), null),

                _ => (false, string.Empty, null)
            };
        }

        private (bool IsValid, string Error, string? FileName) HandleInvalidExtension(string filePath, string originalFileName)
        {
            if (string.IsNullOrEmpty(originalFileName))
                return (false, string.Empty, null);

            var hasExtension = Path.HasExtension(filePath);
            var extension = hasExtension ? Path.GetExtension(filePath) : null;

            var errorMessage = hasExtension
                ? GetValidationErrorMessage(
                    ErrorMessageForInvalidFileExtension,
                    $"File type '{extension}' is not allowed.")
                : GetValidationErrorMessage(
                    ErrorMessageForMissingFileExtension,
                    "Files without extensions are not allowed.");

            return (false, errorMessage, null);
        }

        private string GetValidationErrorMessage(string messageFromSetting, string defaultMessage)
        {
            string str = messageFromSetting;
            if (string.IsNullOrWhiteSpace(str))
                str = defaultMessage;
            return _placeholderParsingService.ParsePlaceHolders(str, true);
        }

        private FileUploadCheck IsFileTypeAllowed(
          IFormFile file,
          Field field,
          string[] alwaysDisallowedExtensions,
          string[] onlyAllowedExtensions)
        {
            if (!file.IsFileTypeAllowed(field, alwaysDisallowedExtensions, onlyAllowedExtensions))
                return FileUploadCheck.FailedExtensionCheck;
            if (sourceArray.Any(x => file.FileName.Contains(x)))
                return FileUploadCheck.FailedFileNameCheck;
            return !_fileStreamSecurityValidator.IsConsideredSafe(file.OpenReadStream()) ? FileUploadCheck.FailedFileStreamSecurityCheck : FileUploadCheck.Allowed;
        }

        /// <inheritdoc />
        public override IEnumerable<object> ProcessSubmittedValue(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            bool flag1 = false;
            bool flag2 = false;
            List<object> objectList = [];
            string name = field.Id.ToString();
            IReadOnlyList<IFormFile> files = context.Request.Form.Files.GetFiles(name);
            string[] extensionsAsArray1 = _securitySettings.Value.GetDisallowedFileUploadExtensionsAsArray();
            string[] extensionsAsArray2 = _securitySettings.Value.GetAllowedFileUploadExtensionsAsArray();
            for (int index = 0; index < files.Count; ++index)
            {
                IFormFile file = files[index];
                if (IsFileTypeAllowed(file, field, extensionsAsArray1, extensionsAsArray2) == FileUploadCheck.Allowed && file is not null && file.Length > 0L)
                {
                    flag1 = true;
                    break;
                }
            }
            string hiddenFieldKeyToSearch = string.Format("{0}_file_", field.Id);
            string[] array = [.. context.Request.Form.Keys.Where(x => x.StartsWith(hiddenFieldKeyToSearch))];
            if (array.Length != 0)
                flag2 = true;
            if (flag1)
            {
                if (flag2)
                {
                    foreach (string key in array)
                    {
                        string path1 = DecryptFilePath(context.Request.Form[key].ToString());
                        if (path1.StartsWith("~/umbraco/Data/TEMP/FileUploads", StringComparison.OrdinalIgnoreCase))
                        {
                            string path2 = _hostEnvironment.MapPathContentRoot(path1);
                            string? directoryName = Path.GetDirectoryName(path2);
                            if (System.IO.File.Exists(path2))
                                System.IO.File.Delete(path2);
                            if (Directory.Exists(directoryName))
                                Directory.Delete(directoryName);
                        }
                    }
                    flag2 = false;
                }
                for (int index = 0; index < files.Count; ++index)
                {
                    IFormFile file = files[index];
                    if (IsFileTypeAllowed(file, field, extensionsAsArray1, extensionsAsArray2) == FileUploadCheck.Allowed && file.Length > 0L)
                    {
                        string tempFolder = file.SaveUploadedFileToTempFolder(_hostEnvironment);
                        objectList.Add(EncryptFilePath(tempFolder));
                    }
                }
            }
            if (flag2)
            {
                foreach (string key in array)
                {
                    string str = context.Request.Form[key].ToString();
                    objectList.Add(str);
                }
            }
            return objectList;
        }

        /// <inheritdoc />
        public override IEnumerable<object> ConvertToRecord(
          Field field,
          IEnumerable<object> postedValues,
          HttpContext context)
        {
            List<object> processed = [];
            foreach (object postedValue in postedValues)
            {
                if (postedValue is not null && !string.IsNullOrEmpty(postedValue.ToString()))
                {
                    string str1 = DecryptFilePath(postedValue.ToString()!);
                    string str2 = _hostEnvironment.MapPathContentRoot(str1.ToString());
                    string[] extensionsAsArray1 = _securitySettings.Value.GetDisallowedFileUploadExtensionsAsArray();
                    string[] extensionsAsArray2 = _securitySettings.Value.GetAllowedFileUploadExtensionsAsArray();
                    string lowerInvariant = Path.GetExtension(str2).ToLowerInvariant();
                    string str3 = lowerInvariant;
                    if (!extensionsAsArray1.Contains(str3) && (extensionsAsArray2.Length == 0 || extensionsAsArray2.Contains(lowerInvariant)))
                    {
                        string? str4;
                        if (context.Request.HasFormContentType && context.Request.Form.ContainsKey("FormId"))
                            str4 = (string?)context.Request.Form["FormId"];
                        else if (context.Items.ContainsKey("ApiSubmittedFormId"))
                            str4 = ((Guid?)context.Items["ApiSubmittedFormId"]).ToString();
                        else
                            continue;
                        DefaultInterpolatedStringHandler interpolatedStringHandler = new(1, 2);
                        interpolatedStringHandler.AppendFormatted(field.Id);
                        interpolatedStringHandler.AppendLiteral("/");
                        interpolatedStringHandler.AppendFormatted(Path.GetFileName(str1.ToString()));
                        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                        string str5 = "forms/upload";
                        interpolatedStringHandler = new DefaultInterpolatedStringHandler(8, 4);
                        interpolatedStringHandler.AppendFormatted(str5);
                        interpolatedStringHandler.AppendLiteral("/form_");
                        interpolatedStringHandler.AppendFormatted(str4);
                        interpolatedStringHandler.AppendLiteral("/");
                        interpolatedStringHandler.AppendFormatted(Guid.NewGuid());
                        interpolatedStringHandler.AppendLiteral("/");
                        interpolatedStringHandler.AppendFormatted(Path.GetFileName(stringAndClear));
                        string lower = interpolatedStringHandler.ToStringAndClear().ToLower(CultureInfo.InvariantCulture);
                        string str6 = _hostEnvironment.MapPathContentRoot("~/umbraco/Data/TEMP/FileUploads");
                        if (System.IO.File.Exists(str2) && str2.StartsWith(str6))
                        {
                            using (MemoryStream destination = new())
                            {
                                using (FileStream fileStream = System.IO.File.OpenRead(str2))
                                {
                                    fileStream.CopyTo(destination);
                                    destination.Position = 0L;
                                }
                                FileInfo fileInfo = new(str2);
                                if (fileInfo.Directory is not null && Directory.Exists(fileInfo.Directory.FullName))
                                {
                                    System.IO.File.Delete(str2);
                                    if (!Directory.EnumerateFileSystemEntries(fileInfo.Directory.FullName).Any())
                                        Directory.Delete(fileInfo.Directory.FullName);
                                }
                                _mediaFileManager.FileSystem.AddFile(lower, destination);
                            }
                            AddValue(processed, lower);
                        }
                        else
                            processed.Add(str1);
                    }
                }
            }
            return processed;
        }

        private void AddValue(List<object> processed, string path) => processed.Add(_mediaFileManager.FileSystem.GetUrl(path));

        private string EncryptFilePath(string path) => path.EncryptFilePath(DataProtector, EncryptedFilePathAndFileNameSeparator);

        private string DecryptFilePath(string path) => path.DecryptFilePath(DataProtector, EncryptedFilePathAndFileNameSeparator);

        /// <summary>
        /// When retrieving the record value and using it to display in the edit record,
        /// we modify the list of objects we send back.
        /// In our case we need the file path so it can be clicked to open and see the existing file
        /// and secondly we just want the file name not the path and not have loads of logic in the view so hence its here.
        /// </summary>
        /// <param name="field">The upload field in the form.</param>
        /// <param name="storedValues">The currently stored values for this upload field that we will transform.</param>
        /// <returns>A list of uploads containing the file path and the filename as a dictionary.</returns>
        public override IEnumerable<object> ConvertFromRecord(
          Field field,
          IEnumerable<object> storedValues)
        {
            List<object> objectList = [];
            foreach (object storedValue in storedValues)
            {
                string str = EncryptFilePath(storedValue.ToString()!.ToLower(CultureInfo.InvariantCulture));
                objectList.Add(str);
            }
            return objectList;
        }

        private enum FileUploadCheck
        {
            Allowed,
            FailedExtensionCheck,
            FailedFileNameCheck,
            FailedFileStreamSecurityCheck,
        }
    }
}