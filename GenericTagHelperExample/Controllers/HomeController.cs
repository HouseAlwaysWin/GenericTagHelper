using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GenericTagHelperExample.Models;
using GenericTagHelperExample.Data;

namespace GenericTagHelperExample.Controllers
{
    public class HomeController : Controller
    {
        public GenericFormDbContext context;
        public HomeController(GenericFormDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult CreateGenericForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateGenericForm(Customer model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateGenericForm");
            }

            context.Customers.Add(model);
            context.SaveChanges();

            return View();
        }

        public IActionResult CreateForm()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateForm(Customer model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            context.Customers.Add(model);
            context.SaveChanges();

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
