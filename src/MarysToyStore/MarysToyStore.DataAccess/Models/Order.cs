using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MarysToyStore.DataAccess.Enum;

namespace MarysToyStore.DataAccess.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required, Display(Name = "Order Status")]
        public OrderStatus OrderStatus { get; set; }

        [Display(Name = "Date Ordered")]
        public DateTime DateOrdered { get; set; }

        [NotMapped]
        public decimal Tax
        {
            get
            {
                decimal tax = 0m;
                foreach(OrderLine ol in OrderLines)
                {
                    tax += ol.TotalTax;
                }
                return Math.Round(tax, 2);
            }
        }

        [NotMapped]
        public decimal Price
        {
            get
            {
                decimal price = 0m;
                foreach(OrderLine ol in OrderLines)
                {
                    price += ol.TotalPrice;
                }
                return Math.Round(price, 2);
            }
        }

        [NotMapped]
        public decimal Cost
        {
            get
            {
                decimal cost = 0m;
                foreach(OrderLine ol in OrderLines)
                {
                    cost += ol.Cost;
                }
                return Math.Round(cost, 2);
            }
        }

        public User User { get; set; }

        public List<OrderLine> OrderLines { get; set; } = new List<OrderLine>();
    }
}