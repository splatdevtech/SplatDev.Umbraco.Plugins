using FormBuilder.Core.Configuration;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Providers.Factories;
using FormBuilder.Core.Workflows;
using FormBuilder.Web.Behaviors.Interfaces;
using FormBuilder.Web.Extensions;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;

namespace FormBuilder.Web.Behaviors
{
    /// <summary>
    /// Default implementation of the     /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    internal class ApplyDefaultWorkflowsBehavior(
      WorkflowCollectionFactory workflowCollectionFactory,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
      IHostEnvironment hostEnvironment,
      IOptions<FormDesignSettings> formDesignSettings,
      IIOHelper ioHelper,
      ILoggerFactory loggerFactory) : IApplyDefaultWorkflowsBehavior
    {
        private readonly WorkflowCollectionFactory _workflowCollectionFactory = workflowCollectionFactory;
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor = backOfficeSecurityAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment = hostingEnvironment;
        private readonly IHostEnvironment _hostEnvironment = hostEnvironment;
        private readonly FormDesignSettings _formDesignSettings = formDesignSettings.Value;
        private readonly IIOHelper _ioHelper = ioHelper;
        private readonly ILoggerFactory _loggerFactory = loggerFactory;

        /// <inheritdoc />
        public virtual void ApplyDefaultWorkflows(FormDesign form)
        {
            if (_formDesignSettings.DisableDefaultWorkflow)
                return;
            WorkflowCollection workflowCollection = _workflowCollectionFactory.GetWorkflowCollection();
            Guid emailWorkflowTypeId = new("17C61629-D984-4E86-B43B-A8407B3EFEA9");
            bool predicate(WorkflowType x) => x.Id == emailWorkflowTypeId;
            WorkflowType? workflowType = workflowCollection.FirstOrDefault(predicate);
            if (workflowType is null)
                return;
            string str = string.Empty;
            IUser? currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            if (currentUser is not null)
                str = currentUser.Email;
            if (string.IsNullOrEmpty(str))
                return;
            FormWorkflowWithTypeSettings withTypeSettings = new()
            {
                Id = Guid.Empty,
                Name = string.Format("Send template email to {0}", str),
                Active = true,
                IncludeSensitiveData = IncludeSensitiveData.False,
                IsDeleted = false,
                Form = Guid.Empty,
                SortOrder = 1,
                WorkflowTypeId = workflowType.Id,
                WorkflowTypeName = workflowType.Name,
                WorkflowTypeDescription = workflowType.Description,
                WorkflowTypeGroup = workflowType.Group,
                WorkflowTypeIcon = workflowType.Icon
            };
            List<SettingWithValue> settingsWithValues = workflowType.GetSettingsWithValues(_formDesignSettings.SettingsCustomization.WorkflowTypes.GetValueForProviderType(workflowType));
            SettingWithValue? settingWithValue1 = settingsWithValues.SingleOrDefault(x => string.Equals(x.Alias, "Email", StringComparison.OrdinalIgnoreCase));
            if (settingWithValue1 is not null)
                settingWithValue1.Value = str;
            SettingWithValue? settingWithValue2 = settingsWithValues.SingleOrDefault(x => string.Equals(x.Alias, "Subject", StringComparison.OrdinalIgnoreCase));
            if (settingWithValue2 is not null)
                settingWithValue2.Value = "The Form '{form_name}' was submitted";
            SettingWithValue? settingWithValue3 = settingsWithValues.SingleOrDefault(x => string.Equals(x.Alias, "RazorViewFilePath", StringComparison.OrdinalIgnoreCase));
            if (settingWithValue3 is not null)
                settingWithValue3.Value = GetDefaultEmailTemplatePath();
            withTypeSettings.Settings = settingsWithValues.ToDictionary(x => x.Alias, x => x.Value);
            form.FormWorkflows.OnSubmit.Add(withTypeSettings);
        }

        private string GetDefaultEmailTemplatePath()
        {
            string defaultEmailTemplate = _formDesignSettings.DefaultEmailTemplate;
            return !string.IsNullOrEmpty(defaultEmailTemplate) && EmailTemplateExists(defaultEmailTemplate) ? defaultEmailTemplate : "Forms/Emails/Example-Template.cshtml";
        }

        private bool EmailTemplateExists(string relativePath)
        {
            string[] source = relativePath.Split('/');
            if (source.Length == 0)
                return false;
            int num = source.Length == 1 ? 1 : 0;
            return EmailTemplateExists("~/Views/Partials/" + (num != 0 ? string.Empty : string.Join("/", source.Take(source.Length - 1))), num != 0 ? source[0] : source.LastOrDefault() ?? string.Empty);
        }

        private bool EmailTemplateExists(string folder, string fileName) => CreateEmailFileSystem(_hostEnvironment.MapPathContentRoot(folder), folder).FileExists(fileName);

        private PhysicalFileSystem CreateEmailFileSystem(
          string rootPath,
          string rootUrl)
        {
            return new PhysicalFileSystem(_ioHelper, _hostingEnvironment, _loggerFactory.CreateLogger<PhysicalFileSystem>(), rootPath, rootUrl);
        }
    }
}