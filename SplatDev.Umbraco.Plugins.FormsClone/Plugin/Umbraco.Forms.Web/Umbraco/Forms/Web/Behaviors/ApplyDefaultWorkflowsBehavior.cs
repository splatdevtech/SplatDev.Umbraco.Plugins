
// Type: Umbraco.Forms.Web.Behaviors.ApplyDefaultWorkflowsBehavior
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models.Membership;
using Umbraco.Cms.Core.Security;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Web.Extensions;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Behaviors
{
    internal class ApplyDefaultWorkflowsBehavior : IApplyDefaultWorkflowsBehavior
    {
        private readonly WorkflowCollectionFactory _workflowCollectionFactory;
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;
        private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;
        private readonly IHostEnvironment _hostEnvironment;
        private readonly FormDesignSettings _formDesignSettings;
        private readonly IIOHelper _ioHelper;
        private readonly ILoggerFactory _loggerFactory;

        public ApplyDefaultWorkflowsBehavior(
          WorkflowCollectionFactory workflowCollectionFactory,
          IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
          Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
          IHostEnvironment hostEnvironment,
          IOptions<FormDesignSettings> formDesignSettings,
          IIOHelper ioHelper,
          ILoggerFactory loggerFactory)
        {
            this._workflowCollectionFactory = workflowCollectionFactory;
            this._backOfficeSecurityAccessor = backOfficeSecurityAccessor;
            this._hostingEnvironment = hostingEnvironment;
            this._hostEnvironment = hostEnvironment;
            this._formDesignSettings = formDesignSettings.Value;
            this._ioHelper = ioHelper;
            this._loggerFactory = loggerFactory;
        }

        public virtual void ApplyDefaultWorkflows(FormDesign form)
        {
            if (this._formDesignSettings.DisableDefaultWorkflow)
                return;
            WorkflowCollection workflowCollection = this._workflowCollectionFactory.GetWorkflowCollection();
            Guid emailWorkflowTypeId = new Guid("17C61629-D984-4E86-B43B-A8407B3EFEA9");
            Func<WorkflowType, bool> predicate = x => x.Id == emailWorkflowTypeId;
            WorkflowType workflowType = workflowCollection.FirstOrDefault<WorkflowType>(predicate);
            if (workflowType == null)
                return;
            string str = string.Empty;
            IUser currentUser = this._backOfficeSecurityAccessor.BackOfficeSecurity?.CurrentUser;
            if (currentUser != null)
                str = currentUser.Email;
            if (string.IsNullOrEmpty(str))
                return;
            FormWorkflowWithTypeSettings withTypeSettings = new FormWorkflowWithTypeSettings()
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
            List<SettingWithValue> settingsWithValues = workflowType.GetSettingsWithValues(this._hostingEnvironment, this._formDesignSettings.SettingsCustomization.WorkflowTypes.GetValueForProviderType(workflowType));
            SettingWithValue settingWithValue1 = settingsWithValues.SingleOrDefault<SettingWithValue>(x => string.Equals(x.Alias, "Email", StringComparison.OrdinalIgnoreCase));
            if (settingWithValue1 != null)
                settingWithValue1.Value = str;
            SettingWithValue settingWithValue2 = settingsWithValues.SingleOrDefault<SettingWithValue>(x => string.Equals(x.Alias, "Subject", StringComparison.OrdinalIgnoreCase));
            if (settingWithValue2 != null)
                settingWithValue2.Value = "The Form '{form_name}' was submitted";
            SettingWithValue settingWithValue3 = settingsWithValues.SingleOrDefault<SettingWithValue>(x => string.Equals(x.Alias, "RazorViewFilePath", StringComparison.OrdinalIgnoreCase));
            if (settingWithValue3 != null)
                settingWithValue3.Value = this.GetDefaultEmailTemplatePath();
            withTypeSettings.Settings = settingsWithValues.ToDictionary<SettingWithValue, string, string>(x => x.Alias, x => x.Value);
            form.FormWorkflows.OnSubmit.Add(withTypeSettings);
        }

        private string GetDefaultEmailTemplatePath()
        {
            string defaultEmailTemplate = this._formDesignSettings.DefaultEmailTemplate;
            return !string.IsNullOrEmpty(defaultEmailTemplate) && this.EmailTemplateExists(defaultEmailTemplate) ? defaultEmailTemplate : "Forms/Emails/Example-Template.cshtml";
        }

        private bool EmailTemplateExists(string relativePath)
        {
            string[] source = relativePath.Split('/');
            if (source.Length == 0)
                return false;
            int num = source.Length == 1 ? 1 : 0;
            return this.EmailTemplateExists("~/Views/Partials/" + (num != 0 ? string.Empty : string.Join("/", source.Take<string>(source.Length - 1))), num != 0 ? source[0] : source.LastOrDefault<string>() ?? string.Empty);
        }

        private bool EmailTemplateExists(string folder, string fileName) => this.CreateEmailFileSystem(this._hostEnvironment.MapPathContentRoot(folder), folder).FileExists(fileName);

        private PhysicalFileSystem CreateEmailFileSystem(
          string rootPath,
          string rootUrl)
        {
            return new PhysicalFileSystem(this._ioHelper, this._hostingEnvironment, this._loggerFactory.CreateLogger<PhysicalFileSystem>(), rootPath, rootUrl);
        }
    }
}
