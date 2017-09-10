using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace GenericTagHelperExample.Models
{
    public class SearchBarViewModel
    {
        [DataType("Select")]
        public int SearchSelectList { get; set; }
        public string SearchInput { get; set; }
    }
}
