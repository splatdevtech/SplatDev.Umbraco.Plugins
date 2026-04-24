using System.Reflection;
using Microsoft.Extensions.Logging;

namespace SplatDev.Umbraco.Tools.PackageActions;

public class PackageActionRunner
{
    private readonly IServiceProvider _services;
    private readonly ILogger<PackageActionRunner> _logger;

    public PackageActionRunner(IServiceProvider services, ILogger<PackageActionRunner> logger)
    {
        _services = services;
        _logger = logger;
    }

    public async Task RunAllAsync(Assembly assembly, CancellationToken cancellationToken = default)
    {
        var actionTypes = assembly.GetTypes()
            .Where(t => typeof(IPackageAction).IsAssignableFrom(t) && !t.IsAbstract && !t.IsInterface);

        foreach (var type in actionTypes)
        {
            if (_services.GetService(type) is IPackageAction action)
            {
                _logger.LogInformation("Running package action: {Name}", action.Name);
                await action.ExecuteAsync(cancellationToken);
            }
        }
    }
}
