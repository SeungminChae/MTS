using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MarysToyStore.Services;
using MarysToyStore.DataAccess.Models;
using MarysToyStore.DataAccess.Data;
using System.Security.Claims;

namespace MarysToyStore.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private readonly DataService _dataService;

        public CartViewComponent(DataContext dataContext)
        {
           _dataService = new DataService(dataContext);
        }

        public IViewComponentResult Invoke()
        {
            List<CartItem> cartItems = new List<CartItem>();

            if (User.Identity.IsAuthenticated)
            {
                int userId = Convert.ToInt32(Request.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                cartItems = _dataService.GetCartItems(userId);
            }

            return View(cartItems);
        }
    }
}