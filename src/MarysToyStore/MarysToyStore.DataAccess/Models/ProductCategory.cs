using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MarysToyStore.DataAccess.Models
{
    public class ProductCategory
    {
        public int Id { get; set; }

        [Required]
        public bool IsArchived { get; set; }

        [Required]
        public string Name { get; set; }
        public List<ProductCategoryProduct> ProductCategoryProducts { get; set; }
    }
}