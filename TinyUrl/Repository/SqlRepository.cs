using Microsoft.EntityFrameworkCore;

namespace TinyUrl.Repository
{
    public class SqlRepository : IUrlRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public SqlRepository(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<int> GenerateIdAsync(Uri url, CancellationToken cancellationToken)
        {
            var urlMappings = new UrlMappings { Url = url.ToString() };
            this._dbContext.Urls.Add(urlMappings);
            await this._dbContext.SaveChangesAsync(cancellationToken);
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
            return true;
        }

        public async Task<Uri?> TryGetUrlByCodeAsync(string code, CancellationToken cancellationToken)
        {
            var urlMappings = await this._dbContext.Urls.FirstOrDefaultAsync(x => x.Code == code, cancellationToken);
            return urlMappings == null ? null : new Uri(urlMappings.Url!);
        }
    }
}
