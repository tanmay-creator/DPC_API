using UserProfile.API.Domain.Repositories;
using UserProfile.API.Infrastructure.Model;

namespace UserProfile.API.Infrastructure.Repositories
{
    internal sealed class UserProfileRepository : IUserProfileRepository
    {
        private readonly RepositoryDBContext _dbContext;

        public UserProfileRepository(RepositoryDBContext dbContext) => _dbContext = dbContext;

       
        //public async Task<UserProfile_DBModel> GetByIdAsync(Guid userprofileId, CancellationToken cancellationToken = default) =>
        //    await _dbContext.UserProfile_DBModel.FirstOrDefaultAsync(x => x.Id == userprofileId, cancellationToken);

        public Task<UserProfile_DBModel> GetUserProfile(Guid userprofileId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
