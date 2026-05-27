using FormBuilder.Extension.Enum;
using FormBuilder.Extension.Interfaces;

namespace FormBuilder.Extension.Workflows
{
    public class SendNotificationWorkflow(IEmailService emailService) : WorkflowSchema("Send Email", "Email", "Sends an email notification upon form submission."), IWorkflow
    {
        private readonly IEmailService _emailService = emailService;

        public int Id { get; set; }
        public override Guid Guid => new("dd6bef78-141c-465e-a526-f8dff3f99e52");
        public static WorkflowType Type => WorkflowType.Email;

        public override async Task<WorkflowExecutionStatus> ExecuteAsync(WorkflowExecutionContext context)
        {
            try
            {
                var submission = context.Record;
                var emailBody = $"Form '{submission?.FormName}' was submitted by {submission?.SubmittedBy}. Data: {submission?.Data}";

                await _emailService.SendAsync("admin@example.com", "New Form Submission", emailBody);

                return WorkflowExecutionStatus.Completed;
            }
            catch
            {
                return WorkflowExecutionStatus.Failed;
            }
        }
    }
}
