using PaginationTagHelper.Pagination;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GenericTagHelperExample.Models
{
    public class CustomerViewModel : IPagingObject<Customer>, IQueryObject
    {
        public string SearchType { get; set; }
        public string SearchItem { get; set; }
        public string SortType { get; set; }
        public bool IsSortDescending { get; set; }

        public int Page { get; set; } = 1;
        public int ItemPerPage { get; set; }
        public int TotalItems { get; set; }
        public IEnumerable<Customer> Items { get; set; }
        public string ItemsJson { get; set; }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}
