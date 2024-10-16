using Microsoft.AspNetCore.Mvc;
using System.Collections.Concurrent;
using TinyUrl.Controllers.Models;
using TinyUrl.Logic;

namespace TinyUrl.Controllers
{
    [Route("api/v1")]
    public class UrlController : ControllerBase
    {
        private readonly IUrlConverter _urlConverter;
        private readonly ConcurrentDictionary<string, string> _urlToCode = new();
        private readonly ConcurrentDictionary<string, string> _codeToUrl = new();

        public UrlController(IUrlConverter urlConverter)
        {
            this._urlConverter = urlConverter;
        }

        [HttpPost("shorten")]
        public IActionResult Shorten([FromBody] LongUrlRequest request)
        {
            string code;
            var urlString = request.LongUrl.ToString();
            if(this._urlToCode.TryGetValue(urlString, out var cachedCode))
            {
                code = cachedCode;
            }
            else
            {
                code = this._urlConverter.Encode(request.LongUrl);
                this._urlToCode[urlString] = code;
                this._codeToUrl[code] = urlString;
            }
            return this.Created("", new ShortUrlResponse { ShortUrl = code });
        }

        [HttpGet("{shortUrlHash}")]
        public IActionResult RedirectToLongUrl(string shortUrlHash)
        {
            if (this._codeToUrl.TryGetValue(shortUrlHash, out var url))
            {
                return this.Redirect(url);
            }
            return this.NotFound();
        }
    }
}
