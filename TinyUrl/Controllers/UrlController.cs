using Microsoft.AspNetCore.Mvc;
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

        public UrlController(IUrlConverter urlConverter, IUrlRepository urlRepository, IUnitOfWork unitOfWork)
        {
            this._urlConverter = urlConverter;
            this._urlRepository = urlRepository;
            this._unitOfWork = unitOfWork;
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
                return this.Created("", new ShortUrlResponse { ShortUrl = code });
            }
            catch (Exception)
            {
                await this._unitOfWork.RollbackAsync(cancellationToken);
                throw;
            }
        }

        [HttpGet("{shortUrlHash}")]
        public async Task<IActionResult> RedirectToLongUrl(string shortUrlHash, CancellationToken cancellationToken)
        {
            var url = await this._urlRepository.TryGetUrlByCodeAsync(shortUrlHash, cancellationToken);
            if (url != null)
            {
                return this.Redirect(url.ToString());
            }
            return this.NotFound();
        }
    }
}
