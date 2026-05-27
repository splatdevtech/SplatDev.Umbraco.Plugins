using FormBuilder.Extension.Interfaces;
using FormBuilder.Extension.Models;
using FormBuilder.Extension.Workflows;

using Microsoft.AspNetCore.Mvc;

namespace FormBuilder.Extension.Controllers
{
    [Route("umbraco/api/formbuilder/submit")]
    public class FormSubmissionController(IFormSubmissionValidator validator, IFormRepository formRepository, WorkflowSchema workflowSchema) : ControllerBase
    {
        private readonly IFormSubmissionValidator _validator = validator;
        private readonly IFormRepository _formRepository = formRepository;
        private readonly WorkflowSchema _workflowSchema = workflowSchema;

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit([FromBody] FormSubmissionModel model)
        {
            var validationResult = await _validator.ValidateAsync(model);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var submission = await _formRepository.CreateSubmissionAsync(model);
            var context = new WorkflowExecutionContext(model.FormId, model.FormGuid)
            {
                Record = new FormSubmission
                {
                    SubmittedBy = model.Email,
                    Data = model.Fields
                }
            };
            await _workflowSchema.ExecuteAsync(context);

            return Ok(new { submission.Id, Status = "Processed" });
        }
    }
}
