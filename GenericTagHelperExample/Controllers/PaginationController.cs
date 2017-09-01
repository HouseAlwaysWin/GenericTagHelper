﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GenericTagHelperExample.Data;
using GenericTagHelperExample.Models;
using Newtonsoft.Json;

namespace GenericTagHelperExample.Controllers
{
    public class PaginationController : Controller
    {
        private GenericDbContext context;
        public PaginationController(GenericDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index(int page)
        {
            var customers = context.Customers.ToList();
            var queryList = new Dictionary<string, string>
            {
                ["page"] = page.ToString()
            };

            var customerList = new CustomerViewModel
            {
                CustomerList = JsonConvert.SerializeObject(customers),
                QueryList = JsonConvert.SerializeObject(queryList),
                CurrentPage = page
            };

            return View(customerList);
        }
    }
}