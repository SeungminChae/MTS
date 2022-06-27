using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MarysToyStore;
using MarysToyStore.DataAccess.Models;
using MarysToyStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MarysToyStore.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;
using MarysToyStore.Models;

namespace MarysToyStore.Controllers
{
    [Route("admin")]
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppConfig _appConfig;

        private readonly DataService _dataService;

        public AdminController(
            ILogger<HomeController> logger,
            IOptions<AppConfig> appConfig,
            DataContext dataContext)
        {
            _logger = logger;
            _appConfig = appConfig.Value;
            _dataService = new DataService(dataContext);
        }

        [Route("products")]
        public IActionResult Products()
        {
            List<Product> model = _dataService.GetProducts();

            return View(model);
        }

        // Short version for this attribute [HttpGet, Route("addbrand")]
        [HttpGet("addbrand")]
        public IActionResult AddBrand()
        {
            return View();
        }

        [HttpPost("addbrand")]
        public IActionResult AddBrand(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _dataService.AddBrand(brand);

            // instead of hard-coding in name of method directly to this line. return RedirectToAction("Brands");
            // This will throw error if method with name of Brands don't exist.
            return RedirectToAction(nameof(Brands));
        }

        [Route("brands")]
        public IActionResult Brands()
        {
            // Need logic to return the list of brands.
            List<Brand> model = _dataService.GetBrands();

            return View(model);
        }

        [HttpGet("addproduct")]
        public IActionResult AddProduct()
        {
            ProductViewModel productViewModel = new ProductViewModel();
            productViewModel.Brands = _dataService.GetBrands();
            productViewModel.AllProductCategories = _dataService.GetProductCategories();

            return View(productViewModel);
        }

        [HttpPost("addproduct")]
        public IActionResult AddProduct(ProductViewModel productViewModel)
        {
            if (!ModelState.IsValid)
            {
                ProductViewModel tempProductViewModel = new ProductViewModel();
                tempProductViewModel.Brands = _dataService.GetBrands();
                tempProductViewModel.AllProductCategories = _dataService.GetProductCategories();

                return View(tempProductViewModel);
            }

            if (productViewModel.SelectedProductCategoryIds != null)
            {
                productViewModel.Product.ProductCategoryProducts = new List<ProductCategoryProduct>();
                foreach (int productCategoryId in productViewModel.SelectedProductCategoryIds)
                {
                    productViewModel.Product.ProductCategoryProducts.Add(new ProductCategoryProduct { ProductCategoryId = productCategoryId });
                }
            }

            _dataService.AddProduct(productViewModel.Product);

            return RedirectToAction(nameof(Products));
        }

        [HttpGet, Route("edit-brand/{id:int}")]
        public IActionResult EditBrand(int id)
        {
            Brand brand = _dataService.GetBrand(id);

            return View(brand);
        }

        [HttpPost, Route("edit-brand/{id:int}")]
        public IActionResult EditBrand(Brand brand)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _dataService.UpdateBrand(brand);

            return RedirectToAction(nameof(Brands));
        }

        [HttpGet, Route("edit-product/{id:int}")]
        public IActionResult EditProduct(int id)
        {
            ProductViewModel model = new ProductViewModel();
            model.Product = _dataService.GetProduct(id);
            model.Brands = _dataService.GetBrands();
            model.AllProductCategories = _dataService.GetProductCategories();

            // Setting the list of related product categories.
            model.SelectedProductCategoryIds = new List<int>();
            foreach (ProductCategoryProduct pcp in model.Product.ProductCategoryProducts)
            {
                model.SelectedProductCategoryIds.Add(pcp.ProductCategoryId);
            }

            return View(model);
        }

        [HttpPost, Route("edit-product/{id:int}")]
        public IActionResult EditProduct(ProductViewModel productViewModel)
        {
            if (!ModelState.IsValid)
            {
                productViewModel.Brands = _dataService.GetBrands();
                Product dbProduct = _dataService.GetProduct(productViewModel.Product.Id);

                productViewModel.SelectedProductCategoryIds = new List<int>();
                foreach (ProductCategoryProduct pcp in dbProduct.ProductCategoryProducts)
                {
                    productViewModel.SelectedProductCategoryIds.Add(pcp.ProductCategoryId);
                }

                productViewModel.AllProductCategories = _dataService.GetProductCategories();

                return View(productViewModel);
            }

            // Convert the list of ints to the a list of product category products.
            if (productViewModel.SelectedProductCategoryIds != null)
            {
                productViewModel.Product.ProductCategoryProducts = new List<ProductCategoryProduct>();                
                foreach(int productCategoryId in productViewModel.SelectedProductCategoryIds)
                {
                    productViewModel.Product.ProductCategoryProducts.Add(new ProductCategoryProduct { ProductCategoryId = productCategoryId });
                }
            }

            _dataService.UpdateProduct(productViewModel.Product);

            return RedirectToAction(nameof(Products));
        }

        [Route("productCategories")]
        public IActionResult ProductCategories()
        {
            List<ProductCategory> productCategories = _dataService.GetProductCategories();

            return View(productCategories);
        }

        [HttpGet("addProductCategory")]
        public IActionResult AddProductCategory()
        {
            return View();
        }

        [HttpPost("addProductCategory")]
        public IActionResult AddProductCategory(ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _dataService.AddProductCategory(productCategory);

            return RedirectToAction(nameof(ProductCategories));
        }

        [HttpGet, Route("edit-productCategories/{id:int}")]
        public IActionResult EditProductCategory(int id)
        {
            ProductCategory ProductCategory = _dataService.GetProductCategory(id);

            return View(ProductCategory);
        }

        [HttpPost, Route("edit-productCategories/{id:int}")]
        public IActionResult EditProductCategory(ProductCategory productCategory)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            _dataService.UpdateProductCategory(productCategory);

            return RedirectToAction(nameof(ProductCategories));
        }

        [HttpGet("delete-brand/{id:int}")]
        public IActionResult DeleteBrandConfirm(int id)
        {
            Brand brand = _dataService.GetBrand(id);

            return View(brand);
        }

        [HttpPost("delete-brand/{id:int}")]
        public IActionResult DeleteBrand(int id)
        {
            _dataService.DeleteBrand(id);

            return RedirectToAction(nameof(Brands));
        }
        [HttpGet("restore-brand")]
        public IActionResult RestoreBrand()
        {
            List<Brand> deletedBrands = _dataService.GetDeletedBrands();

            return View(deletedBrands);
        }

        [HttpGet("restore-brand/{id:int}")]
        public IActionResult RestoreBrand(int id)
        {
            _dataService.RestoreBrand(id);

            return RedirectToAction(nameof(Brands));
        }

        [HttpGet("delete-product/{id:int}")]
        public IActionResult DeleteProductConfirm(int id)
        {
            ProductViewModel model = new ProductViewModel();
            model.Product = _dataService.GetProduct(id);
            model.Product.Brand = _dataService.GetBrand((int)model.Product.BrandId);
            model.AllProductCategories = _dataService.GetProductCategories();

            // Setting the list of related product categories.
            model.SelectedProductCategoryIds = new List<int>();
            foreach (ProductCategoryProduct pcp in model.Product.ProductCategoryProducts)
            {
                model.SelectedProductCategoryIds.Add(pcp.ProductCategoryId);
            }

            return View(model);
        }

        [HttpPost("delete-product/{id:int}")]
        public IActionResult DeleteProduct(int id)
        {
            _dataService.DeleteProduct(id);

            return RedirectToAction(nameof(Products));
        }

        [HttpGet("restore-product")]
        public IActionResult RestoreProduct()
        {
            List<Product> model = _dataService.GetDeletedProducts();

            return View(model);
        }

        [HttpGet("restore-product/{id:int}")]
        public IActionResult RestoreProduct(int id)
        {
            _dataService.RestoreProduct(id);

            return RedirectToAction(nameof(Products));
        }

        [HttpGet("delete-productCategory/{id:int}")]
        public IActionResult DeleteProductCategoryConfirm(int id)
        {
            ProductCategory pc = _dataService.GetProductCategory(id);

            return View(pc);
        }

        [HttpPost("delete-productCategory/{id:int}")]
        public IActionResult DeleteProductCategory(int id)
        {
            _dataService.DeleteProductCategory(id);

            return RedirectToAction(nameof(ProductCategories));
        }

        [HttpGet("restore-category")]
        public IActionResult RestoreProductCategory()
        {
            List<ProductCategory> pc = _dataService.GetDeletedProductCategory();

            return View(pc);
        }

        [HttpGet("restore-category/{id:int}")]
        public IActionResult RestoreProductCategory(int id)
        {
            _dataService.RestoreProductCategory(id);

            return RedirectToAction(nameof(ProductCategories));
        }

        [HttpGet("orders")]
        public IActionResult Orders()
        {
            List<Order> orders = _dataService.GetOrders();

            return View(orders);
        }

        [HttpGet("orderdetails/{orderId:int}")]
        public IActionResult OrderDetails(int orderId)
        {
            Order order = _dataService.GetOrder(orderId);

            return View(order);
        }

        [HttpPost("orderdetails/{orderId:int}")]
        public IActionResult OrderDetails(Order order)
        {
            if(!ModelState.IsValid)
            {
                return View();
            }

            _dataService.UpdateOrderStatus(order.Id, order.OrderStatus);

            return RedirectToAction(nameof(Orders));
        }
    }
}
