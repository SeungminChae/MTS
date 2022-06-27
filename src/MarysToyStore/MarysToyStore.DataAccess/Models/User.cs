using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MarysToyStore.DataAccess.Models
{

    public class User
    {
        public int Id { get; set; }

            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required]
            [DataType(DataType.EmailAddress)]
            public string EmailAddress { get; set; }

            [Required]
            public string PasswordHash { get; set; }

            [Display(Name = "Mailing Address")]
            public string Address { get; set; }

            [Display(Name = "Apt or Suite Number")]
            public string Address2 { get; set; }

            public string City { get; set; }

            public string State { get; set; }

            public string Zip { get; set; }

            [Required]
            public bool IsAdmin { get; set; }

            // NotMapped - I don't want this to show up in database column.
            [NotMapped, Display(Name = "Full Name")]
            public string FullName {
                get{
                    return $"{FirstName} {LastName}";
                }
            }

            public List<CartItem> CartItems { get; set;}
    }
}