﻿using GenericTagHelper.MethodHelpers;
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
using System.Threading.Tasks;

namespace GenericTagHelper
{
    [HtmlTargetElement("pagination")]
    public class GenericPaginationTagHelper : TagHelper
    {
        private IUrlHelperFactory urlHelperFactory;
        public GenericPaginationTagHelper(
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

        public string PaginationTagName { get; set; } = "div";

        public string NoItemsMessage { get; set; } = "No Items";
        public string NoItemsMessageTag { get; set; } = "div";

        public string GroupListTag { get; set; } = "div";
        public string AttributesGroupListTag { get; set; }
        public Dictionary<string, string> AttributesGroupListTagDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttributesGroupListTag);
            }
        }

        public string GroupTag { get; set; } = "ul";
        public string AttributesGroupTag { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttributesGroupTagDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttributesGroupTag);
            }
        }
        public string AttributesAllGroupTag { get; set; }
        private Dictionary<string, string> AttributesAllGroupTagDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttributesAllGroupTag);
            }
        }


        public string ListTag { get; set; } = "li";
        public string AttributesListTag { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttributesListTagDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttributesListTag);
            }
        }

        public bool AddContentBeforeList { get; set; }
        public bool AddContentAfterList { get; set; }

        public int NumberOfTagListOuterLayer { get; set; } = 0;
        public string AttributesTagsListLayer { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttributesTagsListLayerDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttributesTagsListLayer);
            }
        }



        #region Pagination
        public int ItemPerPage { get; set; } = 5;

        public int CurrentPage { get; set; } = 1;

        private int TotalPages
        {
            get
            {
                if (ItemPerPage <= 0)
                {
                    ItemPerPage = 5;
                }
                return (int)Math.Ceiling((decimal)
                    ItemsList.Count / ItemPerPage);
            }
        }


        public string CurrentPageParameter { get; set; } = "page";
        public string PageQueryList { get; set; }

        private Dictionary<string, string> QueryOptions
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(PageQueryList);
            }
        }
        private Dictionary<string, string> QueryListDict { get; set; } = new Dictionary<string, string>();




        public string Items { get; set; }
        private List<Dictionary<string, string>> ItemsList
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_LDss(Items);
            }
        }


        private List<Dictionary<string, string>> ItemsAfterPagination
        {
            get
            {
                var query = JsonDeserialize.JsonDeserializeConvert_LDss(Items);
                return query.Skip((CurrentPage - 1)
                    * ItemPerPage).Take(ItemPerPage).ToList();
            }
        }

        public bool ActiveQueryOptions { get; set; }

        public string PageAction { get; set; } = "";

        public string PageController { get; set; } = "";

        public string PageStyleClass { get; set; } = "pagination";

        public string PageActiveClass { get; set; } = "active";

        public string PageDisableClass { get; set; } = "disabled";

        public int PageMiddleLength { get; set; } = 2;

        public int PageTopBottomLength { get; set; } = 5;

        public string PagePreviousIcon { get; set; } = "Previous";

        public string PageNextIcon { get; set; } = "Next";

        public string PageFirstIcon { get; set; } = "First";

        public string PageLastIcon { get; set; } = "Last";

        public bool PageShowFirst { get; set; } = true;

        public bool PageShowLast { get; set; } = true;

        public string PageBetweenIcon { get; set; } = "...";

        public bool PageShowBetweenIcon { get; set; } = true;

        public bool PageExchangePreviousFirstBtn { get; set; }

        public bool PageExchangeNextLastBtn { get; set; }

        public bool ActivePagination { get; set; }
        #endregion

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {

            TagBuilder group_list_tag = new TagBuilder(GroupListTag);
            if (ItemsAfterPagination.Count == 0)
            {
                TagBuilder no_msg_tag = new TagBuilder(NoItemsMessageTag);

                no_msg_tag.InnerHtml.AppendHtml(NoItemsMessage);

            }
            else
            {
                if (AddContentBeforeList)
                {
                    group_list_tag.InnerHtml.AppendHtml(await output.GetChildContentAsync());
                }

                for (int i = 0; i < ItemsAfterPagination.Count; i++)
                {
                    TagBuilder group_tag = new TagBuilder(GroupTag);
                    HtmlAttributesHelper.AddAttributes(
                        group_tag, AttributesGroupTagDict, i.ToString());

                    if (HtmlAttributesHelper.IsContainsKey(AttributesGroupTagDict, i.ToString()))
                    {
                        HtmlAttributesHelper.AddAttributes(group_tag, AttributesAllGroupTagDict);
                    }

                    ItemsAfterPagination[i].ToDictionary(item =>
                    {
                        TagBuilder list_tag = new TagBuilder(ListTag);

                        list_tag.InnerHtml.AppendHtml(item.Value);

                        group_tag.InnerHtml.AppendHtml(list_tag);
                        return list_tag;
                    });

                    group_list_tag.InnerHtml.AppendHtml(group_tag);
                }
                //ItemsAfterPagination.ForEach(items =>
                //{
                //    TagBuilder group_tag = new TagBuilder(GroupTag);

                //    items.ToDictionary(item =>
                //    {
                //        TagBuilder list_tag = new TagBuilder(ListTag);

                //        list_tag.InnerHtml.AppendHtml(item.Value);

                //        group_tag.InnerHtml.AppendHtml(list_tag);
                //        return list_tag;
                //    });

                //    group_list_tag.InnerHtml.AppendHtml(group_tag);
                //});

                if (AddContentAfterList)
                {
                    group_list_tag.InnerHtml.AppendHtml(await output.GetChildContentAsync());
                }
            }

            group_list_tag = AddLayers(group_list_tag);

            #region Pagination

            if (TotalPages <= 1)
            {

            }

            // <ul class="pagination"></ul>
            TagBuilder nav = new TagBuilder("nav");
            TagBuilder ul = new TagBuilder("ul");
            ul.AddCssClass(PageStyleClass);

            // Show Middle Page Button
            for (int i = 1; i <= TotalPages; ++i)
            {

                TagBuilder list_li = PageButton(
                    has_link: true,
                    page_action: i,
                    icon: i.ToString(),
                    is_disabled: false,
                    active_page: i == CurrentPage);

                TagBuilder first_li = PageButton(
                    has_link: CurrentPage > 1,
                    page_action: i,
                    icon: PageFirstIcon,
                    is_disabled: CurrentPage == 1,
                    active_page: false);

                TagBuilder last_li = PageButton(
                    has_link: CurrentPage < TotalPages,
                    page_action: i,
                    icon: PageLastIcon,
                    is_disabled: CurrentPage == TotalPages,
                    active_page: false);

                TagBuilder dot_li = PageButton(
                    has_link: false,
                    page_action: i,
                    icon: PageBetweenIcon,
                    is_disabled: true,
                    active_page: false);

                /*-----------------------Show First and Previous Page Button-------------------------*/

                // Show previous and first btn in different location
                if (PageExchangePreviousFirstBtn)
                {
                    // Show First Page Btn
                    if (i == 1 && PageShowFirst)
                    {
                        ul.InnerHtml.AppendHtml(first_li);
                    }

                    // Show Previous Page Btn
                    if (i == 1)
                    {
                        var pre_li = PageButton(
                            has_link: (CurrentPage - 1) >= 1,
                            page_action: CurrentPage - 1,
                            icon: PagePreviousIcon,
                            is_disabled: CurrentPage == 1,
                            active_page: false);

                        ul.InnerHtml.AppendHtml(pre_li);
                        // if current page is bigger than 3 
                        // TotalPage can't be same as PageMiddleLength 
                        if (CurrentPage > PageMiddleLength + 1 &&
                            TotalPages > (1 + PageMiddleLength * 2) &&
                            PageShowBetweenIcon)
                        {
                            ul.InnerHtml.AppendHtml(dot_li);
                        }
                    }
                }
                else
                {
                    // Show Previous Btn
                    if (i == 1)
                    {
                        var pre_li = PageButton(
                            has_link: (CurrentPage - 1) >= 1,
                            page_action: CurrentPage - 1,
                            icon: PagePreviousIcon,
                            is_disabled: CurrentPage == 1,
                            active_page: false);

                        ul.InnerHtml.AppendHtml(pre_li);
                    }

                    // Show First Page
                    if (i == 1 && PageShowFirst)
                    {
                        ul.InnerHtml.AppendHtml(first_li);
                        // if current page is bigger than 3 
                        if (CurrentPage > PageMiddleLength + 1 &&
                            TotalPages > (1 + PageMiddleLength * 2) &&
                            PageShowBetweenIcon)
                        {
                            ul.InnerHtml.AppendHtml(dot_li);
                        }
                    }

                }


                /*-----------------------Middle Page Button-------------------------*/

                // Check if bottom page length is override to top page length
                // and only show page once.
                bool checkPageRepeated = true;
                // Show Middle Pages
                // if current page in the bottom then show 5 pages
                // Show pages less than 5 && show pages if bigger than current page-3

                if (i <= PageTopBottomLength &&
                    i >= CurrentPage - PageMiddleLength)
                {
                    checkPageRepeated = false;
                    ul.InnerHtml.AppendHtml(list_li);
                }



                // Show page number after bottom and top pages
                // which is middle pages
                if (i > PageTopBottomLength &&
                    i <= TotalPages - PageTopBottomLength &&
                    i >= CurrentPage - PageMiddleLength &&
                    i <= CurrentPage + PageMiddleLength)
                {
                    ul.InnerHtml.AppendHtml(list_li);
                }

                // Show page larger than total page -5,
                // if current page in the top then show 5 pages
                // and total pages must be bigger than 5
                if (i > TotalPages - PageTopBottomLength &&
                    i <= CurrentPage + PageMiddleLength &&
                    checkPageRepeated)
                {
                    ul.InnerHtml.AppendHtml(list_li);
                }


                /*-----------------------Show Next And Last Page Button-------------------------*/


                if (PageExchangeNextLastBtn)
                {
                    // Show Next Page Btn 
                    if (i == TotalPages)
                    {
                        var next_li = PageButton(
                            has_link: (CurrentPage + 1) <= TotalPages,
                            page_action: CurrentPage + 1,
                            icon: PageNextIcon,
                            is_disabled: CurrentPage == TotalPages,
                            active_page: false);
                        // if current page is smaller than total page minus five 
                        if ((CurrentPage < TotalPages - PageMiddleLength) &&
                            TotalPages > (1 + PageMiddleLength * 2)
                            && PageShowBetweenIcon)
                        {
                            ul.InnerHtml.AppendHtml(dot_li);
                        }

                        ul.InnerHtml.AppendHtml(next_li);
                    }

                    // Show Last Page Btn
                    if (i == TotalPages && PageShowLast)
                    {
                        ul.InnerHtml.AppendHtml(last_li);
                    }

                }
                else
                {
                    // Show Last Page Btn
                    if (i == TotalPages && PageShowLast)
                    {
                        // if current page is smaller than total page minus five 
                        if ((CurrentPage < TotalPages - PageMiddleLength) &&
                            TotalPages > (1 + PageMiddleLength * 2) &&
                            PageShowBetweenIcon)
                        {
                            ul.InnerHtml.AppendHtml(dot_li);
                        }

                        ul.InnerHtml.AppendHtml(last_li);
                    }

                    // Show Next Page Btn
                    if (i == TotalPages)
                    {
                        var next_li = PageButton(
                            has_link: (CurrentPage + 1) <= TotalPages,
                            page_action: CurrentPage + 1,
                            icon: PageNextIcon,
                            is_disabled: CurrentPage == TotalPages,
                            active_page: false);


                        ul.InnerHtml.AppendHtml(next_li);
                    }
                }
            }
            nav.InnerHtml.AppendHtml(ul);
            #endregion

            output.TagName = PaginationTagName;
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Content.AppendHtml(group_list_tag);
            output.Content.AppendHtml(nav);


        }




        // Show Page Icon
        // <span aria-hidden="true">{{ icon  }}</span>
        private TagBuilder PageIcon(string icon)
        {
            TagBuilder span = new TagBuilder("span");
            span.MergeAttribute("aria-hidden", "true");
            span.InnerHtml.AppendHtml(icon);
            return span;
        }

        // Show Page Link
        // <a href="/action?query"></a>
        private TagBuilder PageLink(
            TagBuilder a,
            int page_action)
        {

            if (String.IsNullOrEmpty(PageController))
            {
                QueryListDict[CurrentPageParameter] = page_action.ToString();
                a.Attributes["href"] = urlHelper.Action(
                     PageAction, new { page = page_action });
            }
            else
            {

                if (!String.IsNullOrEmpty(PageQueryList))
                {

                    foreach (var item in QueryOptions)
                    {
                        QueryListDict[item.Key] = item.Value;
                    }

                }

                QueryListDict[CurrentPageParameter] = page_action.ToString();
                a.Attributes["href"] = urlHelper.Action(
                                     PageAction, PageController,
                                     QueryListDict);
            }

            return a;
        }

        // Show Page Button
        // <li disabled>
        //  <a aria-label="{{ icon }}" herf="">
        //      <span>{{ icon }}</span>
        //  </a>
        // </li>
        private TagBuilder PageButton(
            bool has_link,
            int page_action,
            string icon,
            bool is_disabled,
            bool active_page)
        {
            TagBuilder li = new TagBuilder("li");
            TagBuilder a = new TagBuilder("a");

            a.Attributes["aria-label"] = icon;

            if (has_link)
            {
                a = PageLink(a, page_action);
            }

            a.InnerHtml.AppendHtml(PageIcon(icon));
            li.InnerHtml.AppendHtml(a);

            if (is_disabled)
            {
                li.AddCssClass(PageDisableClass);
            }

            if (active_page)
            {
                a.AddCssClass(PageActiveClass);
                li.AddCssClass(PageActiveClass);
            }

            return li;
        }

        public TagBuilder AddLayers(TagBuilder innter_tag)
        {

            for (int i = 1; i <= NumberOfTagListOuterLayer; i++)
            {
                TagBuilder outer_div = new TagBuilder("div");
                if (HtmlAttributesHelper.IsContainsKey(AttributesTagsListLayerDict, i.ToString()))
                {
                    HtmlAttributesHelper.AddAttributes(outer_div,
                        AttributesTagsListLayerDict,
                        i.ToString());
                }
                outer_div.InnerHtml.AppendHtml(innter_tag);
                innter_tag = outer_div;
            }

            return innter_tag;
        }
    }
}