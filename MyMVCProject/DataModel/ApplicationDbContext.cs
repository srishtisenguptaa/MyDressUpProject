using Microsoft.EntityFrameworkCore;
using MyMVCProject.Models;

namespace MyMVCProject.DataModel
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //  Add your tables here
        //public DbSet<LoginViewModel> LoginUsers { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<WishListItem> WishlistItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Ensure EF doesn’t try to map LoginViewModel
          //  modelBuilder.Entity<LoginViewModel>().HasNoKey();
          

    }
}
}
