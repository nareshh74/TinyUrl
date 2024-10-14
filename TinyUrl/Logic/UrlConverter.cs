using System.Collections.Concurrent;
using System.Text;

namespace TinyUrl.Logic
{
    /// <summary>
    /// because the code used in short url has to be truncated to 8 characters, info is lost and no way to reconstruct the original url w/o storing state
    /// in-memory sequential encoding approach
    /// </summary>
    public class UrlConverter : IUrlConverter
    {
        private ConcurrentDictionary<string, string> _UrlToCode = new();
        private ConcurrentDictionary<string, string> _CodeToUrl = new();
        private object _lockObject = 0;
        private int _counter = 0;

        public string Encode(Uri url)
        {
            var urlString = url.ToString();
            if (_UrlToCode.ContainsKey(urlString))
            {
                return _UrlToCode[urlString];
            }

            byte[] urlBytes;

            lock (_lockObject)
            {
                _counter++;
                urlBytes = Encoding.UTF8.GetBytes(_counter.ToString());
            }

            var urlBase64String = Convert.ToBase64String(urlBytes);
            if (urlBase64String.Length > 8)
            {
                urlBase64String = urlBase64String[..8];
            }

            _CodeToUrl[urlBase64String] = urlString;
            _UrlToCode[urlString] = urlBase64String;
            return urlBase64String;
        }

        public Uri? Decode(string code)
        {
            return _CodeToUrl.ContainsKey(code) ? new Uri(_CodeToUrl[code]) : null;
        }
    }
}
