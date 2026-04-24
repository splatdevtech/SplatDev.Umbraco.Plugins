using FormBuilder.Core.Interfaces;

namespace FormBuilder.Core.Providers.EmailTemplates
{
    /// <summary>Defines the default theme and files.</summary>
    public class DefaultEmailTemplate : IEmailTemplate
    {
        /// <inheritdoc />
        public virtual string FileName => "Example-Template.cshtml";
    }
}