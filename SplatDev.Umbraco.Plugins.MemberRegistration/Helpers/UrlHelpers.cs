using System;
using System.Security.Cryptography;
using Umbraco.Cms.Core.PublishedCache;
using Umbraco.Extensions;

namespace SplatDev.Umbraco.Plugins.MemberRegistration.Helpers
{
    public class UrlHelpers
    {
        private readonly IPublishedContentCache _contentCache;
        private const int Seed = 30;

        public UrlHelpers(IPublishedContentCache contentCache)
        {
            _contentCache = contentCache;
        }

        public string GetConfirmVerificationUrl(
            int nodeId,
            string documentTypeAlias,
            string origin,
            string email,
            string languageCode,
            string id = "",
            string code = "")
        {
            code = string.IsNullOrEmpty(code) ? RandomString(Seed) : code;

            var node = _contentCache.GetById(nodeId);
            var confirmPage = node?.Children?.FirstOrDefault(x => x.ContentType.Alias == documentTypeAlias);
            var pagePath = confirmPage?.Url(languageCode) ?? "/confirm-email";

            var query = "?";
            query += !string.IsNullOrEmpty(id) ? $"a={Uri.EscapeDataString(id)}&" : string.Empty;
            query += $"b={Uri.EscapeDataString(code)}&c={Uri.EscapeDataString(email)}";

            var verificationUrl = new Uri(new Uri(origin), $"{pagePath}{query}");
            return verificationUrl.ToString();
        }

        public string GetResetPasswordVerificationUrl(
            int nodeId,
            string documentTypeAlias,
            string origin,
            string username,
            string languageCode,
            string code = "")
        {
            code = string.IsNullOrEmpty(code) ? RandomString(Seed) : code;

            var node = _contentCache.GetById(nodeId);
            var page = node?.Children?.FirstOrDefault(x => x.ContentType.Alias == documentTypeAlias);
            var pageUrl = page?.Url(languageCode) ?? "/reset-password";

            var verificationUrl = new Uri(new Uri(origin), $"{pageUrl}?b={Uri.EscapeDataString(code)}&c={Uri.EscapeDataString(username)}");
            return verificationUrl.ToString();
        }

        private static string RandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var result = new char[length];
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[length];
            rng.GetBytes(bytes);
            for (int i = 0; i < length; i++)
                result[i] = chars[bytes[i] % chars.Length];
            return new string(result);
        }
    }
}
