using FormBuilder.Core.Configuration;
using FormBuilder.Core.Helpers;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers.Collections;
using FormBuilder.Core.Security.Interfaces;
using FormBuilder.Core.Services.Interfaces;
using FormBuilder.Web.Extensions;

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Runtime.CompilerServices;

using Umbraco.Cms.Core.Mapping;

using Umbraco.Cms.Core.Models;

using Umbraco.Cms.Core.Security;

using Umbraco.Cms.Core.Services;

namespace FormBuilder.Web.Api.Management.Controllers
{
    /// <summary>
    /// Management API controller for retrieving a single form by Id.
    /// </summary>
    /// <remarks>
    /// Initializes a new instance of the     /// </remarks>
    public class GetByKeyFormController(
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
      IOptions<FormDesignSettings> formDesignSettings) : FormControllerBase(formService, folderService, workflowService, fieldService, fieldTypeStorage, workflowCollection, backOfficeSecurityAccessor, formsSecurity, mapper, logger, htmlSanitizer)
    {
        private readonly IEntityService _entityService = entityService;
        private readonly IDictionaryHelper _dictionaryHelper = dictionaryHelper;
        private readonly IHostingEnvironment _hostingEnvironment = hostingEnvironment;
        private readonly FormDesignSettings _formDesignSettings = formDesignSettings.Value;

        /// <summary>
        /// Management API endpoint for retrieving a single form by Id.
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(FormDesign), 200)]
        [ProducesResponseType(403)]
        [ProducesResponseType(404)]
        public IActionResult GetByKey(Guid id, bool applyDictionaryTranslations)
        {
            Form? form1 = FormService.Get(id);
            if (form1 is null)
                return NotFound();
            if (!ValidateAccessToForm(form1))
                return Forbid();
            foreach (FieldSet fieldSet in form1.Pages.SelectMany(x => x.FieldSets))
            {
                if (fieldSet.Id == Guid.Empty)
                    fieldSet.Id = Guid.NewGuid();
            }
            if (applyDictionaryTranslations)
                form1.ApplyDictionaryTranslationsToPrevalueCaptions(_dictionaryHelper);
            FormDesign form2 = Mapper.Map<FormDesign>(form1) ?? throw new InvalidOperationException("Could not map from object");
            FormWorkflows workflowsForForm = GetWorkflowsForForm(form1);
            if (!string.IsNullOrWhiteSpace(form2.GoToPageOnSubmit))
            {
                if (form2.GoToPageOnSubmit == "0")
                    form2.GoToPageOnSubmit = null;
                if (int.TryParse(form2.GoToPageOnSubmit, out int result))
                {
                    Umbraco.Cms.Core.Attempt<Guid> key = _entityService.GetKey(result, UmbracoObjectTypes.Document);
                    if (key.Success)
                        form2.GoToPageOnSubmit = key.Result.ToString();
                    else
                        form2.GoToPageOnSubmit = null;
                }
            }
            form2.FormWorkflows = workflowsForForm;
            SetPath(form2);
            return Ok(form2);
        }

        private IList<Guid> GetStartFoldersForCurrentUser() => [.. FormsSecurity.GetStartFolderKeysForCurrentUser()];

        private void SetPath(FormDesign form)
        {
            if (form.FolderId.HasValue)
            {
                string folderPath = GetFolderPath(form.FolderId.Value);
                FormDesign formDesign = form;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(1, 2);
                interpolatedStringHandler.AppendFormatted(folderPath);
                interpolatedStringHandler.AppendLiteral(",");
                interpolatedStringHandler.AppendFormatted(form.Id);
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                formDesign.Path = stringAndClear;
            }
            else
            {
                FormDesign formDesign = form;
                DefaultInterpolatedStringHandler interpolatedStringHandler = new(1, 2);
                interpolatedStringHandler.AppendFormatted("-1");
                interpolatedStringHandler.AppendLiteral(",");
                interpolatedStringHandler.AppendFormatted(form.Id);
                string stringAndClear = interpolatedStringHandler.ToStringAndClear();
                formDesign.Path = stringAndClear;
            }
        }

        private string GetFolderPath(Guid folderId)
        {
            string path = FolderService.GetPath(folderId);
            IList<Guid> foldersForCurrentUser = GetStartFoldersForCurrentUser();
            return foldersForCurrentUser.Count == 0 ? path : path.ModifyFolderPathForStartFolders(foldersForCurrentUser);
        }
    }
}