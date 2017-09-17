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
using System.Linq.Expressions;
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

        public int CurrentPage { get; set; } = 1;

        public int ItemPerPage { get; set; } = 5;
        #endregion

        #region Table Attributes
        public string AttrsTag { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttrsTagDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttrsTag);
            }
        }
        #endregion

        #region Table Content
        public string ContentTag { get; set; }
        private Dictionary<string, string> ContentTagDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(ContentTag);
            }
        }
        #endregion

        #region AttrsTags
        public string TagTable { get; set; } = "table";

        public string TagTableTitle { get; set; } = "title";
        public string TagTableHead { get; set; } = "thead";
        public string TagTableHeadRow { get; set; } = "thead_row";
        public string TagTableHeadIndex { get; set; } = "thead_index";

        public string TagTableBody { get; set; } = "tbody";
        public string TagTableBodyRow { get; set; } = "tbody_row";

        public string TagTableBodyTd { get; set; } = "td";
        public string TagTableBodyTdRow { get; set; } = "td_row";
        public string TagTableBodyTdCol { get; set; } = "td_col";
        public string TagTableTdIndex { get; set; } = "td_index";

        public string TagTableBodyTdO { get; set; } = "td_o";
        public string TagTableBodyTdRowO { get; set; } = "td_row_o";
        public string TagTableBodyTdColO { get; set; } = "td_col_o";
        public string TagTableTdIndexO { get; set; } = "td_index_o";

        public string TagTablePanelHead { get; set; } = "panel_head";
        public string TagTablePanelBody { get; set; } = "panel_body";
        public string TagTablePanelHeadO { get; set; } = "panel_head_o";
        public string TagTablePanelBodyO { get; set; } = "panel_body_o";

        #endregion

        #region Table Properties

        public string TableSortData { get; set; }
        private List<string> TableSortDataList
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Ls(TableSortData);
            }
        }

        // Show how many columns of table
        public int TableColsNumbers { get; set; }

        public string TableHeadDatas { get; set; }
        private List<string> TableHeadDataList
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Ls(TableHeadDatas);
            }
        }

        public bool TableShowIndex { get; set; }
        public string TableIndexTitle { get; set; } = "Index";

        // Set primarykey from your columns of table
        public string TablePrimaryKey { get; set; } = "Id";
        #endregion

        #region Table Panel
        public bool DisablePanel { get; set; }
        public bool DisablePanelHead { get; set; }
        public bool DisablePanelBody { get; set; }
        #endregion

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {

            TagBuilder thead = new TagBuilder("thead");
            SetAttrsAndContent(AttrsTagDict, ContentTagDict, thead, TagTableHead, false);

            TagBuilder thead_tr = new TagBuilder("tr");
            SetAttrsAndContent(AttrsTagDict, ContentTagDict, thead_tr, TagTableHeadRow, false);
            // Active Index
            if (TableShowIndex)
            {
                TagBuilder th = new TagBuilder("th");
                th.InnerHtml.AppendHtml(TableIndexTitle);
                SetAttrsAndContent(AttrsTagDict, ContentTagDict, th, TagTableHeadIndex, false);

                thead_tr.InnerHtml.AppendHtml(th);
            }

            foreach (var name in TableHeadDataList)
            {
                TagBuilder th = new TagBuilder("th");
                th.InnerHtml.AppendHtml(name);
                thead_tr.InnerHtml.AppendHtml(th);
            }

            thead.InnerHtml.AppendHtml(thead_tr);

            TagBuilder tbody = new TagBuilder("tbody");
            SetAttrsAndContent(AttrsTagDict, ContentTagDict, tbody, TagTableBody, false);

            if (ItemsList.Count == 0)
            {
                TagBuilder tbody_tr = new TagBuilder("tr");
                SetAttrsAndContent(
                    AttrsTagDict, ContentTagDict, tbody_tr, TagTableBodyRow, false);

                TagBuilder td = new TagBuilder("td");
                SetAttrsAndContent(
                    AttrsTagDict, ContentTagDict, td, TagTableBodyTd, false);

                td.InnerHtml.AppendHtml(NoItemsMessage);

                tbody_tr.InnerHtml.AppendHtml(td);

                tbody.InnerHtml.AppendHtml(tbody_tr);
            }
            else
            {
                if (TableColsNumbers == 0)
                {
                    TableColsNumbers = ItemsList[0].Values.Count;
                }

                // Start loop table row 
                for (int rows = 0; rows < ItemsList.Count; rows++)
                {
                    var primary_key = ItemsList[rows]
                              .FirstOrDefault(k => k.Key == TablePrimaryKey).Value;

                    var rows_index =
                        (rows + 1 + (ItemPerPage * (CurrentPage - 1))).ToString();

                    TagBuilder tbody_tr = new TagBuilder("tr");

                    AttrsHelper.SetTagAttrs(
                        AttrsTagDict, tbody_tr, TagTableBodyRow);
                    AttrsHelper.SetTagAttrs(
                        AttrsTagDict, tbody_tr,
                        TagTableBodyRow, rows_index);

                    AttrsHelper.SetTagContent(
                        ContentTagDict, tbody_tr, TagTableBodyRow, false);
                    AttrsHelper.SetTagContent(
                        ContentTagDict, tbody_tr, TagTableBodyRow, rows_index, false);

                    // Show index if true
                    if (TableShowIndex)
                    {
                        TagBuilder index = new TagBuilder("td");

                        AttrsHelper.SetTagAttrs(
                            AttrsTagDict, index, TagTableTdIndex);
                        AttrsHelper.SetTagAttrs(
                            AttrsTagDict, index, TagTableTdIndex, rows_index);

                        AttrsHelper.SetTagContent(
                            ContentTagDict, index, TagTableTdIndex, false);
                        AttrsHelper.SetTagContent(
                            ContentTagDict, index, TagTableTdIndex, rows_index, false);

                        index.InnerHtml.AppendHtml(rows_index);

                        tbody_tr.InnerHtml.AppendHtml(index);
                    }

                    // Start loop table columns
                    for (int cols = 0; cols < TableColsNumbers; cols++)
                    {
                        var cols_index = (cols + 1).ToString();

                        TagBuilder td = new TagBuilder("td");

                        // try to get value from 
                        try
                        {
                            if (TableSortDataList.Count != 0)
                            {
                                var tableData = ItemsList[rows].FirstOrDefault(
                                    d => d.Key.Equals(TableSortDataList[cols],
                                    StringComparison.OrdinalIgnoreCase)).Value;
                                if (tableData != null)
                                {
                                    td.InnerHtml.AppendHtml(tableData);
                                }
                            }
                            else
                            {
                                var tableData = ItemsList[rows].Values.ElementAt(cols);
                                td.InnerHtml.AppendHtml(tableData);
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                        }

                        AttrsHelper.SetTagAttrs(
                            AttrsTagDict, td, TagTableBodyTd);
                        AttrsHelper.SetTagRowsOrColsAttrs(
                            AttrsTagDict, td, TagTableBodyTdRow, primary_key, rows_index);
                        AttrsHelper.SetTagRowsOrColsAttrs(
                            AttrsTagDict, td, TagTableBodyTdCol, primary_key, cols_index);
                        AttrsHelper.SetTagAttrs(
                            AttrsTagDict, td, TagTableBodyTd, rows_index, cols_index);

                        AttrsHelper.SetTagContent(
                            ContentTagDict, td, TagTableBodyTd, true);
                        AttrsHelper.SetTagRowsOrColsContent(
                            ContentTagDict, td, TagTableBodyTdRow, primary_key, rows_index, true);
                        AttrsHelper.SetTagRowsOrColsContent(
                            ContentTagDict, td, TagTableBodyTdCol, primary_key, cols_index, true);
                        AttrsHelper.SetTagContent(
                            ContentTagDict, td, TagTableBodyTd, rows_index, cols_index, true);

                        AttrsHelper.SetTagContent(
                            ContentTagDict, td, TagTableBodyTdO, false);
                        AttrsHelper.SetTagRowsOrColsContent(
                            ContentTagDict, td, TagTableBodyTdRowO, primary_key, rows_index, false);
                        AttrsHelper.SetTagRowsOrColsContent(
                            ContentTagDict, td, TagTableBodyTdColO, primary_key, cols_index, false);
                        AttrsHelper.SetTagContent(
                            ContentTagDict, td, TagTableBodyTdO, rows_index, cols_index, false);
                        tbody_tr.InnerHtml.AppendHtml(td);
                    }
                    tbody.InnerHtml.AppendHtml(tbody_tr);
                }

            }

            if (!DisablePanel)
            {

                if (!DisablePanelHead)
                {
                    TagBuilder panel_head = new TagBuilder("div");

                    AttrsHelper.SetTagAttrs(AttrsTagDict, panel_head, TagTablePanelHead);
                    AttrsHelper.SetTagContent(ContentTagDict, panel_head, TagTablePanelHead, false);
                    AttrsHelper.SetTagContent(ContentTagDict, panel_head, TagTablePanelHeadO, true);

                    output.Content.AppendHtml(panel_head);
                }
                if (!DisablePanelBody)
                {
                    TagBuilder panel_body = new TagBuilder("div");

                    AttrsHelper.SetTagAttrs(AttrsTagDict, panel_body, TagTablePanelBody);
                    AttrsHelper.SetTagContent(ContentTagDict, panel_body, TagTablePanelBody, false);
                    AttrsHelper.SetTagContent(ContentTagDict, panel_body, TagTablePanelBodyO, true);

                    output.Content.AppendHtml(panel_body);
                }

                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.AppendHtml(
                    SetTableOutput(output, thead, tbody));
            }
            else
            {
                output.TagName = "table";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.AppendHtml(
                    SetTableOutput(output, thead, tbody)
                    );
            }

        }

        private void SetAttrsAndContent(
            Dictionary<string, Dictionary<string, string>> attrDict,
            Dictionary<string, string> contentDict,
            TagBuilder tag, string tag_name, bool disable_override)
        {
            AttrsHelper.SetTagAttrs(
                    attrDict, tag, tag_name);
            AttrsHelper.SetTagContent(
                contentDict, tag, tag_name, disable_override);
        }



        private TagBuilder SetTableOutput(
            TagHelperOutput output, TagBuilder thead, TagBuilder tbody)
        {
            TagBuilder table = new TagBuilder("table");
            table.Attributes["class"] = "table table-primary";

            AttrsHelper.SetTagAttrs(AttrsTagDict, table, TagTable);
            table.InnerHtml.AppendHtml(thead);
            table.InnerHtml.AppendHtml(tbody);

            return table;
        }
    }
}
