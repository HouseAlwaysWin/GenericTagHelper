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
using System.Reflection;
using System.Threading.Tasks;

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
                { "Select","select" },
                { "TextArea","textarea"},
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
        public string FormTitle { get; set; } = "Form-Title";

        public string ClassTitle { get; set; }

        public string AttrsTitle { get; set; }
        private Dictionary<string, string> AttrsTitleDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsTitle);
            }
        }
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

        public string ClassValidationSummary { get; set; } = "";

        public string AttrsValidationSummary { get; set; }
        private Dictionary<string, string> AttrsValidationSummaryDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsValidationSummary);
            }
        }

        public string ClassValidationSummaryUl { get; set; }

        public string AttrsValidationSummaryUl { get; set; }
        private Dictionary<string, string> AttrsValidationSummaryUlDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsValidationSummaryUl);
            }
        }
        #endregion

        #region FormGroup
        public string ClassAllFormGroup { get; set; } = "form-group";

        // Add Json string to match form-group class 
        public string ClassFormGroup { get; set; }
        private Dictionary<string, string> ClassFormGroupDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(ClassFormGroup);
            }
        }

        public string AttrsAllFormGroup { get; set; }
        private Dictionary<string, string> AttrsAllFormGroupDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsAllFormGroup);
            }
        }

        public string AttrsFormGroup { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttrsFormGroupDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttrsFormGroup);
            }
        }


        #endregion

        #region Label
        public string ClassAllLabel { get; set; } = "";

        // Add Json string to match label class
        public string ClassLabel { get; set; }
        private Dictionary<string, string> ClassLabelDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(ClassLabel);
            }
        }

        public string AttrsAllLabel { get; set; }
        private Dictionary<string, string> AttrsAllLabelDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsAllLabel);
            }
        }

        public string AttrsLabel { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttrsLabelDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttrsLabel);
            }
        }


        #endregion

        #region Input
        public string ClassAllInput { get; set; } = "form-control";

        // Add Json string to match input class
        public string ClassInput { get; set; }
        private Dictionary<string, string> ClassInputDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(ClassInput);
            }
        }

        public string AttrsAllInput { get; set; }
        private Dictionary<string, string> AttrsAllInputDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsAllInput);
            }
        }

        public string AttrsInput { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttrsInputDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttrsInput);
            }
        }
        #endregion

        #region Span
        public string ClassAllSpan { get; set; } = "";

        // Add Json string to match span class
        public string ClassSpan { get; set; }
        private Dictionary<string, string> ClassSpanDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(ClassSpan);
            }
        }

        public string AttrsAllSpan { get; set; }
        private Dictionary<string, string> AttrsAllSpanDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsAllSpan);
            }
        }

        public string AttrsSpan { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttrsSpanDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttrsSpan);
            }
        }
        #endregion

        #region Submit Button
        public string SubmitBtnContent { get; set; } = "Submit";

        public string ClassSubmitBtn { get; set; } = "btn btn-primary";

        public string AttrsSubmitBtn { get; set; }
        private Dictionary<string, string> AttrsSubmitBtnDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsSubmitBtn);
            }
        }
        #endregion

        #region Cancel Button
        public string CancelBtnContent { get; set; } = "Cancel";

        public string ClassCancelBtn { get; set; } = "btn btn-default";

        public string AttrsCancelBtn { get; set; }
        private Dictionary<string, string> AttrsCancelBtnDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsCancelBtn);
            }
        }

        public string CancelLinkReturnAction { get; set; } = "";

        public string CancelLinkReturnController { get; set; } = "";
        #endregion

        #region RadioButton 

        //public bool RadioRight { get; set; }
        public bool RadioLeft { get; set; }
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

        public string ClassRadioBtnSpan { get; set; }
        private Dictionary<string, string> ClassRadioBtnSpanDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(ClassRadioBtnSpan);
            }
        }

        public string AttrsAllRadioBtnSpan { get; set; }
        private Dictionary<string, Dictionary<string, string>> AttrsAllRadioBtnSpanDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(
                    AttrsAllRadioBtnSpan);
            }
        }

        #endregion

        #region  SelectList
        public string SelectDataList { get; set; }
        public Dictionary<string, Dictionary<string, string>> SelectListDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(SelectDataList);
            }
        }
        #endregion

        #region CheckBox 
        public bool CheckBoxTop { get; set; }
        public bool CheckBoxBottom { get; set; }
        public bool CheckBoxLeft { get; set; }
        #endregion

        public override async Task ProcessAsync(
            TagHelperContext context, TagHelperOutput output)
        {

            // Apply Title class and attrs
            TagBuilder title = new TagBuilder("div");
            title.InnerHtml.SetHtmlContent(FormTitle);
            title.AddCssClass(ClassTitle);
            title.Attributes["class"] = ClassTitle;
            HtmlAttributesHelper.AddAttributes(title, AttrsTitleDict);


            // Apply Validation Summary class and attrs
            TagBuilder validation_sum = GenerateValidationSummary();
            //validation_sum.AddCssClass(ClassValidationSummary);
            HtmlAttributesHelper.AddAttributes(
                validation_sum, AttrsValidationSummaryDict);

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



                    if ((property.Metadata.IsEnumerableType ||
                        (property.Metadata.IsCollectionType)) &&
                        complex_type_prop_counter ==
                        FormModel.ModelExplorer.Properties.Count())
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

                    SetTagAttrsAndClass(
                        form_group, ClassAllFormGroup,
                        ClassFormGroupDict, AttrsAllFormGroupDict,
                        AttrsFormGroupDict, property_name);


                    TagBuilder label = Generator.GenerateLabel(
                        ViewContext,
                        property,
                        property.Metadata.PropertyName,
                        labelText: null,
                        htmlAttributes: null);


                    SetTagAttrsAndClass(
                        label, ClassAllLabel,
                        ClassLabelDict, AttrsAllLabelDict,
                        AttrsLabelDict, property_name);

                    TagBuilder input;

                    //if type is radio than use radio button
                    if (property.Metadata.DataTypeName == "Radio")
                    {

                        TagBuilder fieldset = new TagBuilder("fieldset");
                        // According RadioDict's key to match main radio property name
                        // then get the key value pair from Value
                        // and tnen feed key and value to radio button
                        if (RadioDict.Count != 0)
                        {
                            RadioDict.LastOrDefault(
                                 prop => prop.Key.Equals(property_name, StringComparison.OrdinalIgnoreCase))
                                 .Value
                                 .ToDictionary(item =>
                                 {
                                     input = GenerateInputType(property, item.Key);

                                     TagBuilder value_div = new TagBuilder("div");
                                     TagBuilder value_span = new TagBuilder("span");
                                     HtmlAttributesHelper.AddAttributes(
                                         value_span, AttrsAllRadioBtnSpanDict,
                                         property_name);
                                     value_span.InnerHtml.AppendHtml(item.Value);

                                     SetInputLocation(
                                         RadioTop, RadioBottom, RadioLeft,
                                         value_span, input, fieldset);



                                     SetTagAttrsAndClass(
                                         input, ClassAllInput,
                                         ClassInputDict, AttrsAllInputDict,
                                         AttrsInputDict, property_name);

                                     return input;
                                 });
                        }
                        else
                        {
                            TagBuilder NoDataMsg = new TagBuilder("span");
                            NoDataMsg.MergeAttribute("style", "color:red;");
                            NoDataMsg.InnerHtml.SetHtmlContent(
                                "No radio button data,please give a data in your view");
                            fieldset.InnerHtml.AppendHtml(NoDataMsg);
                        }

                        form_group.InnerHtml.AppendHtml(label);
                        form_group.InnerHtml.AppendHtml(fieldset);
                    }
                    else if (property.Metadata.DataTypeName == "CheckBox")
                    {
                        input = GenerateInputType(property);
                        SetInputLocation(
                            CheckBoxTop, CheckBoxBottom, CheckBoxLeft,
                            label, input, form_group);


                        SetTagAttrsAndClass(
                                       input, ClassAllInput,
                                       ClassInputDict, AttrsAllInputDict,
                                       AttrsInputDict, property_name);
                    }
                    else if (property.Metadata.DataTypeName == "Select")
                    {
                        input = GenerateInputType(property);
                        if (HtmlAttributesHelper.IsContainsKey(SelectListDict, property_name) &&
                            SelectListDict.Count() != 0)
                        {
                            SelectListDict.Values.ToDictionary(items => items.ToDictionary(
                                item =>
                                {
                                    TagBuilder option = new TagBuilder("option");
                                    option.Attributes["value"] = item.Key;
                                    option.InnerHtml.SetHtmlContent(item.Value);
                                    input.InnerHtml.AppendHtml(option);
                                    return option;
                                }));
                            form_group.InnerHtml.AppendHtml(label);
                            form_group.InnerHtml.AppendHtml(input);
                            SetTagAttrsAndClass(
                                       input, ClassAllInput,
                                       ClassInputDict, AttrsAllInputDict,
                                       AttrsInputDict, property_name);
                        }
                        else
                        {
                            TagBuilder NoDataMsg = new TagBuilder("div");
                            NoDataMsg.MergeAttribute("style", "color:red;");
                            NoDataMsg.InnerHtml.SetHtmlContent(
                                "No select list data,please give a data in your view");
                            form_group.InnerHtml.AppendHtml(label);
                            form_group.InnerHtml.AppendHtml(NoDataMsg);
                        }
                    }
                    else
                    {
                        input = GenerateInputType(property);

                        SetTagAttrsAndClass(
                                       input, ClassAllInput,
                                       ClassInputDict, AttrsAllInputDict,
                                       AttrsInputDict, property_name);
                        form_group.InnerHtml.AppendHtml(label);
                        form_group.InnerHtml.AppendHtml(input);
                    }


                    TagBuilder span = Generator.GenerateValidationMessage(
                                            ViewContext,
                                            property,
                                            property.Metadata.PropertyName,
                                            message: null,
                                            tag: null,
                                            htmlAttributes: null);


                    SetTagAttrsAndClass(
                        span, ClassAllSpan,
                        ClassSpanDict, AttrsAllSpanDict,
                        AttrsSpanDict, property_name);


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

            output.Content.AppendHtml(await output.GetChildContentAsync());
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
                    Input = GenerateSelectList(modelExplorer, inputTypeHint);
                    break;

                case "textarea":
                    Input = Generator.GenerateTextArea(
                        ViewContext,
                        modelExplorer,
                        modelExplorer.Metadata.PropertyName,
                        rows: 0,
                        columns: 0,
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

        public TagBuilder GenerateSelectList(ModelExplorer modelExplorer, string inputTypeHint)
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

            if (inputTypeHint == "Enum")
            {
                var names = Enum.GetNames(modelExplorer.ModelType);
                for (int i = 0; i < names.Length; i++)
                {
                    TagBuilder selectList = new TagBuilder("option");
                    selectList.MergeAttribute("value", i.ToString());
                    selectList.InnerHtml.SetContent(names[i]);
                    selectTag.InnerHtml.AppendHtml(selectList);
                }
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

            var validation_ul = new TagBuilder("ul");
            validation_ul.AddCssClass(ClassValidationSummaryUl);
            HtmlAttributesHelper.AddAttributes(
                validation_ul, AttrsValidationSummaryUlDict);

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
                        validation_ul.InnerHtml.AppendLine(listItem);
                        isHtmlSummaryModified = true;
                    }
                }
            }

            if (!isHtmlSummaryModified)
            {
                validation_ul.InnerHtml.AppendHtml(@"<li style=""display:none""></li>");
                validation_ul.InnerHtml.AppendLine();
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

            tagBuilder.InnerHtml.AppendHtml(validation_ul);

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


        private void SetInputLocation(
            bool top, bool bottom, bool left,
            TagBuilder label, TagBuilder input, TagBuilder group)

        {
            if (top &&
                !bottom &&
                !left)
            {
                group.InnerHtml.AppendHtml(input);
                TagBuilder div = new TagBuilder("div");
                div.InnerHtml.AppendHtml(label);
                group.InnerHtml.AppendHtml(div);
            }
            else if (!top &&
                bottom &&
                !left)
            {
                TagBuilder div = new TagBuilder("div");
                div.InnerHtml.AppendHtml(label);
                group.InnerHtml.AppendHtml(div);
                group.InnerHtml.AppendHtml(input);
            }
            else if (!top &&
                !bottom &&
                left)
            {
                group.InnerHtml.AppendHtml(input);
                group.InnerHtml.AppendHtml(label);
            }
            else
            {
                group.InnerHtml.AppendHtml(label);
                group.InnerHtml.AppendHtml(input);
            }
        }

        private void SetTagAttrsAndClass(
            TagBuilder tag, string classAll,
            Dictionary<string, string> classDict,
            Dictionary<string, string> attrsAllDict,
            Dictionary<string, Dictionary<string, string>> attrsDict,
            string property_name)
        {
            tag.AddCssClass(classAll);
            HtmlAttributesHelper.AddClass(
                tag, classDict, property_name);
            HtmlAttributesHelper.AddAttributes(
                tag, attrsAllDict,
                attrsDict, property_name);
        }
        #endregion
    }
}

