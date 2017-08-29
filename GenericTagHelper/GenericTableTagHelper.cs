using AutoMapper;
using GenericTagHelper.MethodHelpers;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericTagHelper
{
    [HtmlTargetElement("table", Attributes = "generic")]
    public class GenericTableTagHelper : TagHelper
    {

        private IUrlHelperFactory urlHelperFactory;
        public GenericTableTagHelper(
            IUrlHelperFactory urlHelperFactory)
        {
            this.urlHelperFactory = urlHelperFactory;
        }

        [HtmlAttributeNotBound]
        public IUrlHelper urlHelper
        {
            get
            {
                return urlHelperFactory.GetUrlHelper(ViewContext);
            }
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public string NoItemsMessage { get; set; } = "No Items";




        #region Pagination Properties
        public int CurrentPage { get; set; } = 1;
        public int ItemPerPage { get; set; } = 5;



        //private int TotalPages
        //{
        //    get
        //    {
        //        if (ItemPerPage <= 0)
        //        {
        //            ItemPerPage = 5;
        //        }
        //        return (int)Math.Ceiling((decimal)
        //            ItemsList.Count / ItemPerPage);
        //    }
        //}


        public string CurrentPageParameter { get; set; } = "page";
        public string QueryList { get; set; }
        private Dictionary<string, string> QueryOptions
        {
            get
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(QueryList);
            }
        }
        private Dictionary<string, string> QueryListDict { get; set; } = new Dictionary<string, string>();




        public string Items { get; set; }
        private List<Dictionary<string, string>> ItemsList
        {
            get
            {
                return JsonDeserializeConvert_LDss(Items);
            }
        }
        public int TotalItems
        {
            get
            {
                return ItemsList.Count;
            }
        }


        private List<Dictionary<string, string>> ItemsAfterPagination
        {
            get
            {
                var query = JsonDeserializeConvert_LDss(Items);
                return query.Skip((CurrentPage - 1)
                    * ItemPerPage).Take(ItemPerPage).ToList();
            }
        }

        public bool ActiveQueryOptions { get; set; }


        public string PageAction { get; set; } = "";

        public string PageController { get; set; } = "";

        public string PageStyleClass { get; set; } = "pagination";

        public string ActivateClass { get; set; } = "active";

        public string DisableClass { get; set; } = "disabled";

        public int PageMiddleLength { get; set; } = 2;

        public int PageTopBottomLength { get; set; } = 5;

        public string PreviousIcon { get; set; } = "Previous";

        public string NextIcon { get; set; } = "Next";

        public string FirstIcon { get; set; } = "First";

        public string LastIcon { get; set; } = "Last";

        public bool ShowFirstPage { get; set; } = true;

        public bool ShowLastPage { get; set; } = true;

        public string BetweenIcon { get; set; } = "...";

        public bool ShowBetweenIcon { get; set; } = true;

        public bool ExchangePreviousFirstBtn { get; set; }

        public bool ExchangeNextLastBtn { get; set; }

        public bool ActivePagination { get; set; }
        #endregion

        public string TableHeads { get; set; }
        private List<string> TableHeadList
        {
            get
            {
                return JsonDeserializeConvert_Ls(TableHeads);
            }
        }

        public string AttributesTable { get; set; }
        private Dictionary<string, string> AttributesTableDict
        {
            get
            {
                return JsonDeserializeConvert_Dss(AttributesTable);
            }
        }


        public string AttributesTableHead { get; set; }
        private Dictionary<string, string> AttributesTableHeadDict
        {
            get
            {
                return JsonDeserializeConvert_Dss(AttributesTableHead);
            }
        }

        public string AttributesTableHeadTr { get; set; }
        private Dictionary<string, string> AttributesTableHeadTrDict
        {
            get
            {
                return JsonDeserializeConvert_Dss(AttributesTableHeadTr);
            }
        }

        public string AttributesTableBody { get; set; }
        private Dictionary<string, string> AttributesTableBodyDict
        {
            get
            {
                return JsonDeserializeConvert_Dss(AttributesTableBody);
            }
        }


        public string HtmlContentTh { get; set; }
        private Dictionary<string, Dictionary<string, string>> HtmlContentThDict
        {
            get
            {
                return JsonDeserializeConvert_DsDss(HtmlContentTh);
            }
        }

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {

            TagBuilder table = new TagBuilder("table");
            table = AddAttributes(table, AttributesTableDict);

            TagBuilder thead = new TagBuilder("thead");
            thead = AddAttributes(thead, AttributesTableHeadDict);

            TagBuilder thead_tr = new TagBuilder("tr");
            thead_tr = AddAttributes(thead_tr, AttributesTableHeadTrDict);

            foreach (var name in TableHeadList)
            {
                TagBuilder th = new TagBuilder("th");
                th.InnerHtml.AppendHtml(name);
                thead_tr.InnerHtml.AppendHtml(th);
            }

            thead.InnerHtml.AppendHtml(thead_tr);
            //output.Content.AppendHtml(thead);

            TagBuilder tbody = new TagBuilder("tbody");
            tbody = AddAttributes(tbody, AttributesTableBodyDict);


            if (ItemsAfterPagination.Count == 0)
            {
                TagBuilder tbody_tr = new TagBuilder("tr");

                TagBuilder td = new TagBuilder("td");

                td.InnerHtml.AppendHtml(NoItemsMessage);

                tbody_tr.InnerHtml.AppendHtml(td);

                tbody.InnerHtml.AppendHtml(tbody_tr);
            }
            else
            {
                ItemsAfterPagination.ForEach(items =>
                {
                    TagBuilder tbody_tr = new TagBuilder("tr");
                    items.ToDictionary(item =>
                        {
                            TagBuilder td = new TagBuilder("td");

                            td.InnerHtml.AppendHtml(item.Value);

                            tbody_tr.InnerHtml.AppendHtml(td);
                            return td;
                        });

                    tbody.InnerHtml.AppendHtml(tbody_tr);
                });
            }



            output.Content.AppendHtml(tbody);



            if (ActivePagination)
            {
                PaginationBuilder paginaionBuilder = new PaginationBuilder(
                    ViewContext, urlHelperFactory);
                
                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<GenericTableTagHelper, PaginationBuilder>();
                });
                Mapper.Map<GenericTableTagHelper, PaginationBuilder>(this,paginaionBuilder);
                output.PostElement.AppendHtml(paginaionBuilder.CreatePagination());
            }
        }

        //#region Pagination Helpers
        //public TagBuilder GeneratePagination()
        //{
        //    if (TotalPages <= 1)
        //    {

        //    }

        //    // <ul class="pagination"></ul>
        //    TagBuilder ul = new TagBuilder("ul");
        //    ul.AddCssClass(PageStyleClass);

        //    // Show Middle Page Button
        //    for (int i = 1; i <= TotalPages; ++i)
        //    {

        //        TagBuilder list_li = PageButton(
        //            has_link: true,
        //            page_action: i,
        //            icon: i.ToString(),
        //            is_disabled: false,
        //            active_page: i == CurrentPage);

        //        TagBuilder first_li = PageButton(
        //            has_link: CurrentPage > 1,
        //            page_action: i,
        //            icon: FirstIcon,
        //            is_disabled: CurrentPage == 1,
        //            active_page: false);

        //        TagBuilder last_li = PageButton(
        //            has_link: CurrentPage < TotalPages,
        //            page_action: i,
        //            icon: LastIcon,
        //            is_disabled: CurrentPage == TotalPages,
        //            active_page: false);

        //        TagBuilder dot_li = PageButton(
        //            has_link: false,
        //            page_action: i,
        //            icon: BetweenIcon,
        //            is_disabled: true,
        //            active_page: false);

        //        /*-----------------------Show First and Previous Page Button-------------------------*/

        //        // Show previous and first btn in different location
        //        if (ExchangePreviousFirstBtn)
        //        {
        //            // Show First Page Btn
        //            if (i == 1 && ShowFirstPage)
        //            {
        //                ul.InnerHtml.AppendHtml(first_li);
        //            }

        //            // Show Previous Page Btn
        //            if (i == 1)
        //            {
        //                var pre_li = PageButton(
        //                    has_link: (CurrentPage - 1) >= 1,
        //                    page_action: CurrentPage - 1,
        //                    icon: PreviousIcon,
        //                    is_disabled: CurrentPage == 1,
        //                    active_page: false);

        //                ul.InnerHtml.AppendHtml(pre_li);
        //                // if current page is bigger than 3 
        //                // TotalPage can't be same as PageMiddleLength 
        //                if (CurrentPage > PageMiddleLength + 1 &&
        //                    TotalPages > (1 + PageMiddleLength * 2) &&
        //                    ShowBetweenIcon)
        //                {
        //                    ul.InnerHtml.AppendHtml(dot_li);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            // Show Previous Btn
        //            if (i == 1)
        //            {
        //                var pre_li = PageButton(
        //                    has_link: (CurrentPage - 1) >= 1,
        //                    page_action: CurrentPage - 1,
        //                    icon: PreviousIcon,
        //                    is_disabled: CurrentPage == 1,
        //                    active_page: false);

        //                ul.InnerHtml.AppendHtml(pre_li);
        //            }

        //            // Show First Page
        //            if (i == 1 && ShowFirstPage)
        //            {
        //                ul.InnerHtml.AppendHtml(first_li);
        //                // if current page is bigger than 3 
        //                if (CurrentPage > PageMiddleLength + 1 &&
        //                    TotalPages > (1 + PageMiddleLength * 2) &&
        //                    ShowBetweenIcon)
        //                {
        //                    ul.InnerHtml.AppendHtml(dot_li);
        //                }
        //            }

        //        }


        //        /*-----------------------Middle Page Button-------------------------*/

        //        // Check if bottom page length is override to top page length
        //        // and only show page once.
        //        bool checkPageRepeated = true;
        //        // Show Middle Pages
        //        // if current page in the bottom then show 5 pages
        //        // Show pages less than 5 && show pages if bigger than current page-3

        //        if (i <= PageTopBottomLength &&
        //            i >= CurrentPage - PageMiddleLength)
        //        {
        //            checkPageRepeated = false;
        //            ul.InnerHtml.AppendHtml(list_li);
        //        }



        //        // Show page number after bottom and top pages
        //        // which is middle pages
        //        if (i > PageTopBottomLength &&
        //            i <= TotalPages - PageTopBottomLength &&
        //            i >= CurrentPage - PageMiddleLength &&
        //            i <= CurrentPage + PageMiddleLength)
        //        {
        //            ul.InnerHtml.AppendHtml(list_li);
        //        }

        //        // Show page larger than total page -5,
        //        // if current page in the top then show 5 pages
        //        // and total pages must be bigger than 5
        //        if (i > TotalPages - PageTopBottomLength &&
        //            i <= CurrentPage + PageMiddleLength &&
        //            checkPageRepeated)
        //        {
        //            ul.InnerHtml.AppendHtml(list_li);
        //        }


        //        /*-----------------------Show Next And Last Page Button-------------------------*/


        //        if (ExchangeNextLastBtn)
        //        {
        //            // Show Next Page Btn 
        //            if (i == TotalPages)
        //            {
        //                var next_li = PageButton(
        //                    has_link: (CurrentPage + 1) <= TotalPages,
        //                    page_action: CurrentPage + 1,
        //                    icon: NextIcon,
        //                    is_disabled: CurrentPage == TotalPages,
        //                    active_page: false);
        //                // if current page is smaller than total page minus five 
        //                if ((CurrentPage < TotalPages - PageMiddleLength) &&
        //                    TotalPages > (1 + PageMiddleLength * 2)
        //                    && ShowBetweenIcon)
        //                {
        //                    ul.InnerHtml.AppendHtml(dot_li);
        //                }

        //                ul.InnerHtml.AppendHtml(next_li);
        //            }

        //            // Show Last Page Btn
        //            if (i == TotalPages && ShowLastPage)
        //            {
        //                ul.InnerHtml.AppendHtml(last_li);
        //            }

        //        }
        //        else
        //        {
        //            // Show Last Page Btn
        //            if (i == TotalPages && ShowLastPage)
        //            {
        //                // if current page is smaller than total page minus five 
        //                if ((CurrentPage < TotalPages - PageMiddleLength) &&
        //                    TotalPages > (1 + PageMiddleLength * 2) &&
        //                    ShowBetweenIcon)
        //                {
        //                    ul.InnerHtml.AppendHtml(dot_li);
        //                }

        //                ul.InnerHtml.AppendHtml(last_li);
        //            }

        //            // Show Next Page Btn
        //            if (i == TotalPages)
        //            {
        //                var next_li = PageButton(
        //                    has_link: (CurrentPage + 1) <= TotalPages,
        //                    page_action: CurrentPage + 1,
        //                    icon: NextIcon,
        //                    is_disabled: CurrentPage == TotalPages,
        //                    active_page: false);


        //                ul.InnerHtml.AppendHtml(next_li);
        //            }
        //        }
        //    }
        //    return ul;
        //}

        //// Show Page Icon
        //// <span aria-hidden="true">{{ icon  }}</span>
        //private TagBuilder PageIcon(string icon)
        //{
        //    TagBuilder span = new TagBuilder("span");
        //    span.MergeAttribute("aria-hidden", "true");
        //    span.InnerHtml.AppendHtml(icon);
        //    return span;
        //}

        //// Show Page Link
        //// <a href="/action?query"></a>
        //private TagBuilder PageLink(
        //    TagBuilder a,
        //    int page_action)
        //{

        //    if (String.IsNullOrEmpty(PageController))
        //    {
        //        QueryListDict[CurrentPageParameter] = page_action.ToString();
        //        a.Attributes["href"] = urlHelper.Action(
        //             PageAction, new { page = page_action });
        //    }
        //    else
        //    {

        //        if (!String.IsNullOrEmpty(QueryList))
        //        {

        //            foreach (var item in QueryOptions)
        //            {
        //                QueryListDict[item.Key] = item.Value;
        //            }

        //        }
        //        QueryListDict[CurrentPageParameter] = page_action.ToString();
        //        a.Attributes["href"] = urlHelper.Action(
        //                             PageAction, PageController,
        //                             QueryListDict);
        //    }

        //    return a;
        //}

        //// Show Page Button
        //// <li disabled>
        ////  <a aria-label="{{ icon }}" herf="">
        ////      <span>{{ icon }}</span>
        ////  </a>
        //// </li>
        //private TagBuilder PageButton(
        //    bool has_link,
        //    int page_action,
        //    string icon,
        //    bool is_disabled,
        //    bool active_page)
        //{
        //    TagBuilder li = new TagBuilder("li");
        //    TagBuilder a = new TagBuilder("a");

        //    a.Attributes["aria-label"] = icon;

        //    if (has_link)
        //    {
        //        a = PageLink(a, page_action);
        //    }

        //    a.InnerHtml.AppendHtml(PageIcon(icon));
        //    li.InnerHtml.AppendHtml(a);

        //    if (is_disabled)
        //    {
        //        li.AddCssClass(DisableClass);
        //    }

        //    if (active_page)
        //    {
        //        a.AddCssClass(ActivateClass);
        //        li.AddCssClass(ActivateClass);
        //    }

        //    return li;
        //}

        //#endregion

        #region Helpers
        private List<Dictionary<string, string>> JsonDeserializeConvert_LDss(
           string propertyString)
        {
            if (!String.IsNullOrEmpty(propertyString))
            {
                return JsonConvert.DeserializeObject<List<Dictionary<string, string>>>(propertyString);
            }
            return new List<Dictionary<string, string>>();
        }

        private List<string> JsonDeserializeConvert_Ls(
                  string propertyString)
        {
            if (!String.IsNullOrEmpty(propertyString))
            {
                return JsonConvert.DeserializeObject<List<string>>(propertyString);
            }
            return new List<string>();
        }



        private Dictionary<string, string> JsonDeserializeConvert_Dss(
                   string classString)
        {
            if (!String.IsNullOrEmpty(classString))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(classString);
            }
            return new Dictionary<string, string>();
        }

        private Dictionary<string, Dictionary<string, string>> JsonDeserializeConvert_DsDss(
                   string attributeString)
        {
            if (!String.IsNullOrEmpty(attributeString))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(attributeString);
            }
            return new Dictionary<string, Dictionary<string, string>>();
        }

        private bool IsContainsPropertyKey(
           Dictionary<string, Dictionary<string, string>> tagClassDict,
           string propertyName)
        {
            return tagClassDict.Any(d => d.Key.Equals(
                propertyName, StringComparison.OrdinalIgnoreCase));
        }

        private TagBuilder AddAttributes(
                           TagBuilder tag, Dictionary<string, string> tagAttributeDict)
        {
            foreach (var attr in tagAttributeDict)
            {
                tag.Attributes[attr.Key] = attr.Value;
            }
            return tag;
        }
        #endregion

    }
}
