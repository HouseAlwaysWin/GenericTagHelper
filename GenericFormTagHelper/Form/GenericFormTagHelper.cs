using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace GenericFormTagHelper
{
    [HtmlTargetElement("generic")]
    public class GenericFormTagHelper : TagHelper
    {
        public ModelExpression model { get; set; }
        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {
            TagBuilder form = new TagBuilder("form");
            foreach (var m in model.ModelExplorer.Properties)
            {
                TagBuilder form_group = new TagBuilder("div");
                form_group.AddCssClass("form-group");
                TagBuilder label = new TagBuilder("label");
                label.MergeAttribute("asp-for", m.Properties.ToString());
                TagBuilder input = new TagBuilder("input");
                input.AddCssClass("form-control");
                input.MergeAttribute("asp-for", m.Properties.ToString());
                TagBuilder span = new TagBuilder("span");
                span.AddCssClass("text-danger");
                span.MergeAttribute("asp-validation-for", m.Properties.ToString());
                form_group.InnerHtml.AppendHtml(label);
                form_group.InnerHtml.AppendHtml(input);
                form_group.InnerHtml.AppendHtml(span);
                form.InnerHtml.AppendHtml(form_group);
            }
            output.TagName = "form";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(form);
        }
    }
}
