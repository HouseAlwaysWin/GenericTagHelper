using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.TagHelpers.Internal;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.ViewFeatures.Internal;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace GenericTagHelper.Form
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
                {"Radio" ,"radio" },
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
                { nameof(Enum),"select" },
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

        public ModelExpression FormModel { get; set; }

        public string FormTitle { get; set; } = "Form";

        public string FormTitleClass { get; set; } = "";

        public string AllClassFormGroup { get; set; } = "form-group";

        public string AllClassLabel { get; set; } = "";

        public string AllClassInput { get; set; } = "form-control";

        public string AllClassSpan { get; set; } = "";

        public string SubmitBtnClass { get; set; } = "btn btn-primary";

        public string SubmitBtnContent { get; set; } = "Submit";

        public string CancelBtnClass { get; set; } = "btn btn-default";

        public string CancelBtnContant { get; set; } = "Cancel";

        public string CancelLinkReturnAction { get; set; } = "";

        public string CancelLinkReturnController { get; set; } = "";


        public string RadioButtonDataList { get; set; }
        private Dictionary<string, Dictionary<string, string>> RadioDict
        {
            get
            {
                if (!String.IsNullOrEmpty(RadioButtonDataList))
                {
                    return JsonConvert
                        .DeserializeObject<
                            Dictionary<string, Dictionary<string, string>>>(
                        RadioButtonDataList);
                }
                return new Dictionary<string, Dictionary<string, string>>();
            }
        }

        public bool RadioLeft { get; set; }
        public bool RadioRight { get; set; }
        public bool RadioTop { get; set; }
        public bool RadioBottom { get; set; }

        public string ClassRadioBtnValue { get; set; }
        private Dictionary<string, string> ClassRadioBtnValueDict
        {
            get
            {
                return ClassJsonStringConvert(ClassRadioBtnValue);
            }
        }

        public string AllClassRadioBtnValue { get; set; } = "";
        public string AttributesRadioBtnValue { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttributesRadioBtnValueDict
        {
            get
            {
                return AttributeJsonStringConvert(AttributesRadioBtnValue);
            }
        } 

        public string ClassTitle { get; set; }

        // Add Json string to match form-group class 
        public string ClassFormGroup { get; set; }
        private Dictionary<string, string> ClassFormGroupDict
        {
            get
            {
                return ClassJsonStringConvert(ClassFormGroup);
            }
        }


        // Add Json string to match label class
        public string ClassLabel { get; set; }
        private Dictionary<string, string> ClassLabelDict
        {
            get
            {
                return ClassJsonStringConvert(ClassLabel);
            }
        }

        // Add Json string to match input class
        public string ClassInput { get; set; }
        private Dictionary<string, string> ClassInputDict
        {
            get
            {
                return ClassJsonStringConvert(ClassInput);
            }
        }

        // Add Json string to match span class
        public string ClassSpan { get; set; }
        private Dictionary<string, string> ClassSpanDict
        {
            get
            {
                return ClassJsonStringConvert(ClassSpan);
            }
        }

        public string AttributesFormGroup { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttributesFormGroupDict
        {
            get
            {
                return AttributeJsonStringConvert(AttributesFormGroup);
            }
        }

        public string AttributesInput { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttributesInputDict
        {
            get
            {
                return AttributeJsonStringConvert(AttributesInput);
            }
        }

        public string AttributesLabel { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttributesLabelDict
        {
            get
            {
                return AttributeJsonStringConvert(AttributesLabel);
            }
        }

        public string AttributesSpan { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttributesSpanDict
        {
            get
            {
                return AttributeJsonStringConvert(AttributesSpan);
            }
        }

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {
            TagBuilder form = new TagBuilder("form");
            TagBuilder title = new TagBuilder("div");
            title.InnerHtml.SetHtmlContent(FormTitle);

            form.InnerHtml.AppendHtml(title);

            bool restart;
            // counter for your number of complex type's properties

            // Temporary counter for complex type and primary type
            int property_counter = 0;

            // Store main model for use in the future
            ModelExpression form_model = FormModel;
            // Store main model current counter
            int main_model_prop_counter = 0;
            // make sure complex type is start to count
            bool start_complex_type_loop = false;

            do
            {
                restart = false;
                // each complex type prop counter start from 0
                int complex_type_prop_counter = 0;

                // Loop your Form Model 
                for (int p = property_counter; p < FormModel.ModelExplorer.Properties.Count(); p++)
                {
                    if (start_complex_type_loop)
                        complex_type_prop_counter++;
                    // Get model property's name
                    var property_name = FormModel.Metadata.Properties[p].PropertyName;
                    // Get model property
                    var property = FormModel.ModelExplorer.Properties.SingleOrDefault(
                        n => n.Metadata.PropertyName == property_name);

                    // Skip EnumerableType
                    if (property.Metadata.IsEnumerableType)
                    {
                        continue;
                    }

                    // distinguish property between complex type and primary type
                    if (property.Metadata.IsComplexType &&
                        (!property.Metadata.IsEnumerableType))
                    {
                        // Get Complex type model
                        ModelExpression complexType = new ModelExpression(
                            property.Metadata.PropertyName, property);
                        // Set main model to complex for next loop
                        FormModel = complexType;
                        // Set start counter for next loop 
                        property_counter = complex_type_prop_counter;

                        start_complex_type_loop = true;

                        // Save Main model counter
                        main_model_prop_counter = p;

                        restart = true;
                        break;
                    }

                    /*-------------- Start Print your models-----------*/

                    TagBuilder form_group = new TagBuilder("div");

                    AddClassAndAttrToTag(
                        form_group, ClassFormGroupDict,
                        AttributesFormGroupDict, property_name,
                        AllClassFormGroup);

                    TagBuilder label = Generator.GenerateLabel(
                        ViewContext,
                        property,
                        property.Metadata.PropertyName,
                        labelText: null,
                        htmlAttributes: null);

                    AddClassAndAttrToTag(
                        label, ClassLabelDict,
                        AttributesLabelDict, property_name,
                        AllClassLabel);

                    form_group.InnerHtml.AppendHtml(label);



                    TagBuilder input;

                    // if type is radio than use radio button
                    if (property.Metadata.DataTypeName == "Radio")
                    {
                        
                        TagBuilder fieldset = new TagBuilder("fieldset");
                        // According RadioDict's key to match main radio property name
                        // then get the key value pair from Value
                        // and tnen feed key and value to radio button
                        RadioDict.LastOrDefault(
                             prop => prop.Key.Equals(property_name, StringComparison.OrdinalIgnoreCase))
                             .Value
                             .ToDictionary(item =>
                             {
                                 input = GenerateInputType(property, item.Key);
                                 AddClassAndAttrToTag(
                                                  input, ClassInputDict,
                                                   AttributesInputDict, property_name,
                                                   AllClassInput);

                                 TagBuilder value_div = new TagBuilder("div");
                                 TagBuilder value_span = new TagBuilder("span");
                                 // radio button left
                                 if (RadioLeft &&
                                    !RadioRight &&
                                    !RadioTop &&
                                    !RadioBottom)
                                 {
                                     fieldset.InnerHtml.AppendHtml(input);

                                     value_span.InnerHtml.AppendHtml(item.Value);
                                     AddClassAndAttrToTag(
                                         value_span, ClassRadioBtnValueDict,
                                         AttributesRadioBtnValueDict, property_name,
                                         AllClassRadioBtnValue);
                                     fieldset.InnerHtml.AppendHtml(value_span);
                                 }
                                 // radio button right
                                 else if (
                                     !RadioLeft &&
                                      RadioRight &&
                                     !RadioTop &&
                                     !RadioBottom)
                                 {
                                     value_span.InnerHtml.AppendHtml(item.Value);
                                     AddClassAndAttrToTag(
                                        value_span, ClassRadioBtnValueDict,
                                        AttributesRadioBtnValueDict, property_name,
                                        AllClassRadioBtnValue);
                                     fieldset.InnerHtml.AppendHtml(value_span);

                                     fieldset.InnerHtml.AppendHtml(input);
                                 }
                                 // radio button top
                                 else if (
                                     !RadioLeft &&
                                     !RadioRight &&
                                      RadioTop &&
                                     !RadioBottom)
                                 {
                                     fieldset.InnerHtml.AppendHtml(input);
                                     value_div.InnerHtml.AppendHtml(item.Value);
                                     AddClassAndAttrToTag(
                                        value_div, ClassRadioBtnValueDict,
                                        AttributesRadioBtnValueDict, property_name,
                                        AllClassRadioBtnValue);

                                     fieldset.InnerHtml.AppendHtml(value_div);
                                 }
                                 // radio button bottom
                                 else if (
                                    !RadioLeft &&
                                    !RadioRight &&
                                    !RadioTop &&
                                     RadioBottom)
                                 {
                                     value_div.InnerHtml.AppendHtml(item.Value);
                                     AddClassAndAttrToTag(
                                        value_div, ClassRadioBtnValueDict,
                                        AttributesRadioBtnValueDict, property_name,
                                        AllClassRadioBtnValue);
                                     fieldset.InnerHtml.AppendHtml(value_div);

                                     fieldset.InnerHtml.AppendHtml(input);
                                 }
                                 // default
                                 else
                                 {
                                     fieldset.InnerHtml.AppendHtml(input);

                                     value_span.InnerHtml.AppendHtml(item.Value);
                                     AddClassAndAttrToTag(
                                        value_span, ClassRadioBtnValueDict,
                                        AttributesRadioBtnValueDict, property_name,
                                        AllClassRadioBtnValue);
                                     fieldset.InnerHtml.AppendHtml(value_span);
                                 }

                                 return input;
                             });
                        form_group.InnerHtml.AppendHtml(fieldset);
                    }
                    else
                    {
                        input = GenerateInputType(property);
                        AddClassAndAttrToTag(
                            input, ClassInputDict,
                            AttributesInputDict, property_name,
                            AllClassInput);
                        form_group.InnerHtml.AppendHtml(input);
                    }

                    TagBuilder span = Generator.GenerateValidationMessage(
                                            ViewContext,
                                            property,
                                            property.Metadata.PropertyName,
                                            message: null,
                                            tag: null,
                                            htmlAttributes: null);

                    AddClassAndAttrToTag(
                        span, ClassSpanDict,
                        AttributesSpanDict, property_name,
                        AllClassSpan);

                    /*---------------End print your model----------------*/

                    form_group.InnerHtml.AppendHtml(span);
                    form.InnerHtml.AppendHtml(form_group);

                    // End loop according your number of properties
                    if (complex_type_prop_counter > FormModel.Metadata.Properties.Count() - 1)
                    {
                        // close complex type loop,because of ending of complex type's properties
                        start_complex_type_loop = false;
                        // Set counter for next property, must +1 
                        // or it will loop previous property.
                        property_counter = main_model_prop_counter + 1;
                        // Set model to  main model
                        FormModel = form_model;
                        restart = true;
                        break;
                    }
                }
            } while (restart);

            TagBuilder submitBtn = new TagBuilder("button");
            submitBtn.MergeAttribute("type", "submit");
            submitBtn.AddCssClass(SubmitBtnClass);
            submitBtn.InnerHtml.SetContent(SubmitBtnContent);

            TagBuilder cancelBtn = new TagBuilder("a");
            if (CancelLinkReturnAction != "" && CancelLinkReturnController == "")
            {
                cancelBtn.Attributes["href"] = urlHelper.Action(CancelLinkReturnAction);
            }
            else
            {
                cancelBtn.Attributes["href"] = urlHelper.Action(CancelLinkReturnAction, CancelLinkReturnController);
            }
            cancelBtn.AddCssClass(CancelBtnClass);
            cancelBtn.MergeAttribute("style", "margin-left:10px;");
            cancelBtn.InnerHtml.Append(CancelBtnContant);

            form.InnerHtml.AppendHtml(submitBtn);
            form.InnerHtml.AppendHtml(cancelBtn);

            output.Content.SetHtmlContent(form);

        }


        public TagBuilder GenerateInputType(ModelExplorer modelExplorer, string radioValue = "")
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
                                           value: radioValue,
                                           isChecked: null,
                                           htmlAttributes: null);
                    break;

                case "select":
                    Input = GenerateSelectList(modelExplorer);
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

        private static IEnumerable<string> GetInputTypeHints(
            ModelExplorer modelExplorer)
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

        public TagBuilder GenerateSelectList(ModelExplorer modelExplorer)
        {

            // Base allowMultiple on the instance or declared type of the expression to avoid a
            // "SelectExpressionNotEnumerable" InvalidOperationException during generation.
            // Metadata.IsEnumerableType is similar but does not take runtime type into account.
            var realModelType = modelExplorer.ModelType;
            var _allowMultiple = typeof(string) != realModelType &&
                typeof(IEnumerable).IsAssignableFrom(realModelType);

            var items = Enumerable.Empty<SelectListItem>();

            var selectTag = Generator.GenerateSelect(
                ViewContext,
                modelExplorer,
                optionLabel: null,
                expression: modelExplorer.Metadata.PropertyName,
                selectList: items,
                currentValues: null,
                allowMultiple: _allowMultiple,
                htmlAttributes: null);


            var names = Enum.GetNames(modelExplorer.ModelType);
            for (int i = 0; i < names.Length; i++)
            {
                TagBuilder selectList = new TagBuilder("option");
                selectList.MergeAttribute("value", i.ToString());
                selectList.InnerHtml.SetContent(names[i]);
                selectTag.InnerHtml.AppendHtml(selectList);
            }

            return selectTag;
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
        #region Helpers

        private bool IsContainsPropertyKey(
            Dictionary<string, string> tagClassDict,
            string propertyName)
        {
            return tagClassDict.Any(d => d.Key.Equals(
                propertyName, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsContainsPropertyKey(
            Dictionary<string, Dictionary<string, string>> tagClassDict,
            string propertyName)
        {
            return tagClassDict.Any(d => d.Key.Equals(
                propertyName, StringComparison.OrdinalIgnoreCase));
        }


        private void AddClass(
            TagBuilder tag,
            Dictionary<string, string> tagClassDict,
            string propertyName)
        {
            tag.AddCssClass(
                tagClassDict.LastOrDefault(
                    fg => fg.Key.Equals(propertyName,
                    StringComparison.OrdinalIgnoreCase)).Value);
        }

        private void AddAttribute(
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

        private Dictionary<string, string> ClassJsonStringConvert(
            string classString)
        {
            if (!String.IsNullOrEmpty(classString))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, string>>(classString);
            }
            return new Dictionary<string, string>();
        }

        private Dictionary<string, Dictionary<string, string>> AttributeJsonStringConvert(
            string attributeString)
        {
            if (!String.IsNullOrEmpty(attributeString))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(attributeString);
            }
            return new Dictionary<string, Dictionary<string, string>>();
        }

        private Dictionary<string, ModelExpression> ModelExpJsonStringConvert(
            string modelExpString)
        {
            if (!String.IsNullOrEmpty(modelExpString))
            {
                return JsonConvert.DeserializeObject<Dictionary<string, ModelExpression>>(modelExpString);
            }
            return new Dictionary<string, ModelExpression>();
        }


        public void AddClassAndAttrToTag(
            TagBuilder tag,
            Dictionary<string, string> classDict,
            Dictionary<string, Dictionary<string, string>> attributeDict,
            string propertyName,
             string allClass)
        {
            if (IsContainsPropertyKey(
                           classDict, propertyName))
            {
                AddClass(tag, classDict, propertyName);
            }
            else
            {
                tag.AddCssClass(allClass);
            }

            if (IsContainsPropertyKey(
                    attributeDict, propertyName))
            {
                AddAttribute(tag, attributeDict, propertyName);
            }
        }

      
        #endregion
    }
}
