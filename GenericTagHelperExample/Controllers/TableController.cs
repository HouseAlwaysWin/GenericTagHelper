using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PaginationTagHelper.Extensions;
using GenericTagHelperExample.Data;
using GenericTagHelperExample.Models;
using Newtonsoft.Json;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Html;
using System.Text.Encodings.Web;
using System.IO;
using GenericTagHelper.Extensions;

namespace GenericTagHelperExample.Controllers
{
    public class TableController : Controller
    {
        private GenericDbContext context;
        public TableController(GenericDbContext context)
        {
            this.context = context;
        }

        public IActionResult Index(CustomerViewModel model)
        {
            var customers = context.Customers.AsQueryable();

            customers = customers.ToOrderByList(model.SortType, model.IsSortDescending);


            model.ItemPerPage = 10;

            var customersPagingList = customers.ToPageList(model.Page, model.ItemPerPage);

            var jsonData = JsonConvert.SerializeObject(customersPagingList);

            string sortId = Sorting(model, "Id");
            string sortName = Sorting(model, "Name");

            var customerList = new CustomerViewModel
            {
                IsSortDescending = model.IsSortDescending,
                SortType = model.SortType,
                SearchItem = model.SearchItem,
                SearchType = model.SearchType,

                ItemPerPage = model.ItemPerPage,
                Page = model.Page,
                ItemsJson = jsonData,
                TotalItems = customers.Count(),

                Id = sortId,
                Name = sortName
            };

            return View(customerList);
        }

        public string Sorting(CustomerViewModel model, string sortType)
        {
            if (model.IsSortDescending)
            {
                return SortLink(model, false, sortType).ToHtmlString();
            }
            else
            {
                return SortLink(model, true, sortType).ToHtmlString();
            }
        }

        public TagBuilder SortLink(
            CustomerViewModel model, bool isSortDescending, string sortType)
        {
            TagBuilder link = new TagBuilder("a");
            TagBuilder span = new TagBuilder("span");

            var route = Url.Action("Index", new
            {
                page = model.Page,
                searchtype = model.SearchType,
                searchItem = model.SearchItem,
                sorttype = sortType,
                IsSortDescending = isSortDescending,
            });

            link.ToGeneralTagHtmlString("",
                new Dictionary<string, string>
                {
                    ["href"] = route
                });

            span.ToGeneralTagHtmlString("",
                new Dictionary<string, string>
                {
                    ["class"] = "glyphicon glyphicon-sort-by-alphabet-alt",
                    ["aria-hidden"] = "true"
                });

            link.InnerHtml.AppendHtml(span);
            return link;
        }
    }
}