using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace GenericTagHelper.Helpers
{
    [HtmlTargetElement("table", Attributes = "generic")]
    public class GenericTableTagHelper : TagHelper
    {
        public string TableHeads { get; set; }

        public ModelExpression Items { get; set; }

        private List<string> TableHeadList
        {
            get
            {
                if (!String.IsNullOrEmpty(TableHeads))
                {
                    return JsonConvert.DeserializeObject<List<string>>(TableHeads);
                }
                return new List<string>();
            }
        }

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {
            TagBuilder table = new TagBuilder("table");

            TagBuilder thead = new TagBuilder("thead");
            TagBuilder thead_tr = new TagBuilder("thead_tr");

            foreach (var name in TableHeadList)
            {
                TagBuilder th = new TagBuilder("th");
                th.InnerHtml.AppendHtml(th);
                thead_tr.InnerHtml.AppendHtml(th);
            }

            TagBuilder tbody = new TagBuilder("tbody");

            foreach (var item in Items.ModelExplorer.Properties)
            {
                TagBuilder tbody_tr = new TagBuilder("tbody_tr");
                TagBuilder td = new TagBuilder("td");
                td.InnerHtml.AppendHtml(item.Metadata.Placeholder);
                tbody_tr.InnerHtml.AppendHtml(td);
                tbody.InnerHtml.AppendHtml(tbody_tr);
            }

            table.InnerHtml.AppendHtml(thead).AppendHtml(tbody);
            output.Content.SetHtmlContent(table);
        }
    }
}
