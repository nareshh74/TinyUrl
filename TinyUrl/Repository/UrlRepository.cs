namespace TinyUrl.Repository
{
    public sealed class UrlRepository : IUrlRepository
    {
        private readonly Dictionary<string, string> _urlToCode = new();
        private readonly Dictionary<string, string> _codeToUrl = new();

        public Task<bool> TryGetCodeAsync(Uri url, out string? code, CancellationToken _)
        {
            return Task.FromResult(_urlToCode.TryGetValue(url.ToString(), out code));
        }

        public Task<bool> TryGetUrlAsync(string code, out Uri url, CancellationToken _)
        {
            if (_codeToUrl.TryGetValue(code, out var urlString))
            {
                url = new Uri(urlString);
                return Task.FromResult(true);
            }
            url = null!;
            return Task.FromResult(false);
        }

        public Task<bool> TryAddUrlCodeMappingAsync(Uri url, string code, CancellationToken _)
        {
            _urlToCode[url.ToString()] = code;
            _codeToUrl[code] = url.ToString();
            return Task.FromResult(true);
        }
    }
}
