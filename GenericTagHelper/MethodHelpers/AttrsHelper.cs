using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericTagHelper.MethodHelpers
{
    public static class AttrsHelper
    {
        private static void SetLocalTagAttrs(
           Dictionary<string, Dictionary<string, Dictionary<string, string>>> attrsLocalTagDict,
           TagBuilder tag,
           string property_name,
           string tag_name)
        {
            var models = attrsLocalTagDict.FirstOrDefault(
                                modelName => modelName.Key.Equals(
                                    property_name, StringComparison.OrdinalIgnoreCase)).Value;
            if (attrsLocalTagDict.Count != 0 &&
                attrsLocalTagDict.Keys.Any(modelName => modelName.Equals(property_name,
                    StringComparison.OrdinalIgnoreCase)) &&
                models.Keys.Any(prop => prop.Equals(tag_name,
                    StringComparison.OrdinalIgnoreCase)))
            {
                var attrs = models.FirstOrDefault(
                    modelName => modelName.Key.Equals(
                        tag_name, StringComparison.OrdinalIgnoreCase)).Value;

                for (int i = 0; i < attrs.Count; i++)
                {
                    var attr = attrs.ElementAt(i);
                    tag.Attributes[attr.Key] = attr.Value;
                }
            }
        }

        private static void SetLocalTagAttrs(
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> attrsLocalTagDict,
            TagBuilder tag,
            string property_name,
            string tag_name,
            string value_num)
        {
            var models = attrsLocalTagDict.FirstOrDefault(
                    modelName => modelName.Key.Equals(
                        property_name, StringComparison.OrdinalIgnoreCase)).Value;
            var tagkey = tag_name + "_" + value_num;
            if (attrsLocalTagDict.Count != 0 &&
                attrsLocalTagDict.Keys.Any(modelName => modelName.Equals(property_name, StringComparison.OrdinalIgnoreCase)) &&
                models.Keys.Any(prop => prop.Equals(tagkey, StringComparison.OrdinalIgnoreCase)))
            {
                var attrs = models.FirstOrDefault(modelName => modelName.Key.Equals(
                        tagkey, StringComparison.InvariantCultureIgnoreCase))
                      .Value;
                if (attrs != null)
                {
                    for (int i = 0; i < attrs.Count; i++)
                    {
                        var attr = attrs.ElementAt(i);
                        tag.Attributes[attr.Key] = attr.Value;
                    }
                }
            }
        }

        private static void SetGlobalTagsAttrs(
            Dictionary<string, Dictionary<string, string>> attrsGlobalTagsDict,
            TagBuilder tag,
            string tag_name)
        {
            if (attrsGlobalTagsDict.Count() != 0 &&
                attrsGlobalTagsDict.Keys.Any(
                    tagName => tagName.Equals(
                        tag_name, StringComparison.OrdinalIgnoreCase)))
            {
                var attrs = attrsGlobalTagsDict.FirstOrDefault(
                    attr => attr.Key.Equals(tag_name, StringComparison.OrdinalIgnoreCase))
                    .Value;

                if (attrs != null)
                {
                    for (int i = 0; i < attrs.Count; i++)
                    {
                        var attr = attrs.ElementAt(i);
                        tag.Attributes[attr.Key] = attr.Value;
                    }
                }
            }
        }
    }
}
