using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GenericTagHelper.MethodHelpers
{
    public static class AttrsHelper
    {
        #region AttrsOfProp
        public static void SetTagAttrsOfProp(
           Dictionary<string, Dictionary<string, Dictionary<string, string>>> attrsTagsOfPropDict,
           TagBuilder tag,
           string property_name,
           string tag_name)
        {
            if (attrsTagsOfPropDict != null)
            {
                var models = attrsTagsOfPropDict.FirstOrDefault(
                                    modelName => modelName.Key.Equals(
                                        property_name, StringComparison.OrdinalIgnoreCase)).Value;
                if (models != null)
                {
                    var attrs = models.FirstOrDefault(
                        modelName => modelName.Key.Equals(
                            tag_name, StringComparison.OrdinalIgnoreCase)).Value;

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



        public static void SetTagAttrsOfProp(
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> attrsTagsOfPropDict,
            TagBuilder tag,
            string property_name,
            string tag_name,
            string index_num)
        {
            if (attrsTagsOfPropDict != null)
            {
                var models = attrsTagsOfPropDict.FirstOrDefault(
                        modelName => modelName.Key.Equals(
                            property_name, StringComparison.OrdinalIgnoreCase)).Value;
                var tagKey = tag_name + "_" + index_num;

                if (models != null)
                {
                    var attrs = models.FirstOrDefault(modelName => modelName.Key.Equals(
                            tagKey, StringComparison.InvariantCultureIgnoreCase))
                          .Value;
                    if (attrs != null)
                    {
                        for (int i = 0; i < attrs.Count; i++)
                        {
                            var attr = attrs.ElementAt(i);
                            if (attr.Value.Contains("*"))
                            {
                                var value = attr.Value.Replace("*", index_num);
                                tag.Attributes[attr.Key] = value;
                            }
                            else
                            {
                                tag.Attributes[attr.Key] = attr.Value;
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Attrs
        public static void SetTagAttrs(
            Dictionary<string, Dictionary<string, string>> attrsTagsDict,
            TagBuilder tag,
            string tag_name)

        {
            if (attrsTagsDict != null)

            {
                var attrs = attrsTagsDict.FirstOrDefault(
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

        public static void SetTagAttrs(
            Dictionary<string, Dictionary<string, string>> attrsTagsDict,
            TagBuilder tag,
            string tag_name,
            string index)
        {
            var tagKey = tag_name + "_" + index;
            if (attrsTagsDict != null)
            {
                var attrs = attrsTagsDict.FirstOrDefault(
                    attr => attr.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase))
                    .Value;

                if (attrs != null)
                {
                    for (int i = 0; i < attrs.Count; i++)
                    {
                        var attr = attrs.ElementAt(i);
                        if (attr.Value.Contains("*"))
                        {
                            var value = attr.Value.Replace("*", index);
                            tag.Attributes[attr.Key] = value;
                        }
                        else
                        {
                            tag.Attributes[attr.Key] = attr.Value;
                        }
                    }
                }
            }
        }

        public static void SetTagRowsOrColsAttrs(
            Dictionary<string, Dictionary<string, string>> attrsTagsDict,
            TagBuilder tag,
            string tag_name,
            string primary_key,
            string rows_cols_index)
        {
            var tagKey = tag_name + "_" + rows_cols_index;
            if (attrsTagsDict != null)
            {
                var attrs = attrsTagsDict.FirstOrDefault(
                    attr => attr.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase))
                    .Value;

                if (attrs != null)
                {
                    for (int i = 0; i < attrs.Count; i++)
                    {
                        var attr = attrs.ElementAt(i);
                        if (attr.Value.Contains("*"))
                        {
                            var value = attr.Value.Replace("*", primary_key);
                            tag.Attributes[attr.Key] = value;
                        }
                        else
                        {
                            tag.Attributes[attr.Key] = attr.Value;
                        }
                    }
                }
            }
        }

        public static void SetTagAttrs(
           Dictionary<string, Dictionary<string, string>> attrsTagsDict,
           TagBuilder tag,
           string tag_name,
           string rows_index,
           string cols_index)
        {
            var tagKey = tag_name + "_" + rows_index + "_" + cols_index;

            if (attrsTagsDict != null)

            {
                var attrs = attrsTagsDict.FirstOrDefault(
                    attr => attr.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase))
                    .Value;

                if (attrs != null)
                {
                    for (int i = 0; i < attrs.Count; i++)
                    {
                        var attr = attrs.ElementAt(i);
                        if (attr.Value.Contains("*"))
                        {
                            var value = attr.Value.Replace("*", rows_index);
                            tag.Attributes[attr.Key] = value;
                        }
                        else
                        {
                            tag.Attributes[attr.Key] = attr.Value;
                        }
                    }
                }
            }
        }
        #endregion

        #region TagContentOfProp
        public static void SetTagContentOfProp(
        Dictionary<string, Dictionary<string, string>> contentDict,
        TagBuilder tag,
        string property_name,
        string tag_name,
        bool disable_override)
        {
            if (contentDict != null)
            {
                var models = contentDict.FirstOrDefault(
                                    modelName => modelName.Key.Equals(
                                        property_name, StringComparison.OrdinalIgnoreCase)).Value;
                if (models != null)
                {
                    var content = models.FirstOrDefault(
                        modelName => modelName.Key.Equals(
                            tag_name, StringComparison.OrdinalIgnoreCase)).Value;

                    if (content != null)
                    {
                        if (disable_override)
                        {
                            tag.InnerHtml.AppendHtml(content);
                        }
                        else
                        {
                            tag.InnerHtml.SetHtmlContent(content);
                        }
                    }
                }
            }
        }

        public static void SetTagContentOfProp(
            Dictionary<string, Dictionary<string, string>> contentDict,
            TagBuilder tag,
            string property_name,
            string tag_name,
            string index,
            bool disable_override)
        {
            if (contentDict != null)
            {
                var models = contentDict.FirstOrDefault(
                                    modelName => modelName.Key.Equals(
                                        property_name, StringComparison.OrdinalIgnoreCase)).Value;

                var tagKey = tag_name + "_" + index;
                if (models != null)
                {
                    var content = models.FirstOrDefault(
                        modelName => modelName.Key.Equals(
                            tagKey, StringComparison.OrdinalIgnoreCase)).Value;

                    if (content != null)
                    {
                        if (disable_override)
                        {
                            if (content.Contains("*"))
                            {
                                content = content.Replace("*", index);
                                tag.InnerHtml.AppendHtml(content);
                            }
                            else
                            {
                                tag.InnerHtml.AppendHtml(content);
                            }
                        }
                        else
                        {
                            if (content.Contains("*"))
                            {
                                content = content.Replace("*", index);
                                tag.InnerHtml.SetHtmlContent(content);
                            }
                            else
                            {
                                tag.InnerHtml.SetHtmlContent(content);
                            }

                        }
                    }
                }
            }
        }
        #endregion

        #region TagContent
        public static void SetTagContent(
            Dictionary<string, string> tagsDict,
            TagBuilder tag,
            string tag_name,
            bool disable_override)
        {
            if (tagsDict != null)
            {
                var content = tagsDict.FirstOrDefault(
                    t => t.Key.Equals(tag_name, StringComparison.OrdinalIgnoreCase)).Value;

                if (content != null)
                {
                    if (disable_override)
                    {
                        tag.InnerHtml.AppendHtml(content);
                    }
                    else
                    {
                        tag.InnerHtml.SetHtmlContent(content);
                    }
                }
            }
        }

        public static void SetTagContent(
          Dictionary<string, string> tagsDict,
          TagBuilder tag,
          string tag_name,
          string index,
          bool disable_override)
        {
            var tagKey = tag_name + "_" + index;
            if (tagsDict != null)
            {
                var content = tagsDict.FirstOrDefault(
                    t => t.Key.Equals(
                        tagKey, StringComparison.OrdinalIgnoreCase)).Value;

                if (content != null)
                {
                    if (disable_override)
                    {
                        if (content.Contains("*"))
                        {
                            content = content.Replace("*", index);
                            tag.InnerHtml.AppendHtml(content);
                        }
                        else
                        {
                            tag.InnerHtml.AppendHtml(content);
                        }
                    }
                    else
                    {
                        if (content.Contains("*"))
                        {
                            content = content.Replace("*", index);
                            tag.InnerHtml.SetHtmlContent(content);
                        }
                        else
                        {
                            tag.InnerHtml.SetHtmlContent(content);
                        }
                    }
                }
            }
        }

        public static void SetTagRowsOrColsContent(
            Dictionary<string, string> tagsDict,
            TagBuilder tag,
            string tag_name,
            string primary_key,
            string rows_cols_index,
            bool disable_override)
        {
            var tagKey = tag_name + "_" + rows_cols_index;
            if (tagsDict != null)
            {
                var content = tagsDict.FirstOrDefault(
                    attr => attr.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase))
                    .Value;

                if (content != null)
                {
                    if (disable_override)
                    {
                        if (content.Contains("*"))
                        {
                            content = content.Replace("*", primary_key);
                            tag.InnerHtml.AppendHtml(content);
                        }
                        else
                        {
                            tag.InnerHtml.AppendHtml(content);
                        }
                    }
                    else
                    {
                        if (content.Contains("*"))
                        {
                            content = content.Replace("*", primary_key);
                            tag.InnerHtml.SetHtmlContent(content);
                        }
                        else
                        {
                            tag.InnerHtml.SetHtmlContent(content);
                        }

                    }
                }
            }
        }


        public static void SetTagContent(
          Dictionary<string, string> tagsDict,
          TagBuilder tag,
          string tag_name,
          string rows_num,
          string cols_num,
          bool disable_override)
        {
            var tagKey = tag_name + "_" + rows_num + "_" + cols_num;
            if (tagsDict != null)
            {
                var content = tagsDict.FirstOrDefault(
                    t => t.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase)).Value;

                if (content != null)
                {
                    if (disable_override)
                    {
                        if (content.Contains("*"))
                        {
                            content = content.Replace("*", rows_num);
                            tag.InnerHtml.AppendHtml(content);
                        }
                        else
                        {
                            tag.InnerHtml.AppendHtml(content);
                        }
                    }
                    else
                    {
                        if (content.Contains("*"))
                        {
                            content = content.Replace("*", rows_num);
                            tag.InnerHtml.SetHtmlContent(content);
                        }
                        else
                        {
                            tag.InnerHtml.SetHtmlContent(content);
                        }
                    }
                }
            }
        }
        #endregion

        #region TagDisableOfProp
        public static bool SetTagDisable(
            Dictionary<string, Dictionary<string, bool>> disableTagDict,
            string property_name,
            string tag_name)
        {
            if (disableTagDict != null)
            {
                var models = disableTagDict.FirstOrDefault(
                    prop => prop.Key.Equals(property_name,
                    StringComparison.OrdinalIgnoreCase))
                    .Value;

                if (models != null)
                {
                    bool disableLabel = models.FirstOrDefault(
                         tag => tag.Key.Equals(tag_name,
                         StringComparison.OrdinalIgnoreCase))
                        .Value;

                    return disableLabel;
                }
            }
            return false;
        }

        public static bool SetTagDisable(
           Dictionary<string, Dictionary<string, bool>> disableTagDict,
           string property_name,
           string tag_name,
           string index)
        {
            if (disableTagDict != null)
            {
                var models = disableTagDict.FirstOrDefault(
                    prop => prop.Key.Equals(property_name,
                    StringComparison.OrdinalIgnoreCase))
                    .Value;

                var tagKey = tag_name + "_" + index;

                if (models != null)
                {
                    bool disableLabel = models.FirstOrDefault(
                         tag => tag.Key.Equals(tagKey,
                         StringComparison.OrdinalIgnoreCase))
                        .Value;

                    return disableLabel;
                }
            }
            return false;
        }
        #endregion
    }
}

