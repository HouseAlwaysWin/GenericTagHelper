using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;

namespace GenericTagHelper.Extensions
{
    public static class StringExtensions
    {
        public static string ToHtmlString(
            this TagBuilder tagBuilder)
        {
            var writer = new StringWriter();
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);
            return writer.ToString();
        }

        public static string ToGeneralTagHtmlString(
            this TagBuilder tagBuilder,
            string tagContent,
            Dictionary<string, string> attributes)
        {
            tagBuilder.InnerHtml.AppendHtml(tagContent);
            foreach (var attr in attributes)
            {
                tagBuilder.Attributes[attr.Key] = attr.Value;
            }
            return tagBuilder.ToHtmlString();
        }

        public static string ToGeneralTagHtmlString(
            this TagBuilder tagBuilder,
            TagBuilder tagContent,
            Dictionary<string, string> attributes)
        {
            tagBuilder.InnerHtml.AppendHtml(tagContent);
            foreach (var attr in attributes)
            {
                tagBuilder.Attributes[attr.Key] = attr.Value;
            }
            return tagBuilder.ToHtmlString();
        }

    }
}
