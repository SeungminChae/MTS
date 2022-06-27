using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace MarysToyStore.DataAccess.Models
{
    public class Brand
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public bool IsArchived { get; set; }

        public List<Product> Products { get; set; }
    }
}