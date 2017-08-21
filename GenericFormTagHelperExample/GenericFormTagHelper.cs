using Microsoft.ApplicationInsights.AspNetCore;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GenericFormTagHelperExample
{
    [HtmlTargetElement("form", Attributes = "generic")]

    public class GenericFormTagHelper : TagHelper
    {
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

        public GenericFormTagHelper(
            IUrlHelperFactory urlHelperFactory,
            IHtmlGenerator generator)
        {
            this.urlHelperFactory = urlHelperFactory;
            Generator = generator;
        }

        protected IHtmlGenerator Generator { get; }

        [HtmlAttributeNotBound]
        private IUrlHelper urlHelper
        {
            get
            {
                return urlHelperFactory.GetUrlHelper(viewContext);
            }
        }

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext viewContext { get; set; }

        public object ViewModel { get; set; }

        public ModelExpression Model { get; set; }

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }

            TagBuilder form = new TagBuilder("form");
            foreach (var m in Model.ModelExplorer.Properties)
            {
                TagBuilder form_group = new TagBuilder("div");
                form_group.AddCssClass("form-group");

                TagBuilder label = Generator.GenerateLabel(
                    viewContext,
                    m,
                    m.Metadata.PropertyName,
                    m.Metadata.PropertyName,
                    null);


                TagBuilder Input = GenerateInputTag(m);
                if (!Input.Attributes["type"].Contains("checkbox"))
                    Input.AddCssClass("form-control");

                TagBuilder span = new TagBuilder("span");
                span.AddCssClass("text-danger");

                form_group.InnerHtml.AppendHtml(label);
                form_group.InnerHtml.AppendHtml(Input);
                form_group.InnerHtml.AppendHtml(span);
                form.InnerHtml.AppendHtml(form_group);
            }

            TagBuilder div_formgroup = new TagBuilder("div");
            div_formgroup.AddCssClass("form-group");

            TagBuilder submit_btn = new TagBuilder("button");
            submit_btn.AddCssClass("btn btn-primary");
            submit_btn.MergeAttribute("type", "submit");
            submit_btn.InnerHtml.Append("Submit");

            TagBuilder cancel_link = new TagBuilder("a");
            cancel_link.Attributes["href"] = urlHelper.Action("About");
            cancel_link.AddCssClass("btn btn-default");
            cancel_link.MergeAttribute("style", "margin-left:10px;");
            cancel_link.InnerHtml.Append("Cancel");


            div_formgroup.InnerHtml.AppendHtml(submit_btn);
            div_formgroup.InnerHtml.AppendHtml(cancel_link);

            form.InnerHtml.AppendHtml(div_formgroup);
            output.TagName = "form";
            output.TagMode = TagMode.StartTagAndEndTag;
            output.Content.SetHtmlContent(form);
        }


        public TagBuilder GenerateInputTag(ModelExplorer model)
        {
            TagBuilder Input = new TagBuilder("input");
            var metadata = model.Metadata;
            var modelExplorer = model;
            if (metadata == null)
            {
                throw new InvalidOperationException("Nno provide metadata");
            }

            string inputTypeHint;
            string inputType = GetInputType(modelExplorer, out inputTypeHint);

            if (!Input.Attributes.ContainsKey("type"))
            {
                Input.MergeAttribute("type", inputType);
            }
            TagBuilder tagBuilder;
            switch (inputType)
            {
                case "hidden":
                    tagBuilder = GenerateHidden(modelExplorer);
                    break;
                case "checkbox":
                    tagBuilder = GenerateCheckBox(modelExplorer);
                    break;
                case "password":
                    tagBuilder = Generator.GeneratePassword(
                        viewContext,
                        modelExplorer,
                        modelExplorer.Metadata.PropertyName,
                        value: null,
                        htmlAttributes: null);
                    break;
                case "radio":
                    tagBuilder = GenerateRadio(modelExplorer);
                    break;
                default:
                    tagBuilder = GenerateTextBox(modelExplorer, inputTypeHint, inputType);
                    break;
            }

            if (tagBuilder != null)
            {
                if (tagBuilder.HasInnerHtml)
                {
                    // Since this is not the "checkbox" special-case, no guarantee that output is a self-closing
                    // element. A later tag helper targeting this element may change output.TagMode.
                    Input.InnerHtml.AppendHtml(tagBuilder.InnerHtml);
                }
            }

            return Input;

        }

        private TagBuilder GenerateTextBox(ModelExplorer modelExplorer, string inputTypeHint, string inputType)
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
                viewContext,
                modelExplorer,
                modelExplorer.Metadata.PropertyName,
                value: modelExplorer.Model,
                format: format,
                htmlAttributes: htmlAttributes);
        }

        private TagBuilder GenerateCheckBox(
            ModelExplorer modelExplorer)
        {
            TagBuilder Input = new TagBuilder("input");
            if (modelExplorer.ModelType == typeof(string))
            {
                if (modelExplorer.Model != null)
                {
                    bool potentialBool;
                    if (!bool.TryParse(modelExplorer.Model.ToString(), out potentialBool))
                    {
                        throw new InvalidOperationException("FormatInputTagHelper_InvalidStringResult");
                    }
                }
            }
            else if (modelExplorer.ModelType != typeof(bool))
            {
                throw new InvalidOperationException("FormatInputTagHelper_InvalidExpressionResult");
            }

            // hiddenForCheckboxTag always rendered after the returned element
            var hiddenForCheckboxTag = Generator.GenerateHiddenForCheckbox(
                viewContext, modelExplorer, modelExplorer.Metadata.PropertyName);

            if (hiddenForCheckboxTag != null)
            {
                hiddenForCheckboxTag.TagRenderMode = Input.TagRenderMode;

                if (viewContext.FormContext.CanRenderAtEndOfForm)
                {
                    viewContext.FormContext.EndOfFormContent.Add(hiddenForCheckboxTag);
                }
                else
                {
                    Input.InnerHtml.AppendHtml(hiddenForCheckboxTag);
                }
            }

            return Generator.GenerateCheckBox(
                viewContext,
                modelExplorer,
                modelExplorer.Metadata.PropertyName,
                isChecked: null,
                htmlAttributes: null);
        }

        private TagBuilder GenerateRadio(ModelExplorer modelExplorer)
        {
            // Note empty string is allowed.
            if (modelExplorer.Metadata.PropertyName == null)
            {
                throw new InvalidOperationException("FormatInputTagHelper_ValueRequired");
            }

            return Generator.GenerateRadioButton(
                viewContext,
                modelExplorer,
                modelExplorer.Metadata.PropertyName,
                modelExplorer.Metadata.PropertyName,
                isChecked: null,
                htmlAttributes: null);
        }

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
                viewContext,
                modelExplorer,
                modelExplorer.Metadata.PropertyName,
                value: value,
                format: modelExplorer.Metadata.DisplayFormatString,
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
                viewContext.Html5DateRenderingMode == Html5DateRenderingMode.Rfc3339 &&
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

        protected string GetInputType(ModelExplorer modelExplorer, out string inputTypeHint)
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

    }
}
