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
        public string Encode(Uri url, int seed)
        {
            byte[] urlBytes = Encoding.UTF8.GetBytes(seed.ToString());

            var urlBase64String = Convert.ToBase64String(urlBytes);
            if (urlBase64String.Length > 8)
            {
                urlBase64String = urlBase64String[..8];
            }

            return urlBase64String;
        }
    }
}
