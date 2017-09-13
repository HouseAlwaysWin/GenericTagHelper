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


        public string AttrsTagsOfProp { get; set; }
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> AttrsTagsOfPropDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDssDss(AttrsTagsOfProp);
            }
        }

        public string AttrsTag { get; set; }
        public Dictionary<string, Dictionary<string, string>> AttrsTagDict
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
        public string TagTableHead { get; set; } = "head";
        public string TagTableHeadRow { get; set; } = "head_row";
        public string TagTableIndexHead { get; set; } = "index_head";

        public string TagTableBody { get; set; } = "body";
        public string TagTableBodyRow { get; set; } = "body_row";

        public string TagTableBodyData { get; set; } = "body_data";
        public string TagTableBodyDataRow { get; set; } = "body_data_row";
        public string TagTableBodyDataCol { get; set; } = "body_data_col";
        public string TagTableIndexData { get; set; } = "index_data";

        public string TagTablePanel { get; set; } = "panel";
        public string TagTablePanelHead { get; set; } = "panel_head";
        public string TagTablePanelBody { get; set; } = "panel_body";

        public string TagTableData { get; set; } = "data";
        public string TagTableDatatRow { get; set; } = "data_row";
        public string TagTableDataCol { get; set; } = "data_col";

        public string TagTableDataOverride { get; set; } = "data_override";
        public string TagTableDataOverrideRow { get; set; } = "data_override_row";
        public string TagTableDataOverrideCol { get; set; } = "data_override_col";

        #endregion


        #region Table Properties

        public string TableSortData { get; set; }
        public List<string> TableSortDataList
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Ls(TableSortData);
            }
        }

        // Show how many columns of table
        public int TableColsNumber { get; set; }

        public string TableHeadData { get; set; }
        private List<string> TableHeadDataList
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Ls(TableHeadData);
            }
        }

        // Append Html string to  specific column
        public string TableAppendHtmlCols { get; set; }
        private Dictionary<string, string> TableAppendHtmlColsDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(TableAppendHtmlCols);
            }
        }
        public bool ActiveOverrideHtmlCols { get; set; }

        // Append html string to specific td location
        public string TableAppendHtmlRowsCols { get; set; }
        private Dictionary<string, Dictionary<string, string>> TableAppendHtmlRowsColsDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(TableAppendHtmlRowsCols);
            }
        }
        public bool ActiveOverrideHtmlRowsCols { get; set; }

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


        public string AttrsPanel { get; set; }
        private Dictionary<string, string> AttrsPanelDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsPanel);
            }
        }

        public string AttrsPanelHeading { get; set; }
        private Dictionary<string, string> AttrsPanelHeadingDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsPanelHeading);
            }
        }

        public string AttrsPanelBody { get; set; }
        private Dictionary<string, string> AttrsPanelBodyDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsPanelBody);
            }
        }

        public string PanelHeadingContent { get; set; }
        public string PanelBodyContent { get; set; }
        #endregion

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {

            TagBuilder thead = new TagBuilder("thead");
            AttrsHelper.SetTagAttrs(
                AttrsTagDict, thead, TagTableHead);

            //HtmlAttributesHelper.AddAttributes(thead, AttrsTableHeadDict);

            TagBuilder thead_tr = new TagBuilder("tr");
            AttrsHelper.SetTagAttrs(
                AttrsTagDict, thead_tr, TagTableHeadRow);
            //HtmlAttributesHelper.AddAttributes(thead_tr, AttrsTableHeadTrDict);

            // Active Index
            if (TableShowIndex)
            {
                TagBuilder th = new TagBuilder("th");
                AttrsHelper.SetTagAttrs(
                    AttrsTagDict, th, TagTableIndexHead);

                th.InnerHtml.AppendHtml(TableIndexTitle);
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
            AttrsHelper.SetTagAttrs(
                AttrsTagDict, tbody, TagTableBody);
            //HtmlAttributesHelper.AddAttributes(tbody, AttrsTableBodyDict);



            if (ItemsList.Count == 0)
            {
                TagBuilder tbody_tr = new TagBuilder("tr");

                TagBuilder td = new TagBuilder("td");

                td.InnerHtml.AppendHtml(NoItemsMessage);

                tbody_tr.InnerHtml.AppendHtml(td);

                tbody.InnerHtml.AppendHtml(tbody_tr);
            }
            else
            {
                if (TableColsNumber == 0)
                {
                    TableColsNumber = ItemsList[0].Values.Count;
                }

                // Start loop table row 
                for (int rows = 0; rows < ItemsList.Count; rows++)
                {
                    var rows_index =
                        (rows + 1 + (ItemPerPage * (CurrentPage - 1))).ToString();

                    TagBuilder tbody_tr = new TagBuilder("tr");
                    AttrsHelper.SetTagAttrs(
                        AttrsTagDict, tbody_tr, TagTableBodyRow);

                    AttrsHelper.SetTagAttrs(
                        AttrsTagDict, tbody_tr,
                        TagTableBodyRow, rows_index);

                    // Show index if true
                    if (TableShowIndex)
                    {
                        TagBuilder Index = new TagBuilder("td");

                        AttrsHelper.SetTagAttrs(
                            AttrsTagDict, Index, TagTableIndexData);
                        AttrsHelper.SetTagAttrs(
                            AttrsTagDict, Index, TagTableIndexData, rows_index);

                        Index.InnerHtml.AppendHtml(rows_index);

                        tbody_tr.InnerHtml.AppendHtml(Index);
                    }

                    // Start loop table columns
                    for (int cols = 0; cols < TableColsNumber; cols++)
                    {
                        var cols_index = (cols + 1).ToString();

                        TagBuilder td = new TagBuilder("td");

                        AttrsHelper.SetTagAttrs(
                            AttrsTagDict, td, TagTableBodyData);
                        AttrsHelper.SetTagAttrs(
                            AttrsTagDict, td, TagTableBodyDataCol, cols_index);
                        AttrsHelper.SetTagAttrs(
                            AttrsTagDict, td, TagTableBodyDataRow, rows_index);
                        AttrsHelper.SetTagAttrs(
                            AttrsTagDict, td, TagTableBodyData, rows_index, cols_index);


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

                            //var hiddenCols = ItemsList[rows]
                            //    .Keys.ElementAt(cols);

                            //// Skip hidden columns
                            //if (TableHiddenColumnsList.Contains(hiddenCols))
                            //{
                            //    continue;
                            //}

                            //var tableData = ItemsList[rows]
                            //    .Values.ElementAt(cols);

                            //td.InnerHtml.AppendHtml(tableData);

                            // Get your table list primary key value
                            //var row_Id = ItemsList[rows]
                            //    .FirstOrDefault(k => k.Key == TablePrimaryKey).Value;

                            //AddHtmlRowsAndCols(td, rows_index, cols_index);
                            //AddHtmlCols(td, rows_index, cols_index);
                            AttrsHelper.SetTagContent(
                               ContentTagDict, td, TagTableData, rows_index,
                               false);

                            AttrsHelper.SetTagContent(
                                ContentTagDict, td, TagTableData,
                                rows_index, cols_index, false);

                            AttrsHelper.SetTagContent(
                                ContentTagDict, td, TagTableDatatRow,
                                rows_index, false);

                            AttrsHelper.SetTagContent(
                                ContentTagDict, td, TagTableDataCol,
                                cols_index, false);

                            AttrsHelper.SetTagContent(
                                ContentTagDict, td, TagTableDataOverride,
                                rows_index, cols_index, true);

                            AttrsHelper.SetTagContent(
                              ContentTagDict, td, TagTableDataOverrideRow,
                              rows_index, true);

                            AttrsHelper.SetTagContent(
                                ContentTagDict, td, TagTableDataOverrideCol,
                                cols_index, true);

                        }
                        // if out of index then catch exception to create new columns
                        catch (ArgumentOutOfRangeException)
                        {
                            //var row_Id = ItemsList[rows]
                            //    .FirstOrDefault(k => k.Key == TablePrimaryKey).Value;

                            //AddHtmlRowsAndCols(td, row_Id, cols_index);
                            //AddHtmlCols(td, row_Id, cols_index);

                            AttrsHelper.SetTagContent(
                                ContentTagDict, td, TagTableData, rows_index,
                                false);

                            AttrsHelper.SetTagContent(
                                ContentTagDict, td, TagTableData,
                                rows_index, cols_index, false);

                            AttrsHelper.SetTagContent(
                                ContentTagDict, td, TagTableDatatRow,
                                rows_index, false);

                            AttrsHelper.SetTagContent(
                                ContentTagDict, td, TagTableDataCol,
                                cols_index, false);

                            AttrsHelper.SetTagContent(
                                ContentTagDict, td, TagTableDataOverride,
                                rows_index, cols_index, true);

                            AttrsHelper.SetTagContent(
                              ContentTagDict, td, TagTableDataOverrideRow,
                              rows_index, true);

                            AttrsHelper.SetTagContent(
                                ContentTagDict, td, TagTableDataOverrideCol,
                                cols_index, true);
                        }


                        tbody_tr.InnerHtml.AppendHtml(td);
                    }
                    tbody.InnerHtml.AppendHtml(tbody_tr);
                }


            }

            if (ActivePanel)
            {

                TagBuilder panel_head = new TagBuilder("div");
                AttrsHelper.SetTagAttrs(AttrsTagDict, panel_head, TagTablePanelHead);
                //HtmlAttributesHelper.AddAttributes(panel_heading, AttrsPanelHeadingDict);

                TagBuilder panel_body = new TagBuilder("div");
                AttrsHelper.SetTagAttrs(AttrsTagDict, panel_head, TagTablePanelBody);
                //HtmlAttributesHelper.AddAttributes(panel_body, AttrsPanelBodyDict);

                if (ActivePanelHeading)
                {
                    output.Content.AppendHtml(panel_head);
                    panel_head = SetPanelElement(
                        panel_head, "panel-heading",
                        AttrsPanelHeadingDict,
                        PanelHeadingContent);
                }
                if (ActivePanelBody)
                {
                    output.Content.AppendHtml(panel_body);
                    panel_body = SetPanelElement(
                         panel_body, "panel-body",
                         AttrsPanelBodyDict,
                         PanelBodyContent);
                }

                output.TagName = "div";
                output.TagMode = TagMode.StartTagAndEndTag;
                output.Content.AppendHtml(
                    SetTableOutput(output, thead, tbody)
                    );
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

        private TagBuilder SetTableOutput(
            TagHelperOutput output, TagBuilder thead, TagBuilder tbody)
        {
            TagBuilder table = new TagBuilder("table");
            table.Attributes["class"] = "table table-primary";

            AttrsHelper.SetTagAttrs(AttrsTagDict, table, TagTable);
            //HtmlAttributesHelper.AddAttributes(
            //   table, AttrsTableDict);

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
            AttrsHelper.SetTagAttrs(AttrsTagDict, panel_element, TagTablePanel);
            //HtmlAttributesHelper.AddAttributes(
            //    panel_element, itemAttrsDict);
            panel_element.InnerHtml.AppendHtml(appendContent);
            return panel_element;
        }

        private void AddHtmlRowsAndCols(TagBuilder td, string row_Id, string cols_index)
        {
            // Add html content to specific row and cols
            var rowsValue = TableAppendHtmlRowsColsDict.FirstOrDefault(
                rows => rows.Key.Equals(row_Id, StringComparison.OrdinalIgnoreCase)).Value;
            if (TableAppendHtmlRowsColsDict.Count != 0 &&
                rowsValue != null)
            {
                var colsValue = rowsValue.FirstOrDefault(
                    cols => cols.Key.Equals(cols_index,
                    StringComparison.OrdinalIgnoreCase)).Value;

                if (colsValue != null)
                {
                    colsValue = colsValue.Replace("*", row_Id);
                }

                if (ActiveOverrideHtmlRowsCols)
                {
                    td.InnerHtml.SetHtmlContent(colsValue);
                }
                else
                {
                    td.InnerHtml.AppendHtml(colsValue);
                }
            }
            //if (HtmlAttributesHelper.IsContainsKey(
            //    TableAppendHtmlRowsColsDict, row_Id))
            //{
            //    var htmlValue = TableAppendHtmlRowsColsDict.FirstOrDefault(
            //        rows => rows.Key.Equals(row_Id,
            //        StringComparison.OrdinalIgnoreCase))
            //        .Value.FirstOrDefault(
            //          cols => cols.Key.Equals((columns + 1).ToString(),
            //          StringComparison.OrdinalIgnoreCase)).Value;

            //    if (htmlValue != null)
            //    {
            //        htmlValue = htmlValue.Replace("*", row_Id);
            //    }


            //    if (ActiveOverrideHtmlRowsCols)
            //    {
            //        td.InnerHtml.SetHtmlContent(htmlValue);
            //    }
            //    else
            //    {
            //        td.InnerHtml.AppendHtml(htmlValue);
            //    }
            //}
        }

        private void AddHtmlCols(TagBuilder td, string row_Id, string cols_index)
        {
            var colsValue = TableAppendHtmlColsDict.FirstOrDefault(
                    cols => cols.Key.Equals(
                            cols_index, StringComparison.OrdinalIgnoreCase)).Value;
            if (colsValue != null &&
                TableAppendHtmlColsDict.Count != 0)
            {
                if (colsValue != null)
                {
                    colsValue = colsValue.Replace("*", row_Id);
                }

                if (ActiveOverrideHtmlRowsCols)
                {
                    td.InnerHtml.SetHtmlContent(colsValue);
                }
                else
                {
                    td.InnerHtml.AppendHtml(colsValue);
                }
            }
            //if (HtmlAttributesHelper.IsContainsKey(
            //                   TableAppendHtmlColsDict, (columns + 1).ToString()))
            //{
            //    var htmlValue = TableAppendHtmlColsDict.FirstOrDefault(
            //        cols => cols.Key.Equals(
            //            (columns + 1).ToString(), StringComparison.OrdinalIgnoreCase))
            //        .Value;


            //    if (htmlValue != null)
            //    {
            //        htmlValue = htmlValue.Replace("*", row_Id);
            //    }


            //    if (ActiveOverrideHtmlCols)
            //    {
            //        td.InnerHtml.SetHtmlContent(htmlValue);
            //    }
            //    else
            //    {
            //        td.InnerHtml.AppendHtml(htmlValue);
            //    }
            //}
        }


    }
}
