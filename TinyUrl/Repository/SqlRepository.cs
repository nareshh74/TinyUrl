using Microsoft.EntityFrameworkCore;

namespace TinyUrl.Repository
{
    public class SqlRepository : IUrlRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<SqlRepository> _logger;

        public SqlRepository(ApplicationDbContext dbContext, ILogger<SqlRepository> logger)
        {
            this._dbContext = dbContext;
            this._logger = logger;
        }

        public async Task<int> GenerateIdAsync(Uri url, CancellationToken cancellationToken)
        {

            var urlMappings = new UrlMappings { Url = url.ToString() };
            this._dbContext.Urls.Add(urlMappings);
            await this._dbContext.SaveChangesAsync(cancellationToken);
            this._logger.LogInformation("Generated id {id} for {url}", urlMappings.Id, url);
            return urlMappings.Id;
        }

        public async Task<bool> TryUpdateCodeAsync(int id, string code, CancellationToken cancellationToken)
        {
            var urlMappings = await this._dbContext.Urls.FindAsync(new object[] { id }, cancellationToken);
            if (urlMappings == null)
            {
                return false;
            }

            urlMappings.Code = code;
            await this._dbContext.SaveChangesAsync(cancellationToken);
            this._logger.LogInformation("Updated code {code} for id {id}", code, id);
            return true;
        }

        public async Task<Uri?> TryGetUrlByCodeAsync(string code, CancellationToken cancellationToken)
        {
            var urlMappings = await this._dbContext.Urls.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
            this._logger.LogInformation("Retrieved url {url} for code {code}", urlMappings?.Url, code);
            return urlMappings == null ? null : new Uri(urlMappings.Url!);
        }
    }
}
