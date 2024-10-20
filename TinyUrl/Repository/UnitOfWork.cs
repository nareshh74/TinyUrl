namespace TinyUrl.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _dbContext;
        public UnitOfWork(ApplicationDbContext dbContext)
        {
            this._dbContext = dbContext;
        }
        public async Task BeginTransactionAsync(CancellationToken cancellationToken)
        {
            await this._dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitAsync(CancellationToken cancellationToken)
        {
            await this._dbContext.Database.CommitTransactionAsync(cancellationToken);
        }

        public async Task RollbackAsync(CancellationToken cancellationToken)
        {
            await this._dbContext.Database.RollbackTransactionAsync(cancellationToken);
        }
    }
}
