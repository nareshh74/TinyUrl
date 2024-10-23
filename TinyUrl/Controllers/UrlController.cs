using MessagePack;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using TinyUrl.Controllers.Models;
using TinyUrl.Logic;
using TinyUrl.Repository;

namespace TinyUrl.Controllers
{
    [Route("api/v1")]
    public class UrlController : ControllerBase
    {
        private readonly IUrlConverter _urlConverter;
        private readonly IUrlRepository _urlRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDistributedCache _cache;
        private readonly ILogger<UrlController> _logger;

        public UrlController(IUrlConverter urlConverter,
            IUrlRepository urlRepository,
            IUnitOfWork unitOfWork,
            IDistributedCache cache,
            ILogger<UrlController> logger)
        {
            this._urlConverter = urlConverter;
            this._urlRepository = urlRepository;
            this._unitOfWork = unitOfWork;
            this._cache = cache;
            this._logger = logger;
        }

        [HttpPost("shorten")]
        public async Task<IActionResult> Shorten([FromBody] LongUrlRequest request, CancellationToken cancellationToken)
        {
            try
            {
                await this._unitOfWork.BeginTransactionAsync(cancellationToken);
                int id = await this._urlRepository.GenerateIdAsync(request.LongUrl, cancellationToken);
                string code = this._urlConverter.Encode(request.LongUrl, id);
                await this._urlRepository.TryUpdateCodeAsync(id, code, cancellationToken);
                await this._unitOfWork.CommitAsync(cancellationToken);
                this.WriteToCache(request, cancellationToken, code);
                return this.Created("", new ShortUrlResponse { ShortUrl = code });
            }
            catch (Exception)
            {
                await this._unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }

        private void WriteToCache(LongUrlRequest request, CancellationToken cancellationToken, string code)
        {
            Task.Run
            (
                async () =>
                {
                    var data = MessagePackSerializer.Serialize(request.LongUrl);
                    await this._cache.SetAsync(code, data, new DistributedCacheEntryOptions(), cancellationToken);
                }
                , cancellationToken
            );
        }

        [HttpGet("{shortUrlHash}")]
        public async Task<IActionResult> RedirectToLongUrl(string shortUrlHash, CancellationToken cancellationToken)
        {
            var data = await this._cache.GetAsync(shortUrlHash, cancellationToken);
            if (data?.Length > 0)
            {
                using var stream = new MemoryStream(data);
                var cachedUrl = await MessagePackSerializer.DeserializeAsync<Uri>(stream, options:null, cancellationToken);
                this._logger.LogInformation("Cache hit for {shortUrlHash}", shortUrlHash);
                return this.Redirect(cachedUrl.ToString());
            }
            this._logger.LogInformation("Cache miss for {shortUrlHash}", shortUrlHash);
            var url = await this._urlRepository.TryGetUrlByCodeAsync(shortUrlHash, cancellationToken);
            var urlString = url?.ToString();
            if (string.IsNullOrEmpty(urlString))
            {
                return this.NotFound();
            }
            this.WriteToCache(new LongUrlRequest { LongUrl = url! }, cancellationToken, shortUrlHash);
            return this.Redirect(urlString);
        }
    }
}
