namespace UmbracoCms.Tools.PackageActions;

public interface IPackageAction
{
    string Name { get; }
    Task ExecuteAsync(CancellationToken cancellationToken = default);
}
