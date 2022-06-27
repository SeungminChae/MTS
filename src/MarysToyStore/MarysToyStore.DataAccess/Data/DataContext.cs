using MarysToyStore.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace MarysToyStore.DataAccess.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Specifiy the brand seed data.
            modelBuilder.Entity<Brand>().HasData(
                new Brand { Id = 1, Name = "Mattel" },
                new Brand { Id = 2, Name = "Fisher Price" },
                new Brand { Id = 3, Name = "Hot Wheels" }
            );

            // Specify the user seed data.
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, FirstName = "admin's First name", LastName = "admin's Last Name", EmailAddress = "admin@admin.admin", PasswordHash="AQAAAAEAACcQAAAAECRGWYJZrwdhOR60KJ721NpQDZILO8TKmaYAGZEiy2ZKr+8sAKIlfMrd3PHF202xZw==", IsAdmin = true}
            );

            // Specifiy the product seed data.
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Car", Description = "A toy car that goes really fast.", Price = 3.99m, ImagePath = "/bluecar.jpg", BrandId = 3 },
                new Product { Id = 2, Name = "Ducks", Description = "Toy ducks that float.", Price = 10.99m, ImagePath = "/ducks.jpg", BrandId = 1 },
                new Product { Id = 3, Name = "Legos", Description = "A toy to build your ideas.", Price = 25.99m, ImagePath = "/legos.jpg", BrandId = 1 },
                new Product { Id = 4, Name = "Robot", Description = "An advanced toy that will make anybody happy.", Price = 15.99m, ImagePath = "/robot.jpg", BrandId = 1 },
                new Product { Id = 5, Name = "Teddy", Description = "A soft bear that is comforting to touch.", Price = 29.99m, ImagePath = "/teddy.jpg", BrandId = 2 }
            );

            // Specify the product category seed data.
            modelBuilder.Entity<ProductCategory>().HasData(
                            new ProductCategory { Id = 1, Name = "Sporting Goods" },
                            new ProductCategory { Id = 2, Name = "Home" },
                            new ProductCategory { Id = 3, Name = "Office" },
                            new ProductCategory { Id = 4, Name = "Clothing" },
                            new ProductCategory { Id = 5, Name = "Electronics" }
                        );

            // Specify the product category product seed data.
            modelBuilder.Entity<ProductCategoryProduct>().HasData(
                new ProductCategoryProduct { ProductId = 1, ProductCategoryId = 3 },
                new ProductCategoryProduct { ProductId = 1, ProductCategoryId = 5 }
            );

            // Define the relationship between the Products and the Brands.
            modelBuilder.Entity<Product>()
                .HasOne(x => x.Brand)
                .WithMany(x => x.Products)
                .IsRequired();

            // Define the composite key for ProductCategoryProducts.
            modelBuilder.Entity<ProductCategoryProduct>()
                .HasKey(p => new { p.ProductId, p.ProductCategoryId });

            // Define the relationship between the ProductCategoryProducts and the Products.
            modelBuilder.Entity<ProductCategoryProduct>()
                .HasOne(pcp => pcp.Product)
                .WithMany(p => p.ProductCategoryProducts)
                .HasForeignKey(pcp => pcp.ProductId);

            // Define the relationship between the ProductCategoryProducts and the ProductCaegories.
            modelBuilder.Entity<ProductCategoryProduct>()
                .HasOne(pcp => pcp.ProductCategory)
                .WithMany(p => p.ProductCategoryProducts)
                .HasForeignKey(pcp => pcp.ProductCategoryId);

            // Define the many-to-one relationship between the CartItem and the User.
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.User)
                .WithMany(u => u.CartItems)
                .IsRequired();

            // Define the many-to-one relationship between the CartItem and Product.
            modelBuilder.Entity<CartItem>()
                .HasOne(c => c.Product)
                .WithMany(p => p.CartItems)
                .IsRequired();

            // Define the relationship between the OrderLine and Order
            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.Order)
                .WithMany(o => o.OrderLines)
                .IsRequired();

            // Define the relationship between the OrderLine and Product
            modelBuilder.Entity<OrderLine>()
                .HasOne(ol => ol.Product)
                .WithMany(p => p.OrderLines)
                .IsRequired();
        }

        public DbSet<Product> Products { get; set; }

        public DbSet<Brand> Brands { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<ProductCategory> ProductCategories { get; set; }

        public DbSet<ProductCategoryProduct> ProductCategoryProducts { get; set; }

        public DbSet<CartItem> CartItems { get; set; }

        public DbSet<OrderLine> OrderLines { get; set; }

        public DbSet<Order> Orders { get; set; }
    }
}