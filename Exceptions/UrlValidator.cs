using System;
using System.Text.RegularExpressions;

namespace Project_LMS.Helpers
{
    public static class UrlValidator
    {
        private static readonly Regex UrlRegex = new Regex(
            @"^(https?:\/\/)?([a-zA-Z0-9.-]+)\.([a-zA-Z]{2,6})(\/\S*)?$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        public static bool IsValid(string url) =>
            !string.IsNullOrWhiteSpace(url) && Uri.TryCreate(url, UriKind.Absolute, out _) && UrlRegex.IsMatch(url);

        public static bool IsHttps(string url) =>
            Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Scheme == Uri.UriSchemeHttps;

        public static bool HasDomain(string url, string domain) =>
            Uri.TryCreate(url, UriKind.Absolute, out var uri) && uri.Host.EndsWith(domain, StringComparison.OrdinalIgnoreCase);
    }
}