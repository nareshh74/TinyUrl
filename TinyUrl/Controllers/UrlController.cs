using Microsoft.AspNetCore.Mvc;
using TinyUrl.Controllers.Models;

namespace TinyUrl.Controllers
{
    [Route("api/v1")]
    public class UrlController : ControllerBase
    {
        [HttpPost("shorten")]
        public IActionResult Shorten([FromBody] LongUrlRequest request)
        {
            return this.Created("", new ShortUrlResponse { ShortUrl = "abc" });
        }

        [HttpGet("{shortUrlHash}")]
        public IActionResult RedirectToLongUrl(string shortUrlHash)
        {
            return this.Redirect("https://www.google.com");
        }
    }
}
