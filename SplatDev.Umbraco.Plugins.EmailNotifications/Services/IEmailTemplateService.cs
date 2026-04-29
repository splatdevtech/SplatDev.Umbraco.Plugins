using SplatDev.Umbraco.Plugins.EmailNotifications.Models;

namespace SplatDev.Umbraco.Plugins.EmailNotifications.Services;

public interface IEmailTemplateService
{
    Task<List<EmailTemplate>> GetAllAsync(CancellationToken ct = default);
    Task<EmailTemplate?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<EmailTemplate> CreateAsync(EmailTemplate template, CancellationToken ct = default);
    Task<EmailTemplate?> UpdateAsync(int id, EmailTemplate template, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    /// <summary>Renders a template with the given variable substitutions and returns the full HTML.</summary>
    string RenderPreview(EmailTemplate template, Dictionary<string, string>? variables = null);

    /// <summary>Returns the set of variable placeholders found in the template body, e.g. {{MemberName}}.</summary>
    IEnumerable<string> ExtractVariables(EmailTemplate template);
}
