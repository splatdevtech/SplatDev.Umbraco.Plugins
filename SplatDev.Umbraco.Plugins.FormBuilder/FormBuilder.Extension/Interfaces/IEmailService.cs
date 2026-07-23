namespace FormBuilder.Extension.Interfaces
{
    /// <summary>
    /// Interface for email service to handle email sending functionality.
    /// </summary>
    public interface IEmailService
    {
        /// <summary>
        /// Sends an email with the specified parameters.
        /// </summary>
        /// <param name="to">Recipient email address.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="body">Body content of the email.</param>
        /// <param name="from">Sender email address (optional).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendAsync(string to, string subject, string body, string? from = null);

        /// <summary>
        /// Sends an email to multiple recipients.
        /// </summary>
        /// <param name="recipients">Array of recipient email addresses.</param>
        /// <param name="subject">Subject of the email.</param>
        /// <param name="body">Body content of the email.</param>
        /// <param name="from">Sender email address (optional).</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task SendToMultipleAsync(string[] recipients, string subject, string body, string? from = null);
    }
}
