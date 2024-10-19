using Microsoft.EntityFrameworkCore;

namespace TinyUrl.Repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        public DbSet<UrlMappings> Urls { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UrlMappings>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
                entity.Property(e => e.Url).IsRequired().HasMaxLength(300);
                entity.Property(e => e.Code).IsRequired(false).HasMaxLength(8);
                entity.HasIndex(e => e.Code).IsUnique();
            });
        }
    }
}
