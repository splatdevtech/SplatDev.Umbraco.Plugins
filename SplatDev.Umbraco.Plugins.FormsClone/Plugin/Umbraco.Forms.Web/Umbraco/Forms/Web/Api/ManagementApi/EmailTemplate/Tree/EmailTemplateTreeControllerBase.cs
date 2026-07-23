
// Type: Umbraco.Forms.Web.Api.ManagementApi.EmailTemplate.Tree.EmailTemplateTreeControllerBase
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Api.Common.Attributes;
using Umbraco.Cms.Api.Management.Controllers.Tree;
using Umbraco.Cms.Core.Extensions;
using Umbraco.Cms.Core.IO;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.EmailTemplate.Tree
{
  [ApiExplorerSettings(GroupName = "Email Template")]
  [Route("/umbraco/forms/management/api/v1/tree/email-template")]
  [MapToApi("forms-management")]
  [Authorize(Policy = "SectionAccessForms")]
  [Authorize(Policy = "ManageForms")]
  public abstract class EmailTemplateTreeControllerBase : FileSystemTreeControllerBase
  {
    private readonly IIOHelper _ioHelper;
    private readonly Umbraco.Cms.Core.Hosting.IHostingEnvironment _hostingEnvironment;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ILoggerFactory _loggerFactory;

    protected EmailTemplateTreeControllerBase(
      IIOHelper ioHelper,
      Umbraco.Cms.Core.Hosting.IHostingEnvironment hostingEnvironment,
      IHostEnvironment hostEnvironment,
      ILoggerFactory loggerFactory,
      EmailTemplateCollection emailTemplateCollection)
    {
      this._ioHelper = ioHelper;
      this._hostingEnvironment = hostingEnvironment;
      this._hostEnvironment = hostEnvironment;
      this._loggerFactory = loggerFactory;
      this.EmailTemplateCollection = emailTemplateCollection;
    }

    protected EmailTemplateCollection EmailTemplateCollection { get; }

    protected override IFileSystem FileSystem => (IFileSystem) new PhysicalFileSystem(this._ioHelper, this._hostingEnvironment, this._loggerFactory.CreateLogger<PhysicalFileSystem>(), this._hostEnvironment.MapPathContentRoot("/Views/Partials/Forms/Emails"), "/Views/Partials/Forms/Emails");
  }
}
