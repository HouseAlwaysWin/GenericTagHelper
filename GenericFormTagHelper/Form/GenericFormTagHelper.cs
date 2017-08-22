﻿using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;

namespace GenericFormTagHelper.Form
{
    [HtmlTargetElement("form", Attributes = "generic")]
    public class GenericFormTagHelper : TagHelper
    {
        public GenericFormTagHelper(
                   IUrlHelperFactory urlHelperFactory,
                   IHtmlGenerator generator)
        {
            this.urlHelperFactory = urlHelperFactory;
            Generator = generator;
        }

        private IUrlHelperFactory urlHelperFactory;

        // Mapping from datatype names and data annotation hints to values for the <input/> element's "type" attribute.
        private static readonly Dictionary<string, string> _defaultInputTypes =
            new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "HiddenInput", InputType.Hidden.ToString().ToLowerInvariant() },
                { "Password", InputType.Password.ToString().ToLowerInvariant() },
                { "Text", InputType.Text.ToString().ToLowerInvariant() },
                { "PhoneNumber", "tel" },
                { "Url", "url" },
                { "EmailAddress", "email" },
                { "Date", "date" },
                { "DateTime", "datetime-local" },
                { "DateTime-local", "datetime-local" },
                { "Time", "time" },
                { nameof(Byte), "number" },
                { nameof(SByte), "number" },
                { nameof(Int16), "number" },
                { nameof(UInt16), "number" },
                { nameof(Int32), "number" },
                { nameof(UInt32), "number" },
                { nameof(Int64), "number" },
                { nameof(UInt64), "number" },
                { nameof(Single), InputType.Text.ToString().ToLowerInvariant() },
                { nameof(Double), InputType.Text.ToString().ToLowerInvariant() },
                { nameof(Boolean), InputType.CheckBox.ToString().ToLowerInvariant() },
                { nameof(Decimal), InputType.Text.ToString().ToLowerInvariant() },
                { nameof(String), InputType.Text.ToString().ToLowerInvariant() },
                { nameof(IFormFile), "file" },
                { TemplateRenderer.IEnumerableOfIFormFileName, "file" },
            };

        // Mapping from <input/> element's type to RFC 3339 date and time formats.
        private static readonly Dictionary<string, string> _rfc3339Formats =
            new Dictionary<string, string>(StringComparer.Ordinal)
            {
                { "date", "{0:yyyy-MM-dd}" },
                { "datetime", "{0:yyyy-MM-ddTHH:mm:ss.fffK}" },
                { "datetime-local", "{0:yyyy-MM-ddTHH:mm:ss.fff}" },
                { "time", "{0:HH:mm:ss.fff}" },
            };

        private IHtmlGenerator Generator { get; }


        [HtmlAttributeNotBound]
        private IUrlHelper urlHelper
        {
            get
            {
                return urlHelperFactory.GetUrlHelper(ViewContext);
            }
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        public ModelExpression Model { get; set; }

        public string FormTitle { get; set; } = "Form";

        public string FormGroupClass { get; set; } = "form-group";

        public string FormInputClass { get; set; } = "form-control";

        public string SubmitBtnClass { get; set; } = "btn btn-primary";

        public string CancelBtnClass { get; set; } = "btn btn-default";

        public string CancelLinkAction { get; set; }

        public Dictionary<string,string> CustomPropertyClass { get; set; }
        

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {
            TagBuilder form = new TagBuilder("form");
            TagBuilder title = new TagBuilder("title");
            title.InnerHtml.SetHtmlContent(FormTitle);
            form.InnerHtml.AppendHtml(title);
            foreach (var property in Model.ModelExplorer.Properties)
            {
                TagBuilder form_group = new TagBuilder("div");
                form_group.AddCssClass(FormGroupClass);

                TagBuilder label = Generator.GenerateLabel(
                    ViewContext,
                    property,
                    property.Metadata.PropertyName,
                    labelText: null,
                    htmlAttributes: null);

                TagBuilder input = GenerateInputType(property);

                if (CustomPropertyClass.ContainsKey(property.Metadata.PropertyName))
                {
                    input.AddCssClass(CustomPropertyClass.Values.ToString());
                }
                else
                {
                    input.AddCssClass(FormInputClass);
                }


                TagBuilder span = Generator.GenerateValidationMessage(
                    ViewContext,
                    property,
                    property.Metadata.PropertyName,
                    message: null,
                    tag: null,
                    htmlAttributes: null);

                form_group.InnerHtml.AppendHtml(label);
                form_group.InnerHtml.AppendHtml(input);
                form_group.InnerHtml.AppendHtml(span);
                form.InnerHtml.AppendHtml(form_group);
            }
            TagBuilder submitBtn = new TagBuilder("button");
            submitBtn.MergeAttribute("type", "submit");
            submitBtn.AddCssClass(SubmitBtnClass);
            submitBtn.InnerHtml.SetContent("Submit");

            TagBuilder cancelBtn = new TagBuilder("a");
            cancelBtn.Attributes["href"] = urlHelper.Action(CancelLinkAction);
            cancelBtn.AddCssClass(CancelBtnClass);
            cancelBtn.MergeAttribute("style", "margin-left:10px;");
            cancelBtn.InnerHtml.Append("Cancel");

            form.InnerHtml.AppendHtml(submitBtn);
            form.InnerHtml.AppendHtml(cancelBtn);

            output.Content.SetHtmlContent(form);

        }

        public TagBuilder GenerateInputType(ModelExplorer modelExplorer)
        {
            string inputTypeHint;
            string inputType = GetInputType(modelExplorer, out inputTypeHint);

            TagBuilder Input;
            switch (inputType)
            {
                case "hidden":
                    Input = GenerateHidden(modelExplorer);
                    break;
                case "checkbox":
                    Input = Generator.GenerateCheckBox(
                        ViewContext,
                        modelExplorer,
                        modelExplorer.Metadata.PropertyName,
                        isChecked: null,
                        htmlAttributes: null);
                    break;
                case "password":
                    Input = Generator.GeneratePassword(
                        ViewContext,
                        modelExplorer,
                        modelExplorer.Metadata.PropertyName,
                        value: null,
                        htmlAttributes: null);
                    break;
                case "radio":
                    Input = Generator.GenerateRadioButton(
                        ViewContext,
                        modelExplorer,
                        modelExplorer.Metadata.PropertyName,
                        value: modelExplorer.Metadata.PropertyGetter,
                        isChecked: null,
                        htmlAttributes: null);
                    break;
                default:
                    Input = GenerateTextBox(
                        modelExplorer,
                        inputTypeHint,
                        inputType);
                    break;
            }
            if (!Input.Attributes.ContainsKey("type"))
            {
                Input.MergeAttribute("type", inputType);
            }
            return Input;
        }

        private string GetInputType(
            ModelExplorer modelExplorer, out string inputTypeHint)
        {
            foreach (var hint in GetInputTypeHints(modelExplorer))
            {
                string inputType;
                if (_defaultInputTypes.TryGetValue(hint, out inputType))
                {
                    inputTypeHint = hint;
                    return inputType;
                }
            }

            inputTypeHint = InputType.Text.ToString().ToLowerInvariant();
            return inputTypeHint;
        }

        private static IEnumerable<string> GetInputTypeHints(ModelExplorer modelExplorer)
        {
            if (!string.IsNullOrEmpty(modelExplorer.Metadata.TemplateHint))
            {
                yield return modelExplorer.Metadata.TemplateHint;
            }

            if (!string.IsNullOrEmpty(modelExplorer.Metadata.DataTypeName))
            {
                yield return modelExplorer.Metadata.DataTypeName;
            }

            // In most cases, we don't want to search for Nullable<T>. We want to search for T, which should handle
            // both T and Nullable<T>. However we special-case bool? to avoid turning an <input/> into a <select/>.
            var fieldType = modelExplorer.ModelType;
            if (typeof(bool?) != fieldType)
            {
                fieldType = modelExplorer.Metadata.UnderlyingOrModelType;
            }

            foreach (string typeName in TemplateRenderer.GetTypeNames(modelExplorer.Metadata, fieldType))
            {
                yield return typeName;
            }
        }

        // Imitate Generator.GenerateHidden() using Generator.GenerateTextBox(). This adds support for asp-format that
        // is not available in Generator.GenerateHidden().
        private TagBuilder GenerateHidden(ModelExplorer modelExplorer)
        {
            var value = modelExplorer.Model;
            var byteArrayValue = value as byte[];
            if (byteArrayValue != null)
            {
                value = Convert.ToBase64String(byteArrayValue);
            }

            // In DefaultHtmlGenerator(), GenerateTextBox() calls GenerateInput() _almost_ identically to how
            // GenerateHidden() does and the main switch inside GenerateInput() handles InputType.Text and
            // InputType.Hidden identically. No behavior differences at all when a type HTML attribute already exists.
            var htmlAttributes = new Dictionary<string, object>
            {
                { "type", "hidden" }
            };

            return Generator.GenerateTextBox(
                ViewContext,
                modelExplorer,
                modelExplorer.Metadata.PropertyName,
                value: value,
                format: null,
                htmlAttributes: htmlAttributes);
        }

        private TagBuilder GenerateTextBox(
            ModelExplorer modelExplorer, string inputTypeHint, string inputType)
        {
            var format = modelExplorer.Metadata.DisplayFormatString;
            if (string.IsNullOrEmpty(format))
            {
                format = GetFormat(modelExplorer, inputTypeHint, inputType);
            }

            var htmlAttributes = new Dictionary<string, object>
            {
                { "type", inputType }
            };

            if (string.Equals(inputType, "file") && string.Equals(inputTypeHint, TemplateRenderer.IEnumerableOfIFormFileName))
            {
                htmlAttributes["multiple"] = "multiple";
            }

            return Generator.GenerateTextBox(
                ViewContext,
                modelExplorer,
                modelExplorer.Metadata.PropertyName,
                value: modelExplorer.Model,
                format: format,
                htmlAttributes: htmlAttributes);
        }

        // Get a fall-back format based on the metadata.
        private string GetFormat(ModelExplorer modelExplorer, string inputTypeHint, string inputType)
        {
            string format;
            string rfc3339Format;
            if (string.Equals("decimal", inputTypeHint, StringComparison.OrdinalIgnoreCase) &&
                string.Equals("text", inputType, StringComparison.Ordinal) &&
                string.IsNullOrEmpty(modelExplorer.Metadata.EditFormatString))
            {
                // Decimal data is edited using an <input type="text"/> element, with a reasonable format.
                // EditFormatString has precedence over this fall-back format.
                format = "{0:0.00}";
            }
            else if (_rfc3339Formats.TryGetValue(inputType, out rfc3339Format) &&
                ViewContext.Html5DateRenderingMode == Html5DateRenderingMode.Rfc3339 &&
                !modelExplorer.Metadata.HasNonDefaultEditFormat &&
                (typeof(DateTime) == modelExplorer.Metadata.UnderlyingOrModelType || typeof(DateTimeOffset) == modelExplorer.Metadata.UnderlyingOrModelType))
            {
                // Rfc3339 mode _may_ override EditFormatString in a limited number of cases e.g. EditFormatString
                // must be a default format (i.e. came from a built-in [DataType] attribute).
                format = rfc3339Format;
            }
            else
            {
                // Otherwise use EditFormatString, if any.
                format = modelExplorer.Metadata.EditFormatString;
            }

            return format;
        }
    }
}
