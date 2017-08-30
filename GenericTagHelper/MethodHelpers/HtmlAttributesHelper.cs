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
            Dictionary<string, string> itemAttrs,
            string loopKey)
        {
            return itemAttrs.Any(d => d.Key.Equals(
                loopKey, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsContainsKey(
            Dictionary<string, Dictionary<string, string>> itemAttrs,
            string loopKey)
        {
            return itemAttrs.Any(item => item.Key.Equals(
                loopKey, StringComparison.OrdinalIgnoreCase));
        }

        public static void AddClass(
            TagBuilder tag,
            Dictionary<string, string> itemClass,
            string loopKey)
        {
            if (IsContainsKey(itemClass, loopKey) &&
                itemClass.Count != 0)
            {
                tag.Attributes["class"] =
                    itemClass.LastOrDefault(
                        item => item.Key.Equals(loopKey,
                        StringComparison.OrdinalIgnoreCase)).Value;
            }
        }

        //public static void AddOneItemClass(
        //    TagBuilder tag,
        //    Dictionary<string, string> oneItemClass,
        //    string loopKey)
        //{
        //    tag.AddCssClass(
        //        oneItemClass.LastOrDefault(
        //            item => item.Key.Equals(loopKey,
        //            StringComparison.OrdinalIgnoreCase)).Value);
        //}

        #region  Attributes
        public static void AddAttributes(
            TagBuilder tag,
            Dictionary<string, string> itemAttrs)
        {
            if (itemAttrs.Keys.Count != 0)
            {
                itemAttrs.ToDictionary(attr =>
                {
                    tag.Attributes[attr.Key] = attr.Value;
                    return tag;
                });
            }
        }

        // For applying all tags's attributes
        public static void AddAttributes(
            TagBuilder tag,
            Dictionary<string, string> allItemsAttr,
            string loopKey)
        {
            if (allItemsAttr.Count != 0)
            {
                foreach (var attr in allItemsAttr)
                {
                    tag.Attributes[attr.Key] = attr.Value;
                }
            }
        }

        // For applying specific tag's attributes
        public static void AddAttributes(
            TagBuilder tag,
            Dictionary<string, Dictionary<string, string>> oneItemAttr,
            string loopKey)
        {
            if (IsContainsKey(oneItemAttr, loopKey) &&
                oneItemAttr.Count != 0)
            {
                oneItemAttr.LastOrDefault(
                item => item.Key
                .Equals(loopKey, StringComparison.OrdinalIgnoreCase))
                .Value
                .ToDictionary(attr => tag.Attributes[attr.Key] = attr.Value);
            }
        }


        // For applying both type
        public static void AddAttributes(
            TagBuilder tag,
            Dictionary<string, string> allItemAttrs,
            Dictionary<string, Dictionary<string, string>> oneItemAttrs,
            string loopKey
            )
        {
            AddAttributes(tag, allItemAttrs, loopKey);
            AddAttributes(tag, oneItemAttrs, loopKey);
        }
        #endregion

        //public static void AddClassAndAttrToTag(
        //    TagBuilder tag,
        //    Dictionary<string, string> classDict,
        //    Dictionary<string, Dictionary<string, string>> attributeDict,
        //    string loopKey,
        //    string allClass)
        //{
        //    AddAllClass(tag,classDict,loopKey);




        //    if (IsContainsKey(
        //            attributeDict, loopKey))
        //    {
        //        AddAttributes(tag, attributeDict, loopKey);
        //    }

        //}
    }
}
