using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MarysToyStore.DataAccess.Models;

namespace MarysToyStore.Models
{
    public class ProductViewModel
    {
        public Product Product { get; set; }

        public List<Brand> Brands { get; set; }

        public List<ProductCategory> AllProductCategories { get; set; }

        [Display(Name = "Product Categories")]
        public List<int> SelectedProductCategoryIds { get; set; }
    }
}