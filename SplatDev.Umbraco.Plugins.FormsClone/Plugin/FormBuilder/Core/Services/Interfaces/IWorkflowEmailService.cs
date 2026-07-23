namespace FormBuilder.Core.Services.Interfaces
{
    public interface IWorkflowEmailService
    {
        Task SendEmailAsync(SendEmailArgs args);
    }
}