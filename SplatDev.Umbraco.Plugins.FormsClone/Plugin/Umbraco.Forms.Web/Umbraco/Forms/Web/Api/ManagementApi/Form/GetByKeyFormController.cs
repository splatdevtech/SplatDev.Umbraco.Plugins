
// Type: Umbraco.Forms.Web.Api.ManagementApi.Form.GetByKeyFormController
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Umbraco.Cms.Core.Hosting;
using Umbraco.Cms.Core.Mapping;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Forms.Core.Configuration;
using Umbraco.Forms.Core.Data.Helpers;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;
using Umbraco.Forms.Core.Security;
using Umbraco.Forms.Core.Services;
using Umbraco.Forms.Web.Extensions;
using Umbraco.Forms.Web.Models.Backoffice;


#nullable enable
namespace Umbraco.Forms.Web.Api.ManagementApi.Form
{
  public class GetByKeyFormController : FormControllerBase
  {
    private readonly IEntityService _entityService;
    private readonly IDictionaryHelper _dictionaryHelper;
    private readonly IHostingEnvironment _hostingEnvironment;
    private readonly FormDesignSettings _formDesignSettings;

    public GetByKeyFormController(
      IFormService formService,
      IFolderService folderService,
      IWorkflowService workflowService,
      IFieldService fieldService,
      IFieldTypeStorage fieldTypeStorage,
      WorkflowCollection workflowCollection,
      IBackOfficeSecurityAccessor backOfficeSecurityAccessor,
      IFormsSecurity formsSecurity,
      IUmbracoMapper mapper,
      ILogger<GetByKeyFormController> logger,
      IHtmlSanitizer htmlSanitizer,
      IEntityService entityService,
      IDictionaryHelper dictionaryHelper,
      IHostingEnvironment hostingEnvironment,
      IOptions<FormDesignSettings> formDesignSettings)
      : base(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, (ILogger) logger, htmlSanitizer)
    {
      this._entityService = entityService;
      this._dictionaryHelper = dictionaryHelper;
      this._hostingEnvironment = hostingEnvironment;
      this._formDesignSettings = formDesignSettings.Value;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof (FormDesign), 200)]
    [ProducesResponseType(403)]
    [ProducesResponseType(404)]
    public IActionResult GetByKey(Guid id, bool applyDictionaryTranslations)
    {
      Umbraco.Forms.Core.Models.Form form1 = this.FormService.Get(id);
      if (form1 == null)
        return (IActionResult) this.NotFound();
      if (!this.ValidateAccessToForm(form1))
        return (IActionResult) this.Forbid();
      foreach (FieldSet fieldSet in form1.Pages.SelectMany<Page, FieldSet>((Func<Page, IEnumerable<FieldSet>>) (x => (IEnumerable<FieldSet>) x.FieldSets)))
      {
        if (fieldSet.Id == Guid.Empty)
          fieldSet.Id = Guid.NewGuid();
      }
      if (applyDictionaryTranslations)
        form1.ApplyDictionaryTranslationsToPrevalueCaptions(this._dictionaryHelper);
      FormDesign form2 = this.Mapper.Map<FormDesign>((object) form1) ?? throw new InvalidOperationException("Could not map from object");
      FormWorkflows workflowsForForm = this.GetWorkflowsForForm(form1);
      if (!string.IsNullOrWhiteSpace(form2.GoToPageOnSubmit))
      {
        if (form2.GoToPageOnSubmit == "0")
          form2.GoToPageOnSubmit = (string) null;
        int result;
        if (int.TryParse(form2.GoToPageOnSubmit, out result))
        {
          Umbraco.Cms.Core.Attempt<Guid> key = this._entityService.GetKey(result, UmbracoObjectTypes.Document);
          if (key.Success)
            form2.GoToPageOnSubmit = key.Result.ToString();
          else
            form2.GoToPageOnSubmit = (string) null;
        }
      }
      form2.FormWorkflows = workflowsForForm;
      this.SetPath(form2);
      return (IActionResult) this.Ok((object) form2);
    }

    private IList<Guid> GetStartFoldersForCurrentUser() => (IList<Guid>) this.FormsSecurity.GetStartFolderKeysForCurrentUser().ToList<Guid>();

    private void SetPath(FormDesign form)
    {
      if (form.FolderId.HasValue)
      {
        string folderPath = this.GetFolderPath(form.FolderId.Value);
        FormDesign formDesign = form;
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
        interpolatedStringHandler.AppendFormatted(folderPath);
        interpolatedStringHandler.AppendLiteral(",");
        interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
        formDesign.Path = stringAndClear;
      }
      else
      {
        FormDesign formDesign = form;
        DefaultInterpolatedStringHandler interpolatedStringHandler = new DefaultInterpolatedStringHandler(1, 2);
        interpolatedStringHandler.AppendFormatted("-1");
        interpolatedStringHandler.AppendLiteral(",");
        interpolatedStringHandler.AppendFormatted<Guid>(form.Id);
        string stringAndClear = interpolatedStringHandler.ToStringAndClear();
        formDesign.Path = stringAndClear;
      }
    }

    private string GetFolderPath(Guid folderId)
    {
      string path = this.FolderService.GetPath(folderId);
      IList<Guid> foldersForCurrentUser = this.GetStartFoldersForCurrentUser();
      return foldersForCurrentUser.Count == 0 ? path : path.ModifyFolderPathForStartFolders(foldersForCurrentUser);
    }
  }
}
