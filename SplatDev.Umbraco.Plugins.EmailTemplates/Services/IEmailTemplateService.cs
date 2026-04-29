using SplatDev.Umbraco.Plugins.EmailTemplates.Models;

namespace SplatDev.Umbraco.Plugins.EmailTemplates.Services;

public interface IEmailTemplateService
{
    IReadOnlyList<EmailTemplate> GetAll();
    EmailTemplate? GetById(int id);
    EmailTemplate? GetByName(string name);
    EmailTemplate Create(EmailTemplate template);
    EmailTemplate? Update(int id, EmailTemplate template);
    bool Delete(int id);

    /// <summary>Renders a full HTML preview document with variables substituted.</summary>
    string RenderPreview(EmailTemplate template, EmailStyle? style, Dictionary<string, string>? variables = null);

    /// <summary>Returns distinct variable names found in subject + htmlBody + textBody.</summary>
    IEnumerable<string> ExtractVariables(EmailTemplate template);

    /// <summary>Substitutes {{VariableName}} placeholders in the given text.</summary>
    string ApplyVariables(string content, Dictionary<string, string>? variables);
}
