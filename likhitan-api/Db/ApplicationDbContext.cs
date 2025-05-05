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

            //modelBuilder.Entity<BlogCategory>().HasData(
            //    new BlogCategory { Id = 1, Name = "Lifestyle", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 2, Name = "Travel", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 3, Name = "FoodAndCooking", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 4, Name = "HealthAndWellness", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 5, Name = "Technology", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 6, Name = "FinanceMoney", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 7, Name = "BusinessAndMarketing", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 8, Name = "EducationCareer", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 9, Name = "ParentingAndFamily", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 10, Name = "Entertainment", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 11, Name = "DIYAndCrafts", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 12, Name = "FashionAndBeauty", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 13, Name = "SportsAndFitness", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 14, Name = "RelationshipsAndDating", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 15, Name = "HomeAndGardening", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 16, Name = "Automotive", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 17, Name = "Pets", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 18, Name = "PersonalAndStories", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 19, Name = "Hobbies", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 20, Name = "NewsAndTrends", IsActive = true, IsDeleted = false },
            //    new BlogCategory { Id = 20, Name = "Religious", IsActive = true, IsDeleted = false }
            //);
        }
    }
}
