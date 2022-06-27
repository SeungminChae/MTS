using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarysToyStore.DataAccess.Models
{
    public class OrderLine
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int ProductQuantity { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public decimal Tax { get; set; }

        [Required]
        public decimal Cost { get; set; }

        [NotMapped]
        public decimal TotalTax
        {
            get
            {
                return Math.Round(Tax * ProductQuantity, 2);
            }
        }

        [NotMapped]
        public decimal TotalPrice 
        {
            get
            {
                return Math.Round(Price * ProductQuantity, 2);
            }
        }

        public Order Order { get; set; }

        public Product Product { get; set; }
    }
}