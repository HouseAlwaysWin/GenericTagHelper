using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GenericFormTagHelperExample.Models
{
    public class Customer
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public bool IsAdmin { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public DateTime RegisteredDate { get; set; }
    }
}
