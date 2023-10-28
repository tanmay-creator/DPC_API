using UserProfile.API.Infrastructure.Model;


namespace UserProfile.API.Domain.Repositories
{
    public interface IUserProfileRepository
    {
        Task<UserProfile_DBModel> GetUserProfile(Guid userprofileId, CancellationToken cancellationToken = default);
    }
}
