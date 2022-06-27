using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MarysToyStore.DataAccess.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public int ProductId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        public bool IsArchived { get; set; }

        public Product Product { get; set; }
        
        public User User { get; set; }
    }
}