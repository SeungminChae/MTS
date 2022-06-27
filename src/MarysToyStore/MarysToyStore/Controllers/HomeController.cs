using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MarysToyStore;
using MarysToyStore.DataAccess.Models;
using MarysToyStore.Models;
using MarysToyStore.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MarysToyStore.DataAccess.Data;
using Microsoft.AspNetCore.Authorization;

namespace MarysToyStore.Controllers
{
    [Authorize]
    [Route("")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly AppConfig _appConfig;

        private readonly DataService _dataService;

        public HomeController(
            ILogger<HomeController> logger,
            IOptions<AppConfig> appConfig,
            DataContext dataContext)
        {
            _logger = logger;
            _appConfig = appConfig.Value;
            _dataService = new DataService(dataContext);
        }

        [AllowAnonymous]
        [Route("")]
        public IActionResult Index(string sort, string filter, int? pageNumber)
        {
            const int pageSize = 3;
            List<Product> model = _dataService.GetProducts();

            // Filter based on filter param.
            if (!String.IsNullOrEmpty(filter))
            {
                model = model
                    .Where(p => p.Name.ToLower().Contains(filter.ToLower())
                        || p.Description.ToLower().Contains(filter.ToLower()))
                    .ToList();
            }

            // Sort based on the sort param.
            switch (sort)
            {
                case "name_desc":
                    model = model.OrderByDescending(x => x.Name).ToList();
                    break;
                case "price_asc":
                    model = model.OrderBy(x => x.Price).ToList();
                    break;
                case "price_desc":
                    model = model.OrderByDescending(x => x.Price).ToList();
                    break;
                default:
                    // Order by ascending.
                    model = model.OrderBy(x => x.Name).ToList();
                    break;
            }
            
            // setting the "next" sort.
            ViewData["NameSortParm"] = String.IsNullOrEmpty(sort) ? "name_desc" : "";
            ViewData["PriceSortParm"] = sort == "price_asc" ? "price_desc" : "price_asc";

            ViewData["Filter"] = filter;

            // x ?? 1 = (x != null ? x : 1) 
            PaginatedList<Product> productList = PaginatedList<Product>.Create(model, pageNumber ?? 1, pageSize);
            return View(productList);
        }

        [AllowAnonymous]
        [Route("about")]
        public IActionResult About()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("product/{productId:int}")]
        public IActionResult Product([FromRoute] int productId)
        {
            Product model = _dataService.GetProduct(productId);
            
            return View(model);
        }

        [AllowAnonymous]
        [Route("error")]
        public IActionResult Error()
        {
            return View();
        }
    }
}
