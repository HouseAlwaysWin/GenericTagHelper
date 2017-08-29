using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericTagHelper.MethodHelpers
{
    public static class HtmlAttributesHelper
    {
     
        public static bool IsContainsKey(
            Dictionary<string, string> tagClassDict,
            string propertyName)
        {
            return tagClassDict.Any(d => d.Key.Equals(
                propertyName, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsContainsKey(
            Dictionary<string, Dictionary<string, string>> tagClassDict,
            string propertyName)
        {
            return tagClassDict.Any(d => d.Key.Equals(
                propertyName, StringComparison.OrdinalIgnoreCase));
        }

        public static void  AddClass(
            TagBuilder tag,
            Dictionary<string, string> tagClassDict,
            string propertyName)
        {
            tag.AddCssClass(
                tagClassDict.LastOrDefault(
                    fg => fg.Key.Equals(propertyName,
                    StringComparison.OrdinalIgnoreCase)).Value);
        }

        public static  TagBuilder AddAttributes(
            TagBuilder tag, Dictionary<string, string> tagAttributeDict)
        {
            foreach (var attr in tagAttributeDict)
            {
                tag.Attributes[attr.Key] = attr.Value;
            }
            return tag;
        }
        public static void AddAttributes(
            TagBuilder tag,
            Dictionary<string, Dictionary<string, string>> tagAttributeDict,
            string propertyName)
        {
            // find property if match propertyName then apply attributes.
            tagAttributeDict.LastOrDefault(
                prop => prop.Key
                .Equals(propertyName, StringComparison.OrdinalIgnoreCase))
                .Value
                .ToDictionary(attr => tag.Attributes[attr.Key] = attr.Value);
        }
    }
}
