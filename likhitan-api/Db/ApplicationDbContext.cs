using likhitan.Entities;
using Microsoft.EntityFrameworkCore;

namespace likhitan.Db
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) {  }


        public DbSet<User> Users { get; set; }
        public DbSet<UserRoles> UserRoles { get; set; }
        public DbSet<UserTracking> UserTracking { get; set; }
        public DbSet<Author> Author { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<BlogComments> BlogComments { get; set; }
        public DbSet<BlogLikes> BlogLikes { get; set; }
        public DbSet<Blogs> Blogs { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<BlogViews> BlogViews { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Role)
                .WithMany(u => u.Users)
                .HasForeignKey(u => u.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserRoles>().HasData(
                new UserRoles { Id = 1, Name = "Standard", IsActive = true },
                new UserRoles { Id = 2, Name = "Author", IsActive = true },
                new UserRoles { Id = 3, Name = "Admin", IsActive = true }
            );
        }
    }
}
