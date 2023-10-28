namespace UserProfile.API.Domain.Repositories
{
    public interface IUserProfileUnitofWork
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
