using FundingAccount.API.Domain.Repositories;

namespace FundingAccount.API.Infrastructure.UnitofWork
{

    internal sealed class FundingAccountUnitOfWork : IFundingAccountUnitofWork
    {
        private readonly RepositoryDBContext _dbContext;

        public FundingAccountUnitOfWork(RepositoryDBContext dbContext) => _dbContext = dbContext;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _dbContext.SaveChangesAsync(cancellationToken);
    }
}
