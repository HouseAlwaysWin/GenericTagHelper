using GenericTagHelper.MethodHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace GenericTagHelper
{
    [HtmlTargetElement("table-panel")]
    [RestrictChildren("table")]
    public class GenericTablePanelTagHelper : TagHelper
    { 
        public bool ActivePanelHeading { get; set; }
        public bool ActivePanelBody { get; set; }

        public string PanelHeadingHtmlContent { get; set; }
        public string PanelBodyHtmlContent { get; set; }

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

        public override async Task ProcessAsync(
            TagHelperContext context, TagHelperOutput output)
        {


            TagBuilder panel_heading = new TagBuilder("div");


            TagBuilder panel_body = new TagBuilder("div");


            output.Attributes.SetAttribute("class", "panel panel-primary");

            if (ActivePanelHeading)
            {
                output.Content.AppendHtml(panel_heading);
                panel_heading.AddCssClass("panel-heading");
                panel_heading = HtmlAttributesHelper.AddAttributes(
                    panel_heading, AttributesPanelHeadingDict);
                panel_heading.InnerHtml.AppendHtml(PanelHeadingHtmlContent);
            }
            if (ActivePanelBody)
            {
                output.Content.AppendHtml(panel_body);
                panel_body.AddCssClass("panel-body");
                panel_body = HtmlAttributesHelper.AddAttributes(
                    panel_body, AttributesPanelBodyDict);
                panel_body.InnerHtml.AppendHtml(PanelBodyHtmlContent);
            }

            output.TagName = "div";
            output.TagMode = TagMode.StartTagAndEndTag;

            output.Content.AppendHtml(await output.GetChildContentAsync());

        }
    }
}
