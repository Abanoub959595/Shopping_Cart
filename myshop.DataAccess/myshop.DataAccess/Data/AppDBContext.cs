
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using myshop.Entities.Models;

namespace myshop.DataAccess.Data
{
    public class AppDBContext : IdentityDbContext<ApplicationUser>
    {
        public AppDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderHeader> OrderHeaders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        // public DbSet<ApplicationUser> ApplicationUsers { get; set; }


        // to know the program that the ApplicationUsers is only extend the IdentityUser and not a new table 
        // public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        // public class AppDBContext : IdentityDbContext<IdentityUser>
    }
}
