using FormBuilder.Extension.Attributes;
using FormBuilder.Extension.Enum;
using FormBuilder.Extension.Extensions;
using FormBuilder.Extension.Interfaces;

namespace FormBuilder.Extension.Workflows
{
    public class EmailWorkflow(IFormRepository formRepository, IEmailService emailService) : WorkflowSchema("Email Workflow", "Communication", "Sends emails using configured templates"), IWorkflow
    {
        private readonly IFormRepository _formRepository = formRepository;
        private readonly IEmailService _emailService = emailService;

        public int Id { get; set; }
        public override Guid Guid => new("6b0fce27-c602-4564-affe-3c4057b35dc3");
        public override string Name { get; set; } = "Send Email";
        public override string Description { get; set; } = "Send an email to the author of the submission";

        [Setting("Recipients", Description = "Comma-separated list of email addresses", View = "TextField")]
        public string Recipients { get; set; } = "";

        [Setting("Subject", Description = "Email subject", View = "TextField")]
        public string Subject { get; set; } = "";

        [Setting("Body", Description = "Email body. Use {{FieldAlias}} for placeholders.", View = "TextArea")]
        public string Body { get; set; } = string.Empty;

        public override async Task<WorkflowExecutionStatus> ExecuteAsync(WorkflowExecutionContext context)
        {
            if (context.Record is null) return WorkflowExecutionStatus.Failed;

            try
            {
                var data = context.Record.Data; // Dictionary<string, object>
                var processedBody = TemplateProcessor.ReplacePlaceholders(Body, data);

                await _emailService.SendAsync(
                    to: Recipients,
                    subject: Subject,
                    body: processedBody
                );

                return WorkflowExecutionStatus.Completed;
            }
            catch
            {
                return WorkflowExecutionStatus.Failed;
            }
        }
    }
}