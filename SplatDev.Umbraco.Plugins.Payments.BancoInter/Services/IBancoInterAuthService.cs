namespace SplatDev.Umbraco.Plugins.Payments.BancoInter.Services;

public interface IBancoInterAuthService
{
    /// <summary>
    /// Returns a valid Bearer token for the requested scopes, refreshing from Inter's OAuth endpoint when expired.
    /// Token is cached in-process for its full lifetime minus a 30-second safety margin.
    /// </summary>
    Task<string> GetAccessTokenAsync(string[] scopes, CancellationToken ct = default);
}
