using Microsoft.Extensions.Options;
using SplatDev.Umbraco.Plugins.ENotAssina.Models;

namespace SplatDev.Umbraco.Plugins.ENotAssina.Services;

/// <summary>
/// Injects the e-Not Assina <c>Authorization</c> header on every outbound request.
/// Format: <c>EmpresaXXX|{access_key}</c> — the company ID is normalised to always
/// include the "Empresa" prefix regardless of whether the config value includes it.
/// </summary>
public class ENotAssinaAuthHandler(IOptions<ENotAssinaOptions> options) : DelegatingHandler
{
    private readonly ENotAssinaOptions _opts = options.Value;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(_opts.CompanyId) && !string.IsNullOrWhiteSpace(_opts.AccessKey))
        {
            var companyId = _opts.CompanyId.StartsWith("Empresa", StringComparison.OrdinalIgnoreCase)
                ? _opts.CompanyId
                : $"Empresa{_opts.CompanyId}";

            request.Headers.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue(
                    $"{companyId}|{_opts.AccessKey}");
        }

        return base.SendAsync(request, cancellationToken);
    }
}
