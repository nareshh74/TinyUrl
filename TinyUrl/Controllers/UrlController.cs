using Microsoft.AspNetCore.Mvc;
using TinyUrl.Controllers.Models;
using TinyUrl.Logic;

namespace TinyUrl.Controllers
{
    [Route("api/v1")]
    public class UrlController : ControllerBase
    {
        private readonly IUrlConverter _urlConverter;
        private readonly IUrlRepository _urlRepository;

        public UrlController(IUrlConverter urlConverter, IUrlRepository urlRepository)
        {
            this._urlConverter = urlConverter;
            this._urlRepository = urlRepository;
        }

        [HttpPost("shorten")]
        public async Task<IActionResult> Shorten([FromBody] LongUrlRequest request, CancellationToken cancellationToken)
        {
            string code;
            if(await this._urlRepository.TryGetCodeAsync(request.LongUrl, out var cachedCode, cancellationToken))
            {
                code = cachedCode!;
            }
            else
            {
                code = this._urlConverter.Encode(request.LongUrl);
                await this._urlRepository.TryAddUrlCodeMappingAsync(request.LongUrl, code, cancellationToken);
            }
            return this.Created("", new ShortUrlResponse { ShortUrl = code });
        }

        [HttpGet("{shortUrlHash}")]
        public async Task<IActionResult> RedirectToLongUrl(string shortUrlHash, CancellationToken cancellationToken)
        {
            if (await this._urlRepository.TryGetUrlAsync(shortUrlHash, out var url, cancellationToken))
            {
                return this.Redirect(url.ToString());
            }
            return this.NotFound();
        }
    }
}
