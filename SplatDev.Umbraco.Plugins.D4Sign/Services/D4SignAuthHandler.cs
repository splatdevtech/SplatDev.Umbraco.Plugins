using Microsoft.Extensions.Options;
using SplatDev.Umbraco.Plugins.D4Sign.Models;

namespace SplatDev.Umbraco.Plugins.D4Sign.Services;

/// <summary>
/// Appends D4Sign's <c>tokenAPI</c> and <c>cryptKey</c> as query-string parameters
/// on every outbound request. D4Sign requires these credentials on all API calls.
/// </summary>
public class D4SignAuthHandler(IOptions<D4SignOptions> options) : DelegatingHandler
{
    private readonly D4SignOptions _opts = options.Value;

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        if (request.RequestUri is not null &&
            !string.IsNullOrWhiteSpace(_opts.TokenApi) &&
            !string.IsNullOrWhiteSpace(_opts.CryptKey))
        {
            var sep       = string.IsNullOrEmpty(request.RequestUri.Query) ? "?" : "&";
            var authQuery = $"{sep}tokenAPI={Uri.EscapeDataString(_opts.TokenApi)}" +
                            $"&cryptKey={Uri.EscapeDataString(_opts.CryptKey)}";
            request.RequestUri = new Uri(request.RequestUri.AbsoluteUri + authQuery);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
