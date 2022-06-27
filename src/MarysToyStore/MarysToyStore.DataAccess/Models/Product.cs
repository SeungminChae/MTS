using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MarysToyStore.DataAccess.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        [MinLength(20, ErrorMessage = "The description must be longer than 20 characters." )]
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
                
        [Required]
        [Range(.01, 1000000)]
        [DataType(DataType.Currency)]
        public decimal Price { get; set; }
        // by default, this variable will contain "no-product-image.jpg".
        public string ImagePath { get; set; } = "no-product-image.jpg";

        [Required]
        public bool IsArchived { get; set; }

        [Required]
        public int? BrandId { get; set; }

        // This is a "navigation property"
        public Brand Brand { get; set; }

        public List<ProductCategoryProduct> ProductCategoryProducts { get; set; }
        
        public List<CartItem> CartItems { get; set; }

        public List<OrderLine> OrderLines { get; set; }
    }
}