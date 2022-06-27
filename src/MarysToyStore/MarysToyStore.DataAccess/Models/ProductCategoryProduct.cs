namespace MarysToyStore.DataAccess.Models
{
    public class ProductCategoryProduct
    {
        public int ProductId { get; set; }

        public Product Product { get; set; }

        public int ProductCategoryId { get; set; }

        public ProductCategory ProductCategory { get; set; }
    }
}