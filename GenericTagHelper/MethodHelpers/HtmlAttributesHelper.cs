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

        //public static void AddAllClass(
        //    TagBuilder tag,
        //    Dictionary<string, string> allClassAttr,
        //    string loopKey)
        //{
        //    if (IsContainsKey(allClassAttr, loopKey) &&
        //        allClassAttr.Count != 0)
        //    {
        //        tag.AddCssClass(
        //            allClassAttr.LastOrDefault(
        //                fg => fg.Key.Equals(loopKey,
        //                StringComparison.OrdinalIgnoreCase)).Value);
        //    }
        //}

        public static void AddOneItemClass(
            TagBuilder tag,
            Dictionary<string, string> oneItemClass,
            string loopKey)
        {
            tag.AddCssClass(
                oneItemClass.LastOrDefault(
                    item => item.Key.Equals(loopKey,
                    StringComparison.OrdinalIgnoreCase)).Value);
        }

        #region  Attributes
        public static TagBuilder AddAllItemsAttributes(
            TagBuilder tag,
            Dictionary<string, string> allItemsAttr,
            string loopKey)
        {
            if (IsContainsKey(allItemsAttr, loopKey) &&
                allItemsAttr.Count != 0)
            {
                foreach (var attr in allItemsAttr)
                {
                    tag.Attributes[attr.Key] = attr.Value;
                }
            }
            return tag;
        }

        public static void AddOneItemAttributes(
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


        public static void AddAttributesToTag(
            TagBuilder tag,
            Dictionary<string, string> allItemAttrs,
            Dictionary<string, Dictionary<string, string>> oneItemAttrs,
            string loopKey
            )
        {
            AddAllItemsAttributes(tag, allItemAttrs, loopKey);
            AddOneItemAttributes(tag, oneItemAttrs, loopKey);
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
