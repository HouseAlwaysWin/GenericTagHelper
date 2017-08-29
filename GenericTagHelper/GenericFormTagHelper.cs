using GenericTagHelper.MethodHelpers;
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
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace GenericTagHelper
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


        #region Properties Helper
        private ValidationSummary _validationSummary;
        private IUrlHelperFactory urlHelperFactory;
        private IHtmlGenerator Generator { get; }

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
                { "Radio" ,"radio" },
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

        [HtmlAttributeNotBound]
        [ViewContext]
        public ViewContext ViewContext { get; set; }

        [HtmlAttributeNotBound]
        private IUrlHelper urlHelper
        {
            get
            {
                return urlHelperFactory.GetUrlHelper(ViewContext);
            }
        }
        #endregion

        public ModelExpression FormModel { get; set; }

        #region Title
        public string FormTitle { get; set; } = "Form";

        public string FormTitleClass { get; set; } = "";

        public string ClassTitle { get; set; }
        #endregion

        #region FormGroup
        public string AllClassFormGroup { get; set; } = "form-group";

        // Add Json string to match form-group class 
        public string ClassFormGroup { get; set; }
        private Dictionary<string, string> ClassFormGroupDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(ClassFormGroup);
            }
        }

        public string AttributesFormGroup { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttributesFormGroupDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttributesFormGroup);
            }
        }
        #endregion

        #region Label
        public string AllClassLabel { get; set; } = "";

        // Add Json string to match label class
        public string ClassLabel { get; set; }
        private Dictionary<string, string> ClassLabelDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(ClassLabel);
            }
        }

        public string AttributesLabel { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttributesLabelDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttributesLabel);
            }
        }
        #endregion

        #region Input
        public string AllClassInput { get; set; } = "form-control";

        // Add Json string to match input class
        public string ClassInput { get; set; }
        private Dictionary<string, string> ClassInputDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(ClassInput);
            }
        }

        public string AttributesInput { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttributesInputDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttributesInput);
            }
        }
        #endregion

        #region Span
        public string AllClassSpan { get; set; } = "";

        // Add Json string to match span class
        public string ClassSpan { get; set; }
        private Dictionary<string, string> ClassSpanDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(ClassSpan);
            }
        }

        public string AttributesSpan { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttributesSpanDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttributesSpan);
            }
        }
        #endregion

        #region SubmitGroup
        public string SubmitBtnClass { get; set; } = "btn btn-primary";

        public string SubmitBtnContent { get; set; } = "Submit";

        public string CancelBtnClass { get; set; } = "btn btn-default";

        public string CancelBtnContent { get; set; } = "Cancel";

        public string CancelLinkReturnAction { get; set; } = "";

        public string CancelLinkReturnController { get; set; } = "";
        #endregion

        #region ValidationSummary
        /// <summary>
        /// If <see cref="ValidationSummary.All"/> or <see cref="ValidationSummary.ModelOnly"/>, appends a validation
        /// summary. Otherwise (<see cref="ValidationSummary.None"/>, the default), this tag helper does nothing.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Thrown if setter is called with an undefined <see cref="ValidationSummary"/> value e.g.
        /// <c>(ValidationSummary)23</c>.
        /// </exception>
        [HtmlAttributeName("asp-validation-summary")]
        public ValidationSummary ValidationSummary
        {
            get => _validationSummary;
            set
            {
                switch (value)
                {
                    case ValidationSummary.All:
                    case ValidationSummary.ModelOnly:
                    case ValidationSummary.None:
                        _validationSummary = value;
                        break;

                    default:
                        throw new ArgumentException(
                            message: Resources.FormatInvalidEnumArgument(
                                nameof(value),
                                value,
                                typeof(ValidationSummary).FullName),
                            paramName: nameof(value));
                }
            }
        }

        public string AttributesValidationSummary { get; set; }
        private Dictionary<string, string> AttributesValidationSummaryDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttributesValidationSummary);
            }
        }

        public string AttributesValidationSummaryUl { get; set; }
        private Dictionary<string, string> AttributesValidationSummaryUlDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttributesValidationSummaryUl);
            }
        }
        #endregion

        #region RadioButton

        public bool RadioLeft { get; set; }
        public bool RadioRight { get; set; }
        public bool RadioTop { get; set; }
        public bool RadioBottom { get; set; }

        public string RadioButtonDataList { get; set; }
        private Dictionary<string, Dictionary<string, string>> RadioDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(RadioButtonDataList);
            }
        }

        public string ClassRadioBtnValue { get; set; }
        private Dictionary<string, string> ClassRadioBtnValueDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(ClassRadioBtnValue);
            }
        }

        public string AllClassRadioBtnValue { get; set; } = "";
        public string AttributesRadioBtnValue { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttributesRadioBtnValueDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttributesRadioBtnValue);
            }
        }
        #endregion

        public override void Process(
            TagHelperContext context, TagHelperOutput output)
        {
          
            TagBuilder title = new TagBuilder("div");
            title.InnerHtml.SetHtmlContent(FormTitle);

            TagBuilder validation_sum = GenerateValidationSummary();

            validation_sum =HtmlAttributesHelper.AddAttributes(validation_sum, AttributesValidationSummaryDict);

            output.Content.AppendHtml(title);

            output.Content.AppendHtml(validation_sum);

            bool restart;

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



                    if ((property.Metadata.IsEnumerableType || (property.Metadata.IsCollectionType)) &&
                       complex_type_prop_counter == FormModel.ModelExplorer.Properties.Count())
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
                    // Skip EnumerableType
                    else if (property.Metadata.IsEnumerableType)
                    {
                        continue;
                    }

                    // distinguish property between complex type and primary type
                    if (property.Metadata.IsComplexType &&
                        !property.Metadata.IsEnumerableType &&
                        property.ModelType != typeof(IFormFile))
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

                    HtmlAttributesHelper.AddClassAndAttrToTag(
                        form_group, ClassFormGroupDict,
                        AttributesFormGroupDict, property_name,
                        AllClassFormGroup);

                    TagBuilder label = Generator.GenerateLabel(
                        ViewContext,
                        property,
                        property.Metadata.PropertyName,
                        labelText: null,
                        htmlAttributes: null);

                    HtmlAttributesHelper.AddClassAndAttrToTag(
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
                                 HtmlAttributesHelper.AddClassAndAttrToTag(
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
                                     HtmlAttributesHelper.AddClassAndAttrToTag(
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
                                     HtmlAttributesHelper.AddClassAndAttrToTag(
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
                                     HtmlAttributesHelper.AddClassAndAttrToTag(
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
                                     HtmlAttributesHelper.AddClassAndAttrToTag(
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
                                     HtmlAttributesHelper.AddClassAndAttrToTag(
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
                        HtmlAttributesHelper.AddClassAndAttrToTag(
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

                    HtmlAttributesHelper.AddClassAndAttrToTag(
                        span, ClassSpanDict,
                        AttributesSpanDict, property_name,
                        AllClassSpan);

                    /*---------------End print your model----------------*/

                    form_group.InnerHtml.AppendHtml(span);
                  
                    output.Content.AppendHtml(form_group);

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
            cancelBtn.InnerHtml.Append(CancelBtnContent);

         
            output.Content.AppendHtml(submitBtn);
            output.Content.AppendHtml(cancelBtn);

         

        }

        #region Tag Generators

        private TagBuilder GenerateInputType(ModelExplorer modelExplorer, string radioValue = "")
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

        private TagBuilder GenerateValidationSummary()

        {
            if (ValidationSummary == ValidationSummary.None)
            {
                return null;
            }

            var tagBuilder = GenerateValidationSummary(
                ViewContext,
                excludePropertyErrors: ValidationSummary == ValidationSummary.ModelOnly,
                message: null,
                headerTag: null,
                htmlAttributes: null);

            return tagBuilder;
        }

        /// <inheritdoc />
        private TagBuilder GenerateValidationSummary(
            ViewContext viewContext,
            bool excludePropertyErrors,
            string message,
            string headerTag,
            object htmlAttributes)
        {
            if (viewContext == null)
            {
                throw new ArgumentNullException(nameof(viewContext));
            }

            var viewData = viewContext.ViewData;
            if (!viewContext.ClientValidationEnabled && viewData.ModelState.IsValid)
            {
                // Client-side validation is not enabled to add to the generated element and element will be empty.
                return null;
            }

            ModelStateEntry entryForModel;
            if (excludePropertyErrors &&
                (!viewData.ModelState.TryGetValue(viewData.TemplateInfo.HtmlFieldPrefix, out entryForModel) ||
                 entryForModel.Errors.Count == 0))
            {
                // Client-side validation (if enabled) will not affect the generated element and element will be empty.
                return null;
            }

            TagBuilder messageTag;
            if (string.IsNullOrEmpty(message))
            {
                messageTag = null;
            }
            else
            {
                if (string.IsNullOrEmpty(headerTag))
                {
                    headerTag = viewContext.ValidationSummaryMessageElement;
                }

                messageTag = new TagBuilder(headerTag);
                messageTag.InnerHtml.SetContent(message);
            }

            // If excludePropertyErrors is true, describe any validation issue with the current model in a single item.
            // Otherwise, list individual property errors.
            var isHtmlSummaryModified = false;
            var modelStates = ValidationHelpers.GetModelStateList(viewData, excludePropertyErrors);

            var htmlSummary = new TagBuilder("ul");
            htmlSummary = HtmlAttributesHelper.AddAttributes(
                htmlSummary, AttributesValidationSummaryUlDict);
            foreach (var modelState in modelStates)
            {
                // Perf: Avoid allocations
                for (var i = 0; i < modelState.Errors.Count; i++)
                {
                    var modelError = modelState.Errors[i];
                    var errorText = ValidationHelpers.GetModelErrorMessageOrDefault(modelError);

                    if (!string.IsNullOrEmpty(errorText))
                    {
                        var listItem = new TagBuilder("li");
                        listItem.InnerHtml.SetContent(errorText);
                        htmlSummary.InnerHtml.AppendLine(listItem);
                        isHtmlSummaryModified = true;
                    }
                }
            }

            if (!isHtmlSummaryModified)
            {
                htmlSummary.InnerHtml.AppendHtml(@"<li style=""display:none""></li>");
                htmlSummary.InnerHtml.AppendLine();
            }

            var tagBuilder = new TagBuilder("div");
            tagBuilder.MergeAttributes(GetHtmlAttributeDictionaryOrNull(htmlAttributes));

            if (viewData.ModelState.IsValid)
            {
                tagBuilder.AddCssClass(HtmlHelper.ValidationSummaryValidCssClassName);
            }
            else
            {
                tagBuilder.AddCssClass(HtmlHelper.ValidationSummaryCssClassName);
            }

            if (messageTag != null)
            {
                tagBuilder.InnerHtml.AppendLine(messageTag);
            }

            tagBuilder.InnerHtml.AppendHtml(htmlSummary);

            if (viewContext.ClientValidationEnabled && !excludePropertyErrors)
            {
                // Inform the client where to replace the list of property errors after validation.
                tagBuilder.MergeAttribute("data-valmsg-summary", "true");
            }

            return tagBuilder;
        }

        #endregion

        #region Method Helpers
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

        // Only need a dictionary if htmlAttributes is non-null. TagBuilder.MergeAttributes() is fine with null.
        private static IDictionary<string, object> GetHtmlAttributeDictionaryOrNull(
            object htmlAttributes)
        {
            IDictionary<string, object> htmlAttributeDictionary = null;
            if (htmlAttributes != null)
            {
                htmlAttributeDictionary = htmlAttributes as IDictionary<string, object>;
                if (htmlAttributeDictionary == null)
                {
                    htmlAttributeDictionary = HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes);
                }
            }

            return htmlAttributeDictionary;
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
        #endregion
    }
}

