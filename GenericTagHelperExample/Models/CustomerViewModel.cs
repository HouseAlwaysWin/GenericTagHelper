using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericTagHelperExample.Models
{
    public class CustomerViewModel
    {
        public string CustomerList { get; set; }
        public string QueryList { get; set; }
        public int CurrentPage { get; set; }
    }
}
