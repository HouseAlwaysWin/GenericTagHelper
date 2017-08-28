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

            //var radio = new FormViewModel
            //{
            //    FormModel = new FormModel
            //    {
            //        RadioBoxList = new List<RadioBox>
            //        {
            //        new RadioBox{ Id=1,Name="male"},
            //        new RadioBox {Id=2,Name="female"}
            //        }
            //    }
            //};
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateGenericForm(FormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            context.FormModels.Add(model.FormModel);
            context.SaveChanges();

            return View();
        }

        public IActionResult CreateNormalForm()
        {
            var radio = new FormViewModel
            {
                FormModel = new FormModel
                {
                    RadioBoxList = new List<RadioBox>
                    {
                        new RadioBox{ Id=1,Name="male"},
                        new RadioBox {Id=2,Name="female"}
                    }
                }
            };
            return View(radio);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateNormalForm(FormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            context.FormModels.Add(model.FormModel);
            context.SaveChanges();

            return View("Index");
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
