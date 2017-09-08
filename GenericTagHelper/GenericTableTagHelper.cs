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

        public string AttrsTableBodyTr { get; set; }
        private Dictionary<string, string> AttrsTableBodyTrDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsTableBodyTr);
            }
        }


        // Show how many columns of table
        public int TableColsNumber { get; set; }

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
            HtmlAttributesHelper.AddAttributes(thead, AttrsTableHeadDict);

            TagBuilder thead_tr = new TagBuilder("tr");
            HtmlAttributesHelper.AddAttributes(thead_tr, AttrsTableHeadTrDict);

            // Active Index
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
                    TagBuilder tbody_tr = new TagBuilder("tr");

                    try
                    {
                        AttrsTableBodyTrDict.ToDictionary(attr =>
                        {
                            if (attr.Value.EndsWith("_"))
                            {
                                var attrValue = attr.Value.TrimEnd('_');
                                tbody_tr.Attributes[attr.Key] = attrValue + (rows + 1).ToString();

                            }
                            else
                            {
                                tbody_tr.Attributes[attr.Key] = attr.Value;
                            }
                            return tbody;
                        });
                    }
                    catch (ArgumentException)
                    {

                    }

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
                    for (int columns = 0; columns < TableColsNumber; columns++)
                    {
                        TagBuilder td = new TagBuilder("td");

                        // try to get value from 
                        try
                        {
                            // Get hidden columns name
                            var item_key = ItemsList[rows]
                                .Keys.ElementAt(columns);
                            // Skip hidden columns
                            if (TableHiddenColumnsList.Contains(item_key))
                            {
                                continue;
                            }

                            var item_value = ItemsList[rows]
                                .Values.ElementAt(columns);

                            td.InnerHtml.AppendHtml(item_value);

                            // Get your table list primary key value
                            var row_Id = ItemsList[rows]
                                .FirstOrDefault(k => k.Key == TablePrimaryKey).Value;

                            AddHtmlRowsAndCols(td, row_Id, columns);
                            AddHtmlCols(td, row_Id, columns);

                        }
                        // if out of index then catch exception to create new columns
                        catch (ArgumentOutOfRangeException)
                        {
                            var row_Id = ItemsList[rows]
                                .FirstOrDefault(k => k.Key == TablePrimaryKey).Value;

                            AddHtmlRowsAndCols(td, row_Id, columns);
                            AddHtmlCols(td, row_Id, columns);

                        }


                        tbody_tr.InnerHtml.AppendHtml(td);
                    }
                    tbody.InnerHtml.AppendHtml(tbody_tr);
                }


            }

            if (ActivePanel)
            {

                TagBuilder panel_heading = new TagBuilder("div");
                HtmlAttributesHelper.AddAttributes(panel_heading, AttrsPanelHeadingDict);

                TagBuilder panel_body = new TagBuilder("div");
                HtmlAttributesHelper.AddAttributes(panel_body, AttrsPanelBodyDict);

                if (ActivePanelHeading)
                {
                    output.Content.AppendHtml(panel_heading);
                    panel_heading = SetPanelElement(
                        panel_heading, "panel-heading",
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
                var htmlValue = TableAppendHtmlRowsColsDict.FirstOrDefault(
                    rows => rows.Key.Equals(row_Id,
                    StringComparison.OrdinalIgnoreCase))
                    .Value.FirstOrDefault(
                      cols => cols.Key.Equals((columns + 1).ToString(),
                      StringComparison.OrdinalIgnoreCase)).Value;

                if (htmlValue != null)
                {
                    htmlValue = htmlValue.Replace("*", row_Id);
                }


                if (ActiveOverrideHtmlRowsCols)
                {
                    td.InnerHtml.SetHtmlContent(htmlValue);
                }
                else
                {
                    td.InnerHtml.AppendHtml(htmlValue);
                }
            }
        }

        private void AddHtmlCols(TagBuilder td, string row_Id, int columns)
        {
            if (HtmlAttributesHelper.IsContainsKey(
                               TableAppendHtmlColsDict, (columns + 1).ToString()))
            {
                var htmlValue = TableAppendHtmlColsDict.FirstOrDefault(
                    cols => cols.Key.Equals(
                        (columns + 1).ToString(), StringComparison.OrdinalIgnoreCase))
                    .Value;


                if (htmlValue != null)
                {
                    htmlValue = htmlValue.Replace("*", row_Id);
                }


                if (ActiveOverrideHtmlCols)
                {
                    td.InnerHtml.SetHtmlContent(htmlValue);
                }
                else
                {
                    td.InnerHtml.AppendHtml(htmlValue);
                }
            }
        }

       
    }
}
