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
        public GenericDbContext context;
        public HomeController(GenericDbContext context)
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
        public IActionResult CreateGenericForm(FormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("CreateGenericForm");
            }

            context.FormModels.Add(model);
            context.SaveChanges();

            return View();
        }

        public IActionResult CreateNormalForm()
        {
            var radio = new FormModel
            {
                RadioBoxes = new List<RadioBox>
                {
                    new RadioBox { Id=1,Name="Male" },
                    new RadioBox { Id=2,Name="Female" }
                }
            };
            return View(radio);
        }

        [HttpPost]
        public IActionResult CreateNormalForm(FormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            context.FormModels.Add(model);
            context.SaveChanges();

            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
