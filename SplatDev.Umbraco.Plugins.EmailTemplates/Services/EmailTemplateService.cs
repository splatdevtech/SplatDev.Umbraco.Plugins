using System.Text.RegularExpressions;
using NPoco;
using Umbraco.Cms.Infrastructure.Scoping;
using SplatDev.Umbraco.Plugins.EmailTemplates.Models;

namespace SplatDev.Umbraco.Plugins.EmailTemplates.Services;

public partial class EmailTemplateService(IScopeProvider scopeProvider) : IEmailTemplateService
{
    [GeneratedRegex(@"\{\{(\w+)\}\}", RegexOptions.Compiled)]
    private static partial Regex VariablePattern();

    public IReadOnlyList<EmailTemplate> GetAll()
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        return scope.Database.Fetch<EmailTemplate>(
            new Sql($"SELECT * FROM [{EmailTemplate.TableName}] ORDER BY [name]"));
    }

    public EmailTemplate? GetById(int id)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        return scope.Database.SingleOrDefault<EmailTemplate>(
            new Sql($"SELECT * FROM [{EmailTemplate.TableName}] WHERE [id]=@0", id));
    }

    public EmailTemplate? GetByName(string name)
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        return scope.Database.SingleOrDefault<EmailTemplate>(
            new Sql($"SELECT * FROM [{EmailTemplate.TableName}] WHERE [name]=@0", name));
    }

    public EmailTemplate Create(EmailTemplate template)
    {
        template.Id = 0;
        template.CreatedAt = DateTime.UtcNow;
        template.UpdatedAt = null;

        using var scope = scopeProvider.CreateScope();
        var id = Convert.ToInt32(scope.Database.Insert(template));
        template.Id = id;
        scope.Complete();
        return template;
    }

    public EmailTemplate? Update(int id, EmailTemplate template)
    {
        using var scope = scopeProvider.CreateScope();
        var existing = scope.Database.SingleOrDefault<EmailTemplate>(
            new Sql($"SELECT * FROM [{EmailTemplate.TableName}] WHERE [id]=@0", id));

        if (existing is null)
            return null;

        existing.Name = template.Name;
        existing.Subject = template.Subject;
        existing.HtmlBody = template.HtmlBody;
        existing.TextBody = template.TextBody;
        existing.Variables = template.Variables;
        existing.Category = template.Category;
        existing.UpdatedAt = DateTime.UtcNow;

        scope.Database.Update(existing);
        scope.Complete();
        return existing;
    }

    public bool Delete(int id)
    {
        using var scope = scopeProvider.CreateScope();
        var rows = scope.Database.Execute(
            new Sql($"DELETE FROM [{EmailTemplate.TableName}] WHERE [id]=@0", id));
        scope.Complete();
        return rows > 0;
    }

    public string RenderPreview(EmailTemplate template, EmailStyle? style, Dictionary<string, string>? variables = null)
    {
        var body = ApplyVariables(template.HtmlBody, variables);
        var header = style?.HeaderHtml is not null ? ApplyVariables(style.HeaderHtml, variables) : string.Empty;
        var footer = style?.FooterHtml is not null ? ApplyVariables(style.FooterHtml, variables) : string.Empty;
        var css = style?.GlobalCss ?? string.Empty;
        var subject = ApplyVariables(template.Subject, variables);
        var logoUrl = style?.LogoUrl ?? string.Empty;
        var primaryColor = style?.PrimaryColor ?? "#333333";
        var companyName = style?.CompanyName ?? string.Empty;

        return $"""
            <!DOCTYPE html>
            <html lang="en">
            <head>
              <meta charset="utf-8" />
              <meta name="viewport" content="width=device-width, initial-scale=1" />
              <title>{subject}</title>
              {(string.IsNullOrWhiteSpace(css) ? string.Empty : $"<style>{css}</style>")}
            </head>
            <body style="font-family:sans-serif;color:{primaryColor};margin:0;padding:0;">
              {(string.IsNullOrWhiteSpace(logoUrl) ? string.Empty : $"<div style=\"text-align:center;padding:16px\"><img src=\"{logoUrl}\" alt=\"{companyName}\" style=\"max-height:60px\" /></div>")}
              {header}
              <div style="padding:24px">{body}</div>
              {footer}
            </body>
            </html>
            """;
    }

    public IEnumerable<string> ExtractVariables(EmailTemplate template)
    {
        var combined = $"{template.Subject}{template.HtmlBody}{template.TextBody}";
        return VariablePattern()
            .Matches(combined)
            .Select(m => m.Groups[1].Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(v => v);
    }

    public string ApplyVariables(string content, Dictionary<string, string>? variables)
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
