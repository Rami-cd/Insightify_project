using Auth_service.Models;
using Microsoft.EntityFrameworkCore;

namespace Auth_service.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Tenant> Tenants => Set<Tenant>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<User>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.HasIndex(x => x.Email)
                      .IsUnique();

                entity.Property(x => x.Email)
                      .IsRequired();

                entity.Property(x => x.PasswordHash)
                      .IsRequired();
            });

            builder.Entity<Tenant>(entity =>
            {
                entity.HasKey(x => x.Id);

                entity.Property(x => x.Name)
                      .IsRequired();

                entity.HasOne(x => x.OwnerUser)
                      .WithOne(x => x.Tenant)
                      .HasForeignKey<Tenant>(x => x.OwnerUserId);
            });
        }
    }

}
