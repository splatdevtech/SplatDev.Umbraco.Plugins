using FormBuilder.Core.Providers.Collections;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using Umbraco.Cms.Api.Common.Attributes;

using Umbraco.Cms.Api.Management.Controllers.Tree;
using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.IO;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API base controller for common functionality when working with the email templates tree.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    [ApiExplorerSettings(GroupName = "Email Template")]
    [Route("/formBuilder/management/api/v1/tree/email-template")]
    [MapToApi("formBuilder-management")]
    [Authorize(Policy = "SectionAccessForms")]
    [Authorize(Policy = "ManageForms")]
    public abstract class EmailTemplateTreeControllerBase(
      IIOHelper ioHelper,
      IHostingEnvironment hostingEnvironment,
      Microsoft.Extensions.Hosting.IHostEnvironment hostEnvironment,
      ILoggerFactory loggerFactory,
      EmailTemplateCollection emailTemplateCollection) : FileSystemTreeControllerBase
    {
        private readonly IIOHelper _ioHelper = ioHelper;
        private readonly IHostingEnvironment _hostingEnvironment = hostingEnvironment;
        private readonly Microsoft.Extensions.Hosting.IHostEnvironment _hostEnvironment = hostEnvironment;
        private readonly ILoggerFactory _loggerFactory = loggerFactory;

        /// <summary>
        /// The         /// </summary>
        protected EmailTemplateCollection EmailTemplateCollection { get; } = emailTemplateCollection;

        /// <summary>
        /// Gets the         /// </summary>
        protected override IFileSystem FileSystem => new PhysicalFileSystem(_ioHelper, _hostingEnvironment, _loggerFactory.CreateLogger<PhysicalFileSystem>(), _hostEnvironment.MapPathContentRoot("/Views/Partials/Forms/Emails"), "/Views/Partials/Forms/Emails");
    }
}