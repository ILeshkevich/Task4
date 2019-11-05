using Microsoft.EntityFrameworkCore;

namespace GitApp.Models.Db
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<File> Files { get; set; }

        public DbSet<Repository> Repositories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<File>()
                .HasOne(f => f.Repository)
                .WithMany(r => r.Files)
                .HasForeignKey(f => f.RepositoryId)
                .HasPrincipalKey(r => r.Id);
        }
    }
}