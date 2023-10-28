using UserProfile.API.Domain.Repositories;

namespace UserProfile.API.Infrastructure.UnitofWork
{
    
    internal sealed class UserProfileUnitOfWork : IUserProfileUnitofWork
    {
        private readonly RepositoryDBContext _dbContext;

        public UserProfileUnitOfWork(RepositoryDBContext dbContext) => _dbContext = dbContext;

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            _dbContext.SaveChangesAsync(cancellationToken);
    }
}
