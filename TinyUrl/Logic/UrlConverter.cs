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
        private readonly object _lockObject = 0;
        private int _counter = 0;

        public string Encode(Uri url)
        {
            byte[] urlBytes;

            lock (this._lockObject)
            {
                this._counter++;
                urlBytes = Encoding.UTF8.GetBytes(this._counter.ToString());
            }

            var urlBase64String = Convert.ToBase64String(urlBytes);
            if (urlBase64String.Length > 8)
            {
                urlBase64String = urlBase64String[..8];
            }

            return urlBase64String;
        }
    }
}
