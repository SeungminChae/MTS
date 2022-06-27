using System.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using MarysToyStore.Services;
using MarysToyStore.DataAccess.Models;
using MarysToyStore.DataAccess.Data;
using System.Security.Claims;
using MarysToyStore.DataAccess;
using MarysToyStore.DataAccess.Enum;
using Microsoft.Extensions.Logging;

namespace MarysToyStore.Controllers
{
    [Authorize, Route("cart")]
    public class CartController : Controller
    {
        private readonly DataService _dataService;

        private readonly AppConfig _appConfig;

        private readonly ILogger<CartController> _log;

        public CartController(DataContext dataContext,
            IOptions<AppConfig> appConfig,
            ILogger<CartController> log)
        {
           _dataService = new DataService(dataContext);
           _appConfig = appConfig.Value;
           _log = log;
        }

        [HttpGet("view")]
        public IActionResult ViewCart()
        {
            // Get the User ID from the auth cookie.
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            List<CartItem> model = _dataService.GetCartItems(userId);

            return View(model);
        }

        [HttpPost("add-to-cart/{id:int}")]
        public IActionResult AddToCart(int id)
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            _dataService.AddCartItem(userId, id);

            return RedirectToAction(nameof(ViewCart));
        }

        [HttpGet("remove-from-cart/{id:int}")]
        public IActionResult RemoveFromCart(int id)
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            _dataService.DeleteCartItem(userId, id);

            return RedirectToAction(nameof(ViewCart));
        }

        [HttpGet("order-history")]
        public IActionResult OrderHistory()
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            List<Order> orders = _dataService.GetOrders(userId);

            return View(orders);
        }

        [HttpGet("order-review")]
        public IActionResult OrderReview()
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            Order order = BuildOrder(userId);

            return View(order);
        }

        [HttpPost("order-place")]
        public IActionResult PlaceOrder()
        {
            int userId = Convert.ToInt32(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

            Order order = BuildOrder(userId);

            order.OrderStatus = OrderStatus.Placed;
            order.DateOrdered = DateTime.Now;
            _dataService.AddOrder(order);
            _dataService.EmptyCart(userId);

            _log.LogInformation($"Created order {order.Id} for user {userId}.");

            return RedirectToAction(nameof(OrderHistory));
        }

        private Order BuildOrder(int userId)
        {
            Order order = new Order();
            order.OrderLines = new List<OrderLine>();
            order.UserId = userId;

            List<CartItem> cartItems = _dataService.GetCartItems(userId);

            foreach (CartItem cartItem in cartItems)
            {
                OrderLine orderLine = new OrderLine();
                orderLine.ProductId = cartItem.ProductId;
                orderLine.Product = cartItem.Product;
                orderLine.ProductQuantity = cartItem.Quantity;
                orderLine.Price = cartItem.Product.Price;
                orderLine.Tax = cartItem.Product.Price * _appConfig.TaxRate;
                orderLine.Cost = orderLine.TotalPrice + orderLine.TotalTax;

                order.OrderLines.Add(orderLine);
            }

            return order;
        }
    }
}