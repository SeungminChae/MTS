using System.Security.Cryptography.X509Certificates;
using MarysToyStore.DataAccess.Data;
using System.Linq;
using System.Collections.Generic;
using MarysToyStore.DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using MarysToyStore.DataAccess.Enum;

namespace MarysToyStore.Services
{
    public class DataService
    {
        private readonly DataContext _dataContext;

        public DataService(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public List<Product> GetProducts()
        {
            return _dataContext.Products
                .AsNoTracking()
                .Where(x=> x.IsArchived == false)
                .Include(p => p.ProductCategoryProducts)
                .ToList();
        }

        public Product GetProduct(int id)
        {
            return _dataContext.Products
            .AsNoTracking()
            .Include(p => p.ProductCategoryProducts)
            .FirstOrDefault(x => x.Id == id);

            /* same if you did tsql method
            return _dataContext.Products.Where(x => x.Id == id).FirstOrDefeault(); */
        }

        public List<Brand> GetBrands()
        {
            return _dataContext.Brands
                .AsNoTracking()
                .Where(x=> x.IsArchived == false)
                .ToList();
        }

        public Brand GetBrand(int brandId)
        {
            return _dataContext.Brands
                .AsNoTracking().Where(x => x.Id == brandId).FirstOrDefault();
        }

        public Brand AddBrand(Brand brand)
        {
            _dataContext.Brands.Add(brand);
            _dataContext.SaveChanges();

            // what is brand id? which is auto-generated.
            // brand.Id after the save change.
            return brand;
        }

        public Product AddProduct(Product product)
        {
            _dataContext.Products.Add(product);
            _dataContext.SaveChanges();
            

            return product;
        }

        public User AddUser(User user)
        {
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();

            return user;
        }

        public User GetUser(string emailAddress)
        {
            User user = _dataContext.Users
                .AsNoTracking()
                .Where(x => x.EmailAddress.ToLower() == emailAddress.ToLower())
                .FirstOrDefault();

            return user;
        }

        public User GetUser(int id)
        {
            return _dataContext.Users
                .AsNoTracking()
                .FirstOrDefault(u => u.Id == id);
        }

        public void UpdateUser(User u)
        {
            _dataContext.Users.Update(u);
            _dataContext.SaveChanges();
        }

        public void UpdateBrand(Brand brand)
        {
            _dataContext.Brands.Update(brand);
            _dataContext.SaveChanges();
        }

        public void UpdateProduct(Product product){
            _dataContext.RemoveRange(_dataContext.ProductCategoryProducts
                .Where(x => x.ProductId == product.Id));

            _dataContext.Products.Update(product);
            _dataContext.SaveChanges();
        }

        public ProductCategory AddProductCategory(ProductCategory productCategory)
        {
            _dataContext.ProductCategories.Add(productCategory);
            _dataContext.SaveChanges();

            return productCategory;
        }

        public ProductCategory GetProductCategory(int id)
        {
            return _dataContext.ProductCategories
                .AsNoTracking()
                .FirstOrDefault(x=> x.Id == id);
        }

        public List<ProductCategory> GetProductCategories()
        {
            return _dataContext.ProductCategories
                .AsNoTracking()
                .Where(x=> x.IsArchived == false)
                .ToList();
        }

        public void UpdateProductCategory(ProductCategory productCategory)
        {
            _dataContext.ProductCategories.Update(productCategory);
            _dataContext.SaveChanges();
        }

        public void DeleteBrand(int brandId)
        {
            Brand brand = _dataContext.Brands
                .Where(x => x.Id == brandId)
                .FirstOrDefault();

            brand.IsArchived = true;

            _dataContext.SaveChanges();
        }

        public List<Brand> GetDeletedBrands()
        {
            return _dataContext.Brands
                .Where(x => x.IsArchived)
                .ToList();
        }

        public void RestoreBrand(int brandId)
        {
            Brand brand = _dataContext.Brands
                .Where(x => x.Id == brandId)
                .FirstOrDefault();

            brand.IsArchived = false;

            _dataContext.SaveChanges();
        }

        public void DeleteProduct(int productId)
        {
            Product product = _dataContext.Products
                .FirstOrDefault(p => p.Id == productId);

            product.IsArchived = true;

            _dataContext.SaveChanges();
        }

        public List<Product> GetDeletedProducts()
        {
            return _dataContext.Products
                .Where(p => p.IsArchived)
                .ToList();
        }

        public void RestoreProduct(int productId)
        {
            _dataContext.Products
                .FirstOrDefault(p => p.Id == productId)
                .IsArchived = false;

            _dataContext.SaveChanges();
        }

        public void DeleteProductCategory(int id)
        {
            _dataContext.ProductCategories
                .FirstOrDefault(pc => pc.Id == id)
                .IsArchived = true;

            _dataContext.SaveChanges();
        }

        public List<ProductCategory> GetDeletedProductCategory()
        {
            return _dataContext.ProductCategories
                .Where(pc => pc.IsArchived)
                .ToList();
        }

        public void RestoreProductCategory(int id)
        {
            _dataContext.ProductCategories
                .FirstOrDefault(pc => pc.Id == id)
                .IsArchived = false;

            _dataContext.SaveChanges();
        }

        public CartItem GetCartItem(int userId, int productId)
        {
            CartItem cartItem = _dataContext.CartItems
                .Where(x => x.UserId == userId && x.ProductId == productId && !x.IsArchived)
                .Include(x => x.Product)
                .FirstOrDefault();

            return cartItem;
        }

        public List<CartItem> GetCartItems(int userId)
        {
            return _dataContext.CartItems
                .AsNoTracking()
                .Where(x => x.UserId == userId && !x.IsArchived)
                .Include(x => x.Product)
                .ToList();
        }

        public void UpdateCartItem(CartItem cartItem)
        {
            _dataContext.CartItems.Update(cartItem);
            _dataContext.SaveChanges();
        }

        public void AddCartItem(int userId, int productId)
        {
            CartItem existingCartItem = _dataContext.CartItems
                .Where(x => x.UserId == userId && x.ProductId == productId && !x.IsArchived)
                .FirstOrDefault();

            // Chekc if the cart item with the given IDs exists.
            if (existingCartItem != null)
            {
                existingCartItem.Quantity++;
            }
            else
            {
                CartItem cartItem = new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = 1
                };

                _dataContext.CartItems.Add(cartItem);
            }

            _dataContext.SaveChanges();
        }

        public void DeleteCartItem(int userId, int productId)
        {
            CartItem existingCartItem = GetCartItem(userId, productId);

            existingCartItem.IsArchived = true;

            _dataContext.SaveChanges();
        }

        public List<Order> GetOrders(int userId)
        {
            return _dataContext.Orders
                .AsNoTracking()
                .Where(x => x.UserId == userId)
                .Include(x => x.OrderLines)
                .ToList();
        }

        public void AddOrder(Order order)
        {
            // to prevent adding duplicate data of products in database.
            // This is a hack - remote products so they don't get added to the database (again).
            foreach(OrderLine ol in order.OrderLines)
            {
                ol.Product = null;
            }
            _dataContext.Orders.Add(order);
            _dataContext.SaveChanges();
        }

        public void EmptyCart(int userId)
        {
            List<CartItem> cartItems = _dataContext.CartItems
                .Where(x => x.UserId == userId)
                .ToList();

            foreach(CartItem ci in cartItems)
            {
                ci.IsArchived = true;
                UpdateCartItem(ci);
            }
        }

        public List<Order> GetOrders(){
            return _dataContext.Orders.AsNoTracking().Include(x => x.OrderLines).ToList();
        }

        public Order GetOrder(int orderId){
            Order order = _dataContext.Orders
                .AsNoTracking()
                .Where(x => x.Id == orderId)
                .Include(x => x.OrderLines)
                .Include(x => x.User)
                .FirstOrDefault();

            return order;
        }

        public void UpdateOrderStatus(int orderId, OrderStatus orderStatus)
        {
             Order order = _dataContext.Orders
                .Where(x => x.Id == orderId)
                .FirstOrDefault();

            // Update the order status on the order record.
            order.OrderStatus = orderStatus;
            
            _dataContext.SaveChanges();
        }
    }
}