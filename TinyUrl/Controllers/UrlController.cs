using Microsoft.AspNetCore.Mvc;
using TinyUrl.Controllers.Models;

namespace TinyUrl.Controllers
{
    [Route("api/v1")]
    public class UrlController : ControllerBase
    {
        private readonly UrlConverter _urlConverter;

        public UrlController(UrlConverter urlConverter)
        {
            this._urlConverter = urlConverter;
        }

        [HttpPost("shorten")]
        public IActionResult Shorten([FromBody] LongUrlRequest request)
        {
            return this.Created("", new ShortUrlResponse { ShortUrl = this._urlConverter.Encode(request.LongUrl) });
        }

        [HttpGet("{shortUrlHash}")]
        public IActionResult RedirectToLongUrl(string shortUrlHash)
        {
            return this.Redirect(this._urlConverter.Decode(shortUrlHash).ToString());
        }
    }
}
