using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GenericTagHelperExample.Models
{
    public enum Level
    {
        Bronze,
        Silver,
        Gold,
        Patinum
    }
    public class Customer
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Is Admin?")]
        public bool IsAdmin { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }
        [Required]
        public DateTime RegisteredDate { get; set; }

        public Level Level { get; set; }

        public Address Addresses { get; set; }

        public Gender Gender { get; set; }

    }
}
