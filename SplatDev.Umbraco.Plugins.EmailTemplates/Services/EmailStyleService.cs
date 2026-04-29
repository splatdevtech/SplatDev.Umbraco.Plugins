using NPoco;
using Umbraco.Cms.Infrastructure.Scoping;
using SplatDev.Umbraco.Plugins.EmailTemplates.Models;

namespace SplatDev.Umbraco.Plugins.EmailTemplates.Services;

public class EmailStyleService(IScopeProvider scopeProvider) : IEmailStyleService
{
    public EmailStyle Get()
    {
        using var scope = scopeProvider.CreateScope(autoComplete: true);
        return scope.Database.SingleOrDefault<EmailStyle>(
            new Sql($"SELECT * FROM [{EmailStyle.TableName}] WHERE [id]=1"))
            ?? new EmailStyle();
    }

    public EmailStyle Save(EmailStyle style)
    {
        style.Id = 1;
        style.UpdatedAt = DateTime.UtcNow;

        using var scope = scopeProvider.CreateScope();
        var existing = scope.Database.SingleOrDefault<EmailStyle>(
            new Sql($"SELECT * FROM [{EmailStyle.TableName}] WHERE [id]=1"));

        if (existing is null)
            scope.Database.Insert(style);
        else
            scope.Database.Update(style);

        scope.Complete();
        return style;
    }
}
