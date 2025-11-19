using Microsoft.EntityFrameworkCore;
using MyMVCProject.Models;
using MyUserProject.Models;
using MyUserProject.ViewModels;

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
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<SelectedSize> SelectedSizes { get; set; }
        public DbSet<OtpVerification> OtpVerifications { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<ProductReview> ProductReviews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<UserAddress>()
        .HasKey(a => a.AddressId); // ✅ Explicitly tell EF the key
                                   // Ensure EF doesn’t try to map LoginViewModel
                                   //  modelBuilder.Entity<LoginViewModel>().HasNoKey();

       //     modelBuilder.Entity<OrderItem>()
       //.HasOne(oi => oi.Products)
       //.WithMany()
       //.HasForeignKey(oi => oi.ProductId);

       //     base.OnModelCreating(modelBuilder);
        }
}
}
