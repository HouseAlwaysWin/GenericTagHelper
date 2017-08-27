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

        public string Items { get; set; }
        private List<Dictionary<string,string>> ItemsList
        {
            get
            {
                if (String.IsNullOrEmpty(Items))
                    return JsonConvert.DeserializeObject<List<Dictionary<string,string>>>(Items);

                return new List<Dictionary<string,string>>();
            }

        }


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
            TagBuilder thead_tr = new TagBuilder("tr");

            foreach (var name in TableHeadList)
            {
                TagBuilder th = new TagBuilder("th");
                th.InnerHtml.AppendHtml(name);
                thead_tr.InnerHtml.AppendHtml(th);
            }

            thead.InnerHtml.AppendHtml(thead_tr);
            table.InnerHtml.AppendHtml(thead);

            TagBuilder tbody = new TagBuilder("tbody");

            foreach (var items in ItemsList)
            {
                TagBuilder tbody_tr = new TagBuilder("tbody_tr");
                foreach (var item in items)
                {
                    TagBuilder td = new TagBuilder("td");
                    td.InnerHtml.AppendHtml(item.ToString());
                    tbody_tr.InnerHtml.AppendHtml(td);
                }

                tbody.InnerHtml.AppendHtml(tbody_tr);
            }

            table.InnerHtml.AppendHtml(tbody);
            output.Content.SetHtmlContent(table);
        }
    }
}
