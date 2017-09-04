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
using System.Collections;
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
        private IHtmlGenerator Generator;
        public GenericTableTagHelper(
            IUrlHelperFactory urlHelperFactory,
            IHtmlGenerator generator)
        {
            this.urlHelperFactory = urlHelperFactory;
            this.Generator = generator;
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


        // Show how many columns of table
        public int TableColumns { get; set; }

        // Append Html string to  specific column
        public string TableAppendHtmlCols { get; set; }
        private Dictionary<string, string> TableAppendHtmlColsDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(TableAppendHtmlCols);
            }
        }

        // Append json tag format to specific column
        public string TableAppendTagsCols { get; set; }
        private Dictionary<string, List<List<Dictionary<string, string>>>> TableAppendTagsColsDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsLLDss(TableAppendTagsCols);
            }
        }

        // Append html string to specific td location
        public string TableAppendHtmlRowsCols { get; set; }
        private Dictionary<string, List<Dictionary<string, string>>> TableAppendHtmlRowsColsDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsLDss(TableAppendHtmlRowsCols);
            }
        }


        // Append json tag format to specific td location
        public string TableAppendTagsRowsCols { get; set; }
        private Dictionary<string, List<Dictionary<string, List<List<Dictionary<string, string>>>>>>
            TableAppendTagsRowsColsDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsLDsLLDss(TableAppendTagsRowsCols);
            }
        }

        public string TableHiddenColumns { get; set; }
        private List<string> TableHiddenColumnsList
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Ls(TableHiddenColumns);
            }
        }


        public bool TableShowIndex { get; set; }
        public string TableIndexTitle { get; set; } = "Index";

        // Set primarykey from your columns of table
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
            if (TableShowIndex)
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
                    if (TableShowIndex)
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
                            // Get hidden columns name
                            var item_key = ItemsAfterPagination[rows]
                                .Keys.ElementAt(columns);
                            // Skip hidden columns
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

                            AddHtmlRowsAndCols(td, row_Id, columns);
                            AddHtmlCols(td, columns);
                            AddTagsRowsAndCols(td, row_Id, columns);
                            AddTagsCols(td, row_Id, columns);


                        }
                        // if out of index then catch exception to create new columns
                        catch (ArgumentOutOfRangeException)
                        {
                            var row_Id = ItemsAfterPagination[rows]
                                .FirstOrDefault(k => k.Key == TablePrimaryKey).Value;

                            AddHtmlRowsAndCols(td, row_Id, columns);
                            AddHtmlCols(td, columns);
                            AddTagsRowsAndCols(td, row_Id, columns);
                            AddTagsCols(td, row_Id, columns);
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
                    panel_heading = SetPanelElement(
                        panel_heading, "panel-heading",
                        AttributesPanelHeadingDict,
                        PanelTitle);
                }
                if (ActivePanelBody)
                {
                    output.Content.AppendHtml(panel_body); 
                    panel_body = SetPanelElement(
                         panel_body, "panel-body",
                         AttributesPanelBodyDict,
                         PanelContent);
                }

                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.AppendHtml(
                    SetTableOutput(output, thead, tbody)
                    );
                SetPagination(output);
            }
            else
            {
                output.TagName = "table";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.AppendHtml(
                    SetTableOutput(output, thead, tbody)
                    );
                SetPagination(output);
            }

        }

        private TagBuilder SetTableOutput(
            TagHelperOutput output, TagBuilder thead, TagBuilder tbody)
        {
            TagBuilder table = new TagBuilder("table");
            table.Attributes["class"] = "table table-primary";

            HtmlAttributesHelper.AddAttributes(
               table, AttrsTableDict);

            table.InnerHtml.AppendHtml(thead);
            table.InnerHtml.AppendHtml(tbody);

            return table;
        }

        private TagBuilder SetPanelElement(
            TagBuilder panel_element,
            string elementClass, 
            Dictionary<string, string> itemAttrsDict,
            string appendContent)
        {
            panel_element.AddCssClass(elementClass);
            HtmlAttributesHelper.AddAttributes(
                panel_element, itemAttrsDict);
            panel_element.InnerHtml.AppendHtml(appendContent);
            return panel_element;
        }

        private void AddHtmlRowsAndCols(TagBuilder td, string row_Id, int columns)
        {
            // Add html content to specific row and cols
            if (HtmlAttributesHelper.IsContainsKey(
                TableAppendHtmlRowsColsDict, row_Id))
            {
                TableAppendHtmlRowsColsDict.FirstOrDefault(
                    items => items.Key.Equals(row_Id,
                    StringComparison.OrdinalIgnoreCase))
                    .Value.ForEach(cols =>
                    {
                        cols.ToDictionary(itemDict =>
                        {

                            if (itemDict.Key.Equals((columns + 1).ToString(),
                                StringComparison.OrdinalIgnoreCase))
                            {
                                td.InnerHtml.AppendHtml(itemDict.Value);
                            }
                            return td;
                        });
                    });
            }
        }

        private void AddHtmlCols(TagBuilder td, int columns)
        {
            if (HtmlAttributesHelper.IsContainsKey(
                               TableAppendHtmlColsDict, (columns + 1).ToString()))
            {
                var htmlString = TableAppendHtmlColsDict.FirstOrDefault(
                    cols => cols.Key.Equals(
                        (columns + 1).ToString(), StringComparison.OrdinalIgnoreCase))
                    .Value;
                td.InnerHtml.AppendHtml(htmlString);
            }
        }

        private void AddTagsRowsAndCols(TagBuilder td, string row_Id, int columns)
        {
            if (HtmlAttributesHelper.IsContainsKey(
                              TableAppendTagsRowsColsDict, row_Id))
            {
                TableAppendTagsRowsColsDict.FirstOrDefault(
                    items => items.Key.Equals(row_Id,
                    StringComparison.OrdinalIgnoreCase))
                    .Value.ForEach(cols =>
                    {
                        cols.ToDictionary(itemDict =>
                        {
                            if (itemDict.Key.Equals((columns + 1).ToString(),
                                StringComparison.OrdinalIgnoreCase))
                            {
                                //colsData.Value.ForEach(tag_attr =>
                                for (int tags_num = 0; tags_num < itemDict.Value.Count; tags_num++)
                                {
                                    var tag_attrs_total = itemDict.Value.ElementAt(tags_num);

                                    // first attrs tagName and tagContent
                                    var tagName = tag_attrs_total.ElementAt(0).ElementAt(0);

                                    // Build Tag
                                    TagBuilder tag = new TagBuilder(tagName.Key);

                                    AddTagContentWithRowId(tagName, tag, row_Id);

                                    tag = AddTagsToTableData(
                                        tagName,
                                        tag_attrs_total,
                                        tag,
                                        row_Id);
                                    td.InnerHtml.AppendHtml(tag);
                                }
                            }
                            return td;
                        });
                    });
            }
        }

        private void AddTagsCols(TagBuilder td, string row_Id, int columns)
        {
            if (HtmlAttributesHelper.IsContainsKey(
                          TableAppendTagsColsDict, (columns + 1).ToString()))
            {
                // { 1:[[{b,c},{d,e}]], 2:[[{f,g},{h,g}]] }
                for (int cols = 0; cols < TableAppendTagsColsDict.Count; cols++)
                {
                    // { 1: [[{b,c},{d,e}]] } get cols data
                    var colsData = TableAppendTagsColsDict.ElementAt(cols);
                    // if data.key = column_num 
                    if (colsData.Key == (columns + 1).ToString())
                    {
                        // [[{b,c},{d,e}]]  numbers of tags loop
                        for (int tags_num = 0; tags_num < colsData.Value.Count; tags_num++)
                        {

                            var tag_attrs_total = colsData.Value.ElementAt(tags_num);

                            // first attrs tagName and tagContent
                            var tagName = tag_attrs_total.ElementAt(0).ElementAt(0);


                            // Build Tag
                            TagBuilder tag = new TagBuilder(tagName.Key);

                            AddTagContentWithRowId(tagName, tag, row_Id);


                            tag = AddTagsToTableData(
                                tagName,
                                tag_attrs_total,
                                tag,
                                row_Id);
                            td.InnerHtml.AppendHtml(tag);
                        }
                    }
                }
            }
        }

        private TagBuilder AddTagsToTableData(
            KeyValuePair<string, string> tagName,
            List<Dictionary<string, string>> tag_attrs_total,
            TagBuilder tag,
            string row_Id)
        {
            if (tagName.Key == "input")
            {
                for (int attrs_num = 1; attrs_num < tag_attrs_total.Count; attrs_num++)
                {

                    tag = AddRowIdWith_(
                       tag, tag_attrs_total, attrs_num, row_Id);
                }
            }
            else if (tagName.Key == "a")
            {


                // second attrs action and controller
                var actionController = tag_attrs_total.ElementAt(1).ElementAt(0);

                Dictionary<string, string> query = new Dictionary<string, string>
                {
                    [TablePrimaryKey] = row_Id
                };

                //var link_query = tag_attrs_total.ElementAt(0).ElementAt(2);
                tag.Attributes["href"] = urlHelper.Action(
                    actionController.Key,
                    actionController.Value,
                    query);

                for (int attrs_num = 2; attrs_num < tag_attrs_total.Count; attrs_num++)
                {

                    tag = AddRowIdWith_(
                       tag, tag_attrs_total, attrs_num, row_Id);
                }
            }
            else if (tagName.Key == "select")
            {
                //tag.InnerHtml.AppendHtml(tagName.Value);

                for (int attrs_num = 1; attrs_num < tag_attrs_total.Count; attrs_num++)
                {
                    tag = AddRowIdWith_(
                        tag, tag_attrs_total, attrs_num, row_Id);
                }
            }
            else if (tagName.Key == "label")
            {


                for (int attrs_num = 1; attrs_num < tag_attrs_total.Count; attrs_num++)
                {
                    tag = AddRowIdWith_(
                       tag, tag_attrs_total, attrs_num, row_Id);
                }
            }
            else
            {
                //if (tagName.Value.EndsWith("_"))
                //{
                //    tag.InnerHtml.AppendHtml(tagName.Value + row_Id);
                //}
                //else
                //{
                //    tag.InnerHtml.AppendHtml(tagName.Value);
                //}
                // skip first attr and loop attrs element  
                for (int attrs_num = 1; attrs_num < tag_attrs_total.Count; attrs_num++)
                {
                    tag = AddRowIdWith_(
                        tag, tag_attrs_total, attrs_num, row_Id);
                }
            }

            return tag;
        }

        private void AddTagContentWithRowId(
            KeyValuePair<string, string> tagName,
            TagBuilder tag,
            string row_Id)
        {
            if (tagName.Value.EndsWith("_"))
            {
                var tagValue = tagName.Value.TrimEnd('_');
                tag.InnerHtml.AppendHtml(tagValue + row_Id);
            }
            else
            {
                tag.InnerHtml.AppendHtml(tagName.Value);
            }
        }

        private TagBuilder AddRowIdWith_(
            TagBuilder tag,
            List<Dictionary<string, string>> tag_attrs_total,
            int attrs_num,
            string row_Id)
        {

            var attr = tag_attrs_total.ElementAt(attrs_num).ElementAt(0);

            // Distinguish string is number
            if (tag.TagName == "select")
            {
                int number;
                bool isNumberic = int.TryParse(attr.Key, out number);
                if (isNumberic)
                {
                    TagBuilder option = new TagBuilder("option");
                    option.Attributes["value"] = attr.Key;
                    option.InnerHtml.AppendHtml(attr.Value);
                    tag.InnerHtml.AppendHtml(option);
                }
                else if (attr.Value.EndsWith("_"))
                {
                    var attrValue = attr.Value.TrimEnd('_');
                    tag.Attributes[attr.Key] = attrValue + row_Id;
                }
                else
                {
                    tag.Attributes[attr.Key] = attr.Value;
                }
            }
            else
            {
                if (attr.Value.EndsWith("_"))
                {
                    var attrValue = attr.Value.TrimEnd('_');
                    tag.Attributes[attr.Key] = attrValue + row_Id;
                }
                else
                {
                    tag.Attributes[attr.Key] = attr.Value;
                }
            }

            return tag;
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
