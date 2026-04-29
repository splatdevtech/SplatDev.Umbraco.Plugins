using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using SplatDev.Umbraco.Plugins.EmailNotifications.Models;

namespace SplatDev.Umbraco.Plugins.EmailNotifications.Services;

public partial class EmailTemplateService(EmailNotificationsDbContext db) : IEmailTemplateService
{
    [GeneratedRegex(@"\{\{(\w+)\}\}", RegexOptions.Compiled)]
    private static partial Regex VariablePattern();

    public Task<List<EmailTemplate>> GetAllAsync(CancellationToken ct = default) =>
        db.EmailTemplates.OrderBy(t => t.Name).ToListAsync(ct);

    public Task<EmailTemplate?> GetByIdAsync(int id, CancellationToken ct = default) =>
        db.EmailTemplates.FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<EmailTemplate> CreateAsync(EmailTemplate template, CancellationToken ct = default)
    {
        template.Id = 0;
        template.CreatedAt = DateTime.UtcNow;
        template.UpdatedAt = null;
        db.EmailTemplates.Add(template);
        await db.SaveChangesAsync(ct);
        return template;
    }

    public async Task<EmailTemplate?> UpdateAsync(int id, EmailTemplate template, CancellationToken ct = default)
    {
        var existing = await db.EmailTemplates.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (existing is null)
            return null;

        existing.Name = template.Name;
        existing.Subject = template.Subject;
        existing.HeaderHtml = template.HeaderHtml;
        existing.BodyHtml = template.BodyHtml;
        existing.FooterHtml = template.FooterHtml;
        existing.GlobalStyles = template.GlobalStyles;
        existing.UpdatedAt = DateTime.UtcNow;

        await db.SaveChangesAsync(ct);
        return existing;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var template = await db.EmailTemplates.FirstOrDefaultAsync(t => t.Id == id, ct);
        if (template is null)
            return false;

        db.EmailTemplates.Remove(template);
        await db.SaveChangesAsync(ct);
        return true;
    }

    public string RenderPreview(EmailTemplate template, Dictionary<string, string>? variables = null)
    {
        var body = ApplyVariables(template.BodyHtml, variables);
        var header = template.HeaderHtml is not null ? ApplyVariables(template.HeaderHtml, variables) : string.Empty;
        var footer = template.FooterHtml is not null ? ApplyVariables(template.FooterHtml, variables) : string.Empty;
        var styles = template.GlobalStyles ?? string.Empty;

        return $"""
            <!DOCTYPE html>
            <html>
            <head>
              <meta charset="utf-8" />
              <meta name="viewport" content="width=device-width, initial-scale=1" />
              <title>{template.Subject}</title>
              {(string.IsNullOrWhiteSpace(styles) ? string.Empty : $"<style>{styles}</style>")}
            </head>
            <body>
              {header}
              {body}
              {footer}
            </body>
            </html>
            """;
    }

    public IEnumerable<string> ExtractVariables(EmailTemplate template)
    {
        var combined = $"{template.HeaderHtml}{template.BodyHtml}{template.FooterHtml}";
        return VariablePattern()
            .Matches(combined)
            .Select(m => m.Groups[1].Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(v => v);
    }

    private static string ApplyVariables(string content, Dictionary<string, string>? variables)
    {
        if (variables is null or { Count: 0 })
            return content;

        return VariablePattern().Replace(content, match =>
        {
            var key = match.Groups[1].Value;
            return variables.TryGetValue(key, out var value) ? value : match.Value;
        });
    }
}
