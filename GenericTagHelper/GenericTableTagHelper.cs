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
using System.Text.RegularExpressions;

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

        public string AttrsTable { get; set; }
        private Dictionary<string, string> AttrsTableDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsTable);
            }
        }


        public string AttrsTableHead { get; set; }
        private Dictionary<string, string> AttrsTableHeadDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsTableHead);
            }
        }

        public string AttrsTableHeadTr { get; set; }
        private Dictionary<string, string> AttrsTableHeadTrDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsTableHeadTr);
            }
        }

        public string AttrsTableBody { get; set; }
        private Dictionary<string, string> AttrsTableBodyDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsTableBody);
            }
        }


        public int TableColumns { get; set; }

        public string TableAllColumnContent { get; set; }
        // { columnes: items }
        private Dictionary<string, string> TableAllColumnContentDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(TableAllColumnContent);
            }
        }

        public string TableContent { get; set; }
        // { rows:{ columnes: item}}
        private Dictionary<string, Dictionary<string, string>> TableContentDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(TableContent);
            }
        }

        public  string TableHiddenColumns { get; set; }
        private List<string> TableHiddenColumnsList
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Ls(TableHiddenColumns);
            }
        }

        public bool ShowTableIndex { get; set; }
        public string TableIndexTitle { get; set; } = "Index";

        public string TablePrimaryKey { get; set; } = "Id";
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
            HtmlAttributesHelper.AddAttributes(thead, AttrsTableHeadDict);

            TagBuilder thead_tr = new TagBuilder("tr");
            HtmlAttributesHelper.AddAttributes(thead_tr, AttrsTableHeadTrDict);
            if (ShowTableIndex)
            {
                TagBuilder th = new TagBuilder("th");
                th.InnerHtml.AppendHtml(TableIndexTitle);
                thead_tr.InnerHtml.AppendHtml(th);
            }

            foreach (var name in TableHeadList)
            {
                TagBuilder th = new TagBuilder("th");
                th.InnerHtml.AppendHtml(name);
                thead_tr.InnerHtml.AppendHtml(th);
            }

            thead.InnerHtml.AppendHtml(thead_tr);


            TagBuilder tbody = new TagBuilder("tbody");
            HtmlAttributesHelper.AddAttributes(tbody, AttrsTableBodyDict);


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
                if (TableColumns == 0)
                {
                    TableColumns = ItemsAfterPagination[0].Values.Count;
                }



                // Start loop table row 
                for (int rows = 0; rows < ItemsAfterPagination.Count; rows++)
                {
                    TagBuilder tbody_tr = new TagBuilder("tr");
                    HtmlAttributesHelper.AddAttributes(tbody_tr, AttrsTableBodyDict);
                    // Show index if true
                    if (ShowTableIndex)
                    {
                        TagBuilder Index = new TagBuilder("td");
                        Index.InnerHtml.AppendHtml(
                            (rows + 1 + (ItemPerPage * (CurrentPage - 1)))
                            .ToString());
                        tbody_tr.InnerHtml.AppendHtml(Index);
                    }

                    // Start loop table columns
                    for (int columns = 0; columns < TableColumns; columns++)
                    {
                        TagBuilder td = new TagBuilder("td");

                        // try to get value from 
                        try
                        {
                            var item_key = ItemsAfterPagination[rows]
                                .Keys.ElementAt(columns);
                            if (TableHiddenColumnsList.Contains(item_key))
                            {
                                continue;
                            }
                            var item_value = ItemsAfterPagination[rows]
                                .Values.ElementAt(columns);

                            td.InnerHtml.AppendHtml(item_value);

                            // Get your table list primary key value
                            var row_Id = ItemsAfterPagination[rows]
                                .FirstOrDefault(k => k.Key == TablePrimaryKey).Value;

                            if (HtmlAttributesHelper.IsContainsKey(
                                TableContentDict, row_Id))
                            {
                                TableContentDict.FirstOrDefault(
                                    items => items.Key.Equals(row_Id,
                                    StringComparison.OrdinalIgnoreCase))
                                    .Value
                                    .ToDictionary(cols =>
                                    {
                                        if (cols.Key.Equals((columns + 1).ToString(),
                                            StringComparison.OrdinalIgnoreCase))
                                        {
                                            td.InnerHtml.AppendHtml(cols.Value);
                                        }
                                        return td;
                                    });
                            }

                            if (HtmlAttributesHelper.IsContainsKey(
                                TableAllColumnContentDict, (columns + 1).ToString()))
                            {
                                //TableAllColumnContentDict.FirstOrDefault(
                                //items => items.Key.Equals((columns + 1).ToString())).
                                var value = TableAllColumnContentDict[(columns + 1).ToString()];
                                td.InnerHtml.AppendHtml(value);
                            }

                        }
                        // if out of index then catch exception to create new columns
                        catch (ArgumentOutOfRangeException)
                        {
                            var row_Id = ItemsAfterPagination[rows]
                                .FirstOrDefault(k => k.Key == TablePrimaryKey).Value;

                            if (HtmlAttributesHelper.IsContainsKey(TableContentDict, row_Id))
                            {
                                TableContentDict.FirstOrDefault(
                                    items => items.Key.Equals(row_Id, StringComparison.OrdinalIgnoreCase))
                                    .Value
                                    .ToDictionary(i =>
                                    {
                                        if (i.Key.Equals((columns + 1).ToString(), StringComparison.OrdinalIgnoreCase))
                                        {
                                            td.InnerHtml.AppendHtml(i.Value);
                                        }
                                        return td;
                                    });
                            }

                            if (HtmlAttributesHelper.IsContainsKey(
                               TableAllColumnContentDict, (columns + 1).ToString()))
                            {

                                // Get columns value
                                var value = TableAllColumnContentDict[(columns + 1).ToString()];

                                string[] valueArray = value.Split('|').ToArray();
                                TagBuilder a = new TagBuilder(valueArray[0]);
                                a.InnerHtml.AppendHtml(valueArray[1].ToString());
                                a.Attributes["class"] = valueArray[2];
                                var query = new Dictionary<string, string>
                                {
                                    [valueArray[5]] = row_Id
                                };
                                a.Attributes["href"] = urlHelper.Action(valueArray[3].ToString(), valueArray[4].ToString(), query);
                                //if (value.StartsWith("<a") && value.EndsWith("</a>"))
                                //{
                                //    Regex find_controller_regex = new Regex(@"(?<=\bcontroller="")[^""]*");
                                //    Regex find_action_regex = new Regex(@"(?<=\baction="")[^""]*");
                                //    Match controller_match = find_controller_regex.Match(value);
                                //    Match action_match = find_action_regex.Match(value);

                                //    string controller = controller_match.Value;
                                //    string action = action_match.Value;
                                //    var url= urlHelper.Action(action, controller,new { Id = row_Id});
                                //    value = "<a href=\"" + url + "\">Edit</a>";
                                //}

                                td.InnerHtml.AppendHtml(a);
                            }


                        }


                        tbody_tr.InnerHtml.AppendHtml(td);
                    }
                    tbody.InnerHtml.AppendHtml(tbody_tr);
                }


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
                   table, AttrsTableDict);

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
