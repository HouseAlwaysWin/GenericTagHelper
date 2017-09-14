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
            var models = attrsTagsOfPropDict.FirstOrDefault(
                                modelName => modelName.Key.Equals(
                                    property_name, StringComparison.OrdinalIgnoreCase)).Value;
            if (attrsTagsOfPropDict.Count != 0 &&
                attrsTagsOfPropDict.Keys.Any(modelName => modelName.Equals(property_name,
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



        public static void SetTagAttrsOfProp(
            Dictionary<string, Dictionary<string, Dictionary<string, string>>> attrsTagsOfPropDict,
            TagBuilder tag,
            string property_name,
            string tag_name,
            string index_num)
        {
            var models = attrsTagsOfPropDict.FirstOrDefault(
                    modelName => modelName.Key.Equals(
                        property_name, StringComparison.OrdinalIgnoreCase)).Value;
            var tagKey = tag_name + "_" + index_num;
            if (attrsTagsOfPropDict.Count != 0 &&
                attrsTagsOfPropDict.Keys.Any(modelName => modelName.Equals(property_name, StringComparison.OrdinalIgnoreCase)) &&
                models.Keys.Any(prop => prop.Equals(tagKey, StringComparison.OrdinalIgnoreCase)))
            {
                var attrs = models.FirstOrDefault(modelName => modelName.Key.Equals(
                        tagKey, StringComparison.InvariantCultureIgnoreCase))
                      .Value;
                if (attrs != null)
                {
                    for (int i = 0; i < attrs.Count; i++)
                    {
                        var attr = attrs.ElementAt(i);
                        if (attr.Value.EndsWith("*"))
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

        #endregion

        #region Attrs
        public static void SetTagAttrs(
            Dictionary<string, Dictionary<string, string>> attrsTagsDict,
            TagBuilder tag,
            string tag_name)

        {
            if (attrsTagsDict.Count() != 0 &&
                attrsTagsDict.Keys.Any(
                    tagName => tagName.Equals(
                        tag_name, StringComparison.OrdinalIgnoreCase)))
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
            if (attrsTagsDict.Count() != 0 &&
                attrsTagsDict.Keys.Any(
                    tagName => tagName.Equals(
                        tagKey, StringComparison.OrdinalIgnoreCase)))
            {
                var attrs = attrsTagsDict.FirstOrDefault(
                    attr => attr.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase))
                    .Value;

                if (attrs != null)
                {
                    for (int i = 0; i < attrs.Count; i++)
                    {
                        var attr = attrs.ElementAt(i);
                        if (attr.Value.EndsWith("*"))
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
            if (attrsTagsDict.Count() != 0 &&
                attrsTagsDict.Keys.Any(
                    tagName => tagName.Equals(
                        tagKey, StringComparison.OrdinalIgnoreCase)))
            {
                var attrs = attrsTagsDict.FirstOrDefault(
                    attr => attr.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase))
                    .Value;

                if (attrs != null)
                {
                    for (int i = 0; i < attrs.Count; i++)
                    {
                        var attr = attrs.ElementAt(i);
                        if (attr.Value.EndsWith("*"))
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


        //public static void SetTagRowsAttrs(
        //    Dictionary<string, Dictionary<string, string>> attrsTagsDict,
        //    TagBuilder tag,
        //    string tag_name,
        //    string rows_index,
        //    string primary_key)
        //{
        //    var tagKey = tag_name + "_" + rows_index;
        //    if (attrsTagsDict.Count() != 0 &&
        //        attrsTagsDict.Keys.Any(
        //            tagName => tagName.Equals(
        //                tagKey, StringComparison.OrdinalIgnoreCase)))
        //    {
        //        var attrs = attrsTagsDict.FirstOrDefault(
        //            attr => attr.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase))
        //            .Value;

        //        if (attrs != null)
        //        {
        //            for (int i = 0; i < attrs.Count; i++)
        //            {
        //                var attr = attrs.ElementAt(i);
        //                if (attr.Value.EndsWith("*"))
        //                {
        //                    var value = attr.Value.Replace("*", primary_key);
        //                    tag.Attributes[attr.Key] = value;
        //                }
        //                else
        //                {
        //                    tag.Attributes[attr.Key] = attr.Value;
        //                }
        //            }
        //        }
        //    }
        //}


        public static void SetTagAttrs(
           Dictionary<string, Dictionary<string, string>> attrsTagsDict,
           TagBuilder tag,
           string tag_name,
           string rows_index,
           string cols_index)
        {
            var tagKey = tag_name + "_" + cols_index + "_" + rows_index;

            if (attrsTagsDict.Count() != 0 &&
                attrsTagsDict.Keys.Any(
                    tagName => tagName.Equals(
                        tagKey, StringComparison.OrdinalIgnoreCase)))
            {
                var attrs = attrsTagsDict.FirstOrDefault(
                    attr => attr.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase))
                    .Value;

                if (attrs != null)
                {
                    for (int i = 0; i < attrs.Count; i++)
                    {
                        var attr = attrs.ElementAt(i);
                        if (attr.Value.EndsWith("*"))
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

        #region TagContent
        public static void SetTagContent(
            Dictionary<string, string> tagsDict,
            TagBuilder tag,
            string tag_name,
            bool override_num)
        {
            if (tagsDict.Count != 0 &&
                tagsDict.Any(
                    t => t.Key.Equals(tag_name,
                    StringComparison.OrdinalIgnoreCase)))
            {
                var content = tagsDict.FirstOrDefault(
                    t => t.Key.Equals(tag_name, StringComparison.OrdinalIgnoreCase)).Value;

                if (override_num)
                {
                    tag.InnerHtml.SetHtmlContent(content);
                }
                else
                {
                    tag.InnerHtml.AppendHtml(content);
                }
            }
        }


        public static void SetTagContent(
          Dictionary<string, string> tagsDict,
          TagBuilder tag,
          string tag_name,
          string index,
          bool override_num)
        {
            var tagKey = tag_name + "_" + index;
            if (tagsDict.Count != 0 &&
                tagsDict.Any(
                    t => t.Key.Equals(tagKey,
                    StringComparison.OrdinalIgnoreCase)))
            {
                var content = tagsDict.FirstOrDefault(
                    t => t.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase)).Value;

                if (override_num)
                {
                    if (content.EndsWith("*"))
                    {
                        content = content.Replace("*", index);
                        tag.InnerHtml.SetHtmlContent(content);
                    }
                    else
                    {
                        tag.InnerHtml.SetHtmlContent(content);
                    }
                }
                else
                {
                    if (content.EndsWith("*"))
                    {
                        content = content.Replace("*", index);
                        tag.InnerHtml.AppendHtml(content);
                    }
                    else
                    {
                        tag.InnerHtml.AppendHtml(content);
                    }
                }
            }

        }



        public static void SetTagRowsOrColsContent(
            Dictionary<string, string> tagsDict,
            TagBuilder tag,
            string tag_name,
            string rows_cols_index,
            string primary_key,
            bool override_num)
        {
            var tagKey = tag_name + "_" + rows_cols_index;
            if (tagsDict.Count != 0 &&
                tagsDict.Any(
                    t => t.Key.Equals(tagKey,
                    StringComparison.OrdinalIgnoreCase)))
            {
                var content = tagsDict.FirstOrDefault(
                    t => t.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase)).Value;

                if (override_num)
                {
                    if (content.EndsWith("*"))
                    {
                        content = content.Replace("*", primary_key);
                        tag.InnerHtml.SetHtmlContent(content);
                    }
                    else
                    {
                        tag.InnerHtml.SetHtmlContent(content);
                    }
                }
                else
                {
                    if (content.EndsWith("*"))
                    {
                        content = content.Replace("*", primary_key);
                        tag.InnerHtml.AppendHtml(content);
                    }
                    else
                    {
                        tag.InnerHtml.AppendHtml(content);
                    }
                }
            }

        }


        //public static void SetTagRowsContent(
        //   Dictionary<string, string> tagsDict,
        //   TagBuilder tag,
        //   string tag_name,
        //   string rows_num,
        //   string primary_key,
        //   bool override_num)
        //{
        //    var tagKey = tag_name + "_" + rows_num;
        //    if (tagsDict.Count != 0 &&
        //        tagsDict.Any(
        //            t => t.Key.Equals(tagKey,
        //            StringComparison.OrdinalIgnoreCase)))
        //    {
        //        var content = tagsDict.FirstOrDefault(
        //            t => t.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase)).Value;

        //        if (override_num)
        //        {
        //            if (content.EndsWith("*"))
        //            {
        //                content = content.Replace("*", primary_key);
        //                tag.InnerHtml.Append(content);
        //            }
        //            else
        //            {
        //                tag.InnerHtml.Append(content);
        //            }
        //        }
        //        else
        //        {
        //            if (content.EndsWith("*"))
        //            {
        //                content = content.Replace("*", cols_num);
        //                tag.InnerHtml.AppendHtml(content);
        //            }
        //            else
        //            {
        //                tag.InnerHtml.AppendHtml(content);
        //            }
        //        }
        //    }
        //}

        public static void SetTagContent(
          Dictionary<string, string> tagsDict,
          TagBuilder tag,
          string tag_name,
          string rows_num,
          string cols_num,
          bool override_num)
        {
            var tagKey = tag_name + "_" + rows_num + "_" + cols_num;
            if (tagsDict.Count != 0 &&
                tagsDict.Any(
                    t => t.Key.Equals(tagKey,
                    StringComparison.OrdinalIgnoreCase)))
            {
                var content = tagsDict.FirstOrDefault(
                    t => t.Key.Equals(tagKey, StringComparison.OrdinalIgnoreCase)).Value;

                if (override_num)
                {
                    if (content.EndsWith("*"))
                    {
                        content = content.Replace("*", rows_num);
                        tag.InnerHtml.SetHtmlContent(content);
                    }
                    else
                    {
                        tag.InnerHtml.SetHtmlContent(content);
                    }
                }
                else
                {
                    if (content.EndsWith("*"))
                    {
                        content = content.Replace("*", rows_num);
                        tag.InnerHtml.AppendHtml(content);
                    }
                    else
                    {
                        tag.InnerHtml.AppendHtml(content);
                    }
                }
            }
        }
        #endregion
    }
}

