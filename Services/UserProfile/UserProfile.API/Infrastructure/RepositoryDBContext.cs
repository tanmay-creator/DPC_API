using Microsoft.EntityFrameworkCore;
using UserProfile.API.Infrastructure.Model;

namespace UserProfile.API.Infrastructure
{

    public sealed class RepositoryDBContext : DbContext
    {
        public RepositoryDBContext(DbContextOptions options)
        : base(options)
        {
        }
        public DbSet<UserProfile_DBModel> Owners { get; set; }

       
        protected override void OnModelCreating(ModelBuilder modelBuilder) =>
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(RepositoryDBContext).Assembly);
    }
}
