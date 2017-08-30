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

        public int TotalItems
        {
            get
            {
                return ItemsList.Count;
            }
        }

        public string CurrentPageParameter { get; set; } = "page";

        public string PageQueryList { get; set; }

        public int CurrentPage { get; set; } = 1;

        public int ItemPerPage { get; set; } = 5;

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

        #region Table Properties
        public string TableHeads { get; set; }
        private List<string> TableHeadList
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Ls(TableHeads);
            }
        }

        public string AttributesTable { get; set; }
        private Dictionary<string, string> AttributesTableDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttributesTable);
            }
        }


        public string AttributesTableHead { get; set; }
        private Dictionary<string, string> AttributesTableHeadDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttributesTableHead);
            }
        }

        public string AttributesTableHeadTr { get; set; }
        private Dictionary<string, string> AttributesTableHeadTrDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttributesTableHeadTr);
            }
        }

        public string AttributesTableBody { get; set; }
        private Dictionary<string, string> AttributesTableBodyDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttributesTableBody);
            }
        }

        #endregion

        #region Table Panel
        public bool ActivePanel { get; set; } = true;
        public bool ActivePanelHeading { get; set; } = true;
        public bool ActivePanelBody { get; set; }


        public string AttributesPanel { get; set; }
        private Dictionary<string, string> AttributesPanelDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttributesPanel);
            }
        }

        public string AttributesPanelHeading { get; set; }
        private Dictionary<string, string> AttributesPanelHeadingDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttributesPanelHeading);
            }
        }

        public string AttributesPanelBody { get; set; }
        private Dictionary<string, string> AttributesPanelBodyDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttributesPanelBody);
            }
        }

        public string PanelTitle { get; set; }
        public string PanelContent { get; set; }
        #endregion

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {

            TagBuilder thead = new TagBuilder("thead");
            HtmlAttributesHelper.AddAttributes(thead, AttributesTableHeadDict);

            TagBuilder thead_tr = new TagBuilder("tr");
            HtmlAttributesHelper.AddAttributes(thead_tr, AttributesTableHeadTrDict);

            foreach (var name in TableHeadList)
            {
                TagBuilder th = new TagBuilder("th");
                th.InnerHtml.AppendHtml(name);
                thead_tr.InnerHtml.AppendHtml(th);
            }

            thead.InnerHtml.AppendHtml(thead_tr);
            //output.Content.AppendHtml(thead);

            TagBuilder tbody = new TagBuilder("tbody");
            HtmlAttributesHelper.AddAttributes(tbody, AttributesTableBodyDict);


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
                    HtmlAttributesHelper.AddAttributes(tbody_tr, AttributesTableBodyDict);

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

            if (ActivePanel)
            {

                TagBuilder panel_heading = new TagBuilder("div");
                HtmlAttributesHelper.AddAttributes(panel_heading, AttributesPanelHeadingDict);

                TagBuilder panel_body = new TagBuilder("div");
                HtmlAttributesHelper.AddAttributes(panel_body, AttributesPanelBodyDict);



                if (ActivePanelHeading)
                {
                    output.Content.AppendHtml(panel_heading);
                    panel_heading.AddCssClass("panel-heading");
                    HtmlAttributesHelper.AddAttributes(
                        panel_heading, AttributesPanelHeadingDict);
                    panel_heading.InnerHtml.AppendHtml(PanelTitle);
                }
                if (ActivePanelBody)
                {
                    output.Content.AppendHtml(panel_body);
                    panel_body.AddCssClass("panel-body");
                    HtmlAttributesHelper.AddAttributes(
                       panel_body, AttributesPanelBodyDict);
                    panel_body.InnerHtml.AppendHtml(PanelContent);
                }

                TagBuilder table = new TagBuilder("table");
                table.AddCssClass("table table-primary");
                HtmlAttributesHelper.AddAttributes(
                   table, AttributesTableDict);

                table.InnerHtml.AppendHtml(thead);
                table.InnerHtml.AppendHtml(tbody);

                output.Content.AppendHtml(table);

                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                SetPagination(output);
            }
            else
            {
                output.TagName = "table";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.AppendHtml(tbody);
                SetPagination(output);
            }

        }

        private void SetPagination(TagHelperOutput output)
        {
            if (ActivePagination)
            {
                PaginationBuilder paginaionBuilder = new PaginationBuilder(
                    ViewContext, urlHelperFactory);

                Mapper.Initialize(cfg =>
                {
                    cfg.CreateMap<GenericTableTagHelper, PaginationBuilder>();
                });
                Mapper.Map<GenericTableTagHelper, PaginationBuilder>(this, paginaionBuilder);
                output.PostElement.AppendHtml(paginaionBuilder.CreatePagination());
            }
        }
    }
}
