﻿using System.Collections.Concurrent;
using System.Text;

namespace TinyUrl
{
    /// <summary>
    /// because the code used in short url has to be truncated to 8 characters, info is lost and no way to reconstruct the original url w/o storing state
    /// in-memory sequential encoding approach
    /// </summary>
    public class UrlConverter
    {
        private ConcurrentDictionary<string, string> _UrlToCode = new();
        private ConcurrentDictionary<string, string> _CodeToUrl = new();
        private object _lockObject= 0;
        private int _counter = 0;

        public string Encode(Uri url)
        {
            var urlString = url.ToString();
            if (this._UrlToCode.ContainsKey(urlString))
            {
                return this._UrlToCode[urlString];
            }

            byte[] urlBytes;

            lock (this._lockObject)
            {
                this._counter++;
                urlBytes = System.Text.Encoding.UTF8.GetBytes(this._counter.ToString());
            }
            
            var urlBase64String = Convert.ToBase64String(urlBytes);
            if (urlBase64String.Length > 8)
            {
                urlBase64String = urlBase64String[..8];
            }

            this._CodeToUrl[urlBase64String] = urlString;
            this._UrlToCode[urlString] = urlBase64String;
            return urlBase64String;
        }

        public Uri? Decode(string code)
        {
            return this._CodeToUrl.ContainsKey(code) ? new Uri(this._CodeToUrl[code]) : null;
        }
    }
}
