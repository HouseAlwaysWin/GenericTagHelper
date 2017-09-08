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
    public static class TagBuilderExtensions
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
            string content,
            Dictionary<string, string> attributes)
        {
            tagBuilder.InnerHtml.AppendHtml(content);
            foreach (var attr in attributes)
            {

                tagBuilder.Attributes[attr.Key] = attr.Value;
            }

            return tagBuilder.ToHtmlString();
        }

        public static string ToLinkHtmlString(
            this TagBuilder tagBuilder,
            string urlString,
            string linkContent)
        {

            tagBuilder.Attributes["href"] = urlString;
            tagBuilder.InnerHtml.AppendHtml(linkContent);
            return tagBuilder.ToHtmlString();
        }

        public static string ToLinkHtmlString(
           this TagBuilder tagBuilder,
           string urlString,
           string linkContent,
           Dictionary<string, string> linkAttributes)
        {
            tagBuilder.Attributes["href"] = urlString;
            tagBuilder.InnerHtml.AppendHtml(linkContent);
            foreach (var attr in linkAttributes)
            {
                tagBuilder.Attributes[attr.Key] = attr.Value;
            }
            return tagBuilder.ToHtmlString();
        }

        public static string ToLinkHtmlString(
            this TagBuilder tagBuilder,
            IUrlHelper urlHelper,
            string action,
            string controller,
            object linkQuery,
            string linkContent)
        {
            tagBuilder.Attributes["href"] = urlHelper.Action(action, controller, linkQuery);
            tagBuilder.InnerHtml.AppendHtml(linkContent);
            return tagBuilder.ToHtmlString();
        }

        public static string ToLinkHtmlString(
            this TagBuilder tagBuilder,
            IUrlHelper urlHelper,
            string action,
            string controller,
            object linkQuery,
            string linkContent,
            Dictionary<string, string> linkAttributes)
        {
            tagBuilder.Attributes["href"] = urlHelper.Action(action, controller, linkQuery);
            tagBuilder.InnerHtml.AppendHtml(linkContent);
            foreach (var attr in linkAttributes)
            {
                tagBuilder.Attributes[attr.Key] = attr.Value;
            }
            return tagBuilder.ToHtmlString();
        }

        public static string ToRadioGroupHtmlString(
            this TagBuilder tagBuilder,
            Dictionary<string, string> fieldSetAttrs,
            int radioNumbers,
            List<Dictionary<string, string>> radiosAttrs)
        {
            foreach (var attr in fieldSetAttrs)
            {
                tagBuilder.Attributes[attr.Key] = attr.Value;
            }
            for (int i = 0; i < radioNumbers; ++i)
            {
                TagBuilder input = new TagBuilder("input");
                foreach (var attr in radiosAttrs[i])
                {
                    input.Attributes[attr.Key] = attr.Value;
                }

            }

        }

        public static string ToSelectHtmlString(
            this TagBuilder tagBuilder,
            Dictionary<string, string> selectAttrs,
            int optionNumbers,
            List<Dictionary<string, string>> optionsAttrs)
        {
            foreach (var attr in selectAttrs)
            {
                tagBuilder.Attributes[attr.Key] = attr.Value;
            }

            for (int i = 0; i < optionNumbers; ++i)
            {
                TagBuilder option = new TagBuilder("options");
                foreach (var attr in optionsAttrs[i])
                {
                    option.Attributes[attr.Key] = attr.Value;
                }
                tagBuilder.InnerHtml.AppendHtml(option);
            }

            return tagBuilder.ToHtmlString();
        }


    }
}
