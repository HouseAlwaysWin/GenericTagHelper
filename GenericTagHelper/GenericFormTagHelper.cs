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
        public string FormTitle { get; set; }
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

        //public string ClassValidationSummary { get; set; } = "";

        //public string AttrsValidationSummary { get; set; }
        //private Dictionary<string, string> AttrsValidationSummaryDict
        //{
        //    get
        //    {
        //        return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsValidationSummary);
        //    }
        //}

        //public string ClassValidationSummaryUl { get; set; }

        //public string AttrsValidationSummaryUl { get; set; }
        //private Dictionary<string, string> AttrsValidationSummaryUlDict
        //{
        //    get
        //    {
        //        return JsonDeserialize.JsonDeserializeConvert_Dss(AttrsValidationSummaryUl);
        //    }
        //}
        #endregion

        #region  Attributes

        public string AttrsTagOfProp { get; set; }
        public Dictionary<string, Dictionary<string, Dictionary<string, string>>> AttrsTagOfPropDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDssDss(AttrsTagOfProp);
            }
        }

        public string AttrsTag { get; set; }
        public Dictionary<string, Dictionary<string, string>> AttrsTagDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(AttrsTag);
            }
        }

        public string ContentTag { get; set; }
        private Dictionary<string, string> ContentTagDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_Dss(ContentTag);
            }
        }

        public string ContentTagOfProp { get; set; }
        private Dictionary<string, Dictionary<string, string>> ContentTagOfPropDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(ContentTagOfProp);
            }
        }
        #endregion

        #region ActiveTags
        public bool ActiveValidationSummary { get; set; } = true;
        public bool ActiveLabel { get; set; } = true;
        public bool ActiveValidation { get; set; } = true;
        public bool ActiveGroup { get; set; } = true;
        #endregion

        #region AttrsTags
        public string TagFormTitle { get; set; } = "title";
        public string TagValidationSum { get; set; } = "validation_sum";
        public string TagValidationGroup { get; set; } = "validation_group";
        public string TagValidation { get; set; } = "validation";

        public string TagFormGroup { get; set; } = "form_group";
        public string TagFormLabel { get; set; } = "label";
        public string TagFormInput { get; set; } = "input";

        public string TagCheckboxFieldSet { get; set; } = "group";
        public string TagCheckboxLabel { get; set; } = "label";
        public string TagCheckboxInput { get; set; } = "input";

        public string TagRadioFieldSet { get; set; } = "group";
        public string TagRadioLabel { get; set; } = "label";
        public string TagRadioInput { get; set; } = "input";


        public string TagSelectOption { get; set; } = "option";
        public string TagSelectInput { get; set; } = "select";

        public string TagColsListGroup { get; set; } = "cols_list_group";
        public string TagLabelDiv { get; set; } = "label_div";
        #endregion

        #region RadioButton 

        //public bool RadioRight { get; set; }
        public bool RadioLeft { get; set; }
        public bool RadioTop { get; set; }
        public bool RadioBottom { get; set; }
        public bool RadioColsList { get; set; }

        public string RadioButtonDataList { get; set; }
        private Dictionary<string, Dictionary<string, string>> RadioDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDss(RadioButtonDataList);
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
        public bool CheckboxTop { get; set; }
        public bool CheckboxBottom { get; set; }
        public bool CheckboxLeft { get; set; }
        public bool CheckboxColsList { get; set; }

        public string CheckboxDataList { get; set; }
        private Dictionary<string, Dictionary<string, bool>> CheckboxDataListDict
        {
            get
            {
                return JsonDeserialize.JsonDeserializeConvert_DsDsb(CheckboxDataList);
            }
        }
        #endregion

        public override async Task ProcessAsync(
            TagHelperContext context, TagHelperOutput output)
        {

            // Apply Title class and attrs
            TagBuilder title = new TagBuilder("div");

            if (FormTitle != null)
            {
                title.InnerHtml.SetHtmlContent(FormTitle);

                SetAttrsAndContent(title, TagFormTitle);
                //AttrsHelper.SetTagAttrs(AttrsTagDict, title, TagFormTitle);
                //AttrsHelper.SetTagContent(ContentTagDict, title, TagFormTitle, false);

                output.Content.AppendHtml(title);
            }

            // Apply Validation Summary class and attrs
            if (ActiveValidationSummary)
            {
                TagBuilder validation_sum = GenerateValidationSummary();
                //AttrsHelper.SetTagAttrs(
                //    AttrsTagDict, validation_sum, TagValidationSum);
                //AttrsHelper.SetTagContent(
                //    ContentTagDict, validation_sum, TagValidationSum, false);
                SetAttrsAndContent(validation_sum, TagValidationSum);


                output.Content.AppendHtml(validation_sum);
            }

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
                        // close complex type loop,because end of complex type's properties
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

                    // Set form group attributes and default value
                    TagBuilder form_group = new TagBuilder("div");
                    form_group.Attributes["class"] = "form-group";

                    SetAttrsAndContent(form_group, property_name, TagFormGroup);

                    //AttrsHelper.SetTagAttrs(AttrsTagDict, form_group, TagFormGroup);
                    //AttrsHelper.SetTagAttrsOfProp(
                    //    AttrsTagOfPropDict, form_group, TagFormGroup, property_name);

                    //AttrsHelper.SetTagContent(
                    //    ContentTagDict, form_group, TagFormGroup, false);
                    //AttrsHelper.SetTagContentOfProp(
                    //    ContentTagOfPropDict, form_group, property_name, TagFormGroup, false);

                    // Set label attributes and default value
                    TagBuilder label = Generator.GenerateLabel(
                        ViewContext,
                        property,
                        property.Metadata.PropertyName,
                        labelText: null,
                        htmlAttributes: null);

                    SetAttrsAndContent(label, property_name, TagFormLabel);
                    //AttrsHelper.SetTagAttrs(AttrsTagDict, label, TagFormLabel);
                    //AttrsHelper.SetTagAttrsOfProp(
                    //    AttrsTagOfPropDict, label, property_name, TagFormLabel);

                    //AttrsHelper.SetTagContent(
                    //    ContentTagDict, label, TagFormLabel, false);
                    //AttrsHelper.SetTagContentOfProp(
                    //    ContentTagOfPropDict, form_group, property_name, TagFormLabel, false);

                    TagBuilder input;
                    // Set special input tag
                    if (property.Metadata.DataTypeName == "Radio")
                    {
                        TagBuilder radioFieldSet = new TagBuilder("fieldset");
                        SetAttrsAndContent(radioFieldSet, property_name, TagRadioFieldSet);
                        //AttrsHelper.SetTagAttrs(AttrsTagDict, radioFieldSet, TagRadioFieldSet);
                        //AttrsHelper.SetTagAttrsOfProp(
                        //    AttrsTagOfPropDict, radioFieldSet, property_name, TagRadioFieldSet);

                        //AttrsHelper.SetTagContent(
                        //    ContentTagDict, radioFieldSet, TagRadioFieldSet, false);
                        //AttrsHelper.SetTagContentOfProp(
                        //    ContentTagOfPropDict, radioFieldSet, property_name, TagRadioFieldSet, false);

                        var radioTag =
                            RadioDict.FirstOrDefault(
                                prop => prop.Key.Equals(property_name,
                                StringComparison.OrdinalIgnoreCase)).Value;

                        if (RadioDict.Count != 0 &&
                            radioTag != null)
                        {
                            for (int i = 0; i < radioTag.Count; i++)
                            {
                                var item = radioTag.ElementAt(i);
                                // set tag key value number of sequence
                                var tag_number = (i + 1).ToString();

                                input = GenerateInputType(property, item.Key);
                                input.Attributes["id"] = "radio_label" + tag_number;

                                TagBuilder radio_label = new TagBuilder("label");
                                radio_label.Attributes["for"] = "radio_label" + tag_number;
                                // set radio label attrs
                                //AttrsHelper.SetTagAttrs(AttrsTagDict, radio_label, TagRadioLabel);
                                //AttrsHelper.SetTagAttrsOfProp(
                                //    AttrsTagOfPropDict, radio_label, property_name, TagRadioLabel, tag_number);

                                //AttrsHelper.SetTagContent(
                                //    ContentTagDict, radio_label, TagRadioLabel, false);
                                //AttrsHelper.SetTagContentOfProp(
                                //    ContentTagOfPropDict, radio_label, property_name, TagRadioLabel, tag_number, false);

                                SetAttrsAndContent(
                                    radio_label, property_name, TagRadioLabel, tag_number);

                                radio_label.InnerHtml.AppendHtml(item.Value);

                                // set radio input location
                                radioFieldSet = SetInputLocation(
                                    RadioTop, RadioBottom, RadioLeft,
                                    radio_label, input, radioFieldSet,
                                    property_name, tag_number, RadioColsList);
                            }
                        }
                        else
                        {
                            TagBuilder NoDataMsg = new TagBuilder("span");
                            NoDataMsg.MergeAttribute("style", "color:red;");
                            NoDataMsg.InnerHtml.SetHtmlContent(
                                "No radio button data,please give a data in your view");
                            radioFieldSet.InnerHtml.AppendHtml(NoDataMsg);
                        }

                        if (ActiveGroup)
                        {
                            if (ActiveLabel)
                            {
                                form_group.InnerHtml.AppendHtml(label);
                            }
                            form_group.InnerHtml.AppendHtml(radioFieldSet);
                        }
                        else
                        {
                            if (ActiveLabel)
                            {
                                output.Content.AppendHtml(label);
                            }
                            output.Content.AppendHtml(radioFieldSet);
                        }
                    }
                    // check if it's checkbox
                    else if (property.ModelType.Name == "Boolean")
                    {
                        var checkBoxTag =
                            CheckboxDataListDict.FirstOrDefault(
                                model => model.Key.Equals(property_name,
                                StringComparison.OrdinalIgnoreCase)).Value;

                        if (CheckboxDataListDict.Count() != 0 &&
                            checkBoxTag != null)
                        {
                            TagBuilder checkboxFieldSet = new TagBuilder("fieldSet");

                            //AttrsHelper.SetTagAttrs(
                            //    AttrsTagDict, checkboxFieldSet, TagCheckboxFieldSet);
                            //AttrsHelper.SetTagAttrsOfProp(
                            //    AttrsTagOfPropDict, checkboxFieldSet, property_name, TagCheckboxFieldSet);

                            //AttrsHelper.SetTagContent(
                            //    ContentTagDict, checkboxFieldSet, TagCheckboxFieldSet, false);
                            //AttrsHelper.SetTagContentOfProp(
                            //    ContentTagOfPropDict, checkboxFieldSet, property_name, TagCheckboxFieldSet, false);
                            SetAttrsAndContent(
                                checkboxFieldSet, property_name, TagCheckboxFieldSet);

                            for (int i = 0; i < checkBoxTag.Count; i++)
                            {
                                var item = checkBoxTag.ElementAt(i);
                                // Set tag key value number of sequence
                                var tag_number = (i + 1).ToString();

                                TagBuilder checkboxLabel = new TagBuilder("label");
                                checkboxLabel.Attributes["for"] = "checkbox_label" + tag_number;

                                //AttrsHelper.SetTagAttrs(AttrsTagDict, checkboxLabel, TagCheckboxLabel);
                                //AttrsHelper.SetTagAttrsOfProp(
                                //    AttrsTagOfPropDict, checkboxLabel, property_name,
                                //    TagCheckboxLabel, tag_number);

                                //AttrsHelper.SetTagContent(
                                //    ContentTagDict, checkboxLabel, TagCheckboxLabel, false);
                                //AttrsHelper.SetTagContentOfProp(
                                //    ContentTagOfPropDict, checkboxLabel, property_name,
                                //    TagCheckboxLabel, tag_number, false);
                                SetAttrsAndContent(
                                    checkboxLabel, property_name, TagCheckboxLabel, tag_number);

                                checkboxLabel.InnerHtml.AppendHtml(item.Key);

                                input = GenerateInputType(property);

                                input.Attributes["id"] = "checkbox_label" + tag_number;

                                checkboxFieldSet = SetInputLocation(
                                     CheckboxTop, CheckboxBottom, CheckboxLeft,
                                     checkboxLabel, input, checkboxFieldSet,
                                     property_name, tag_number, CheckboxColsList);
                            }

                            if (ActiveGroup)
                            {
                                if (ActiveLabel)
                                {
                                    form_group.InnerHtml.AppendHtml(label);
                                }
                                form_group.InnerHtml.AppendHtml(checkboxFieldSet);
                            }
                            else
                            {
                                if (ActiveLabel)
                                {
                                    output.Content.AppendHtml(label);
                                }
                                output.Content.AppendHtml(checkboxFieldSet);
                            }
                        }
                        else
                        {
                            input = GenerateInputType(property);

                            if (ActiveGroup)
                            {
                                if (ActiveLabel)
                                {
                                    form_group.InnerHtml.AppendHtml(label);
                                }
                                form_group.InnerHtml.AppendHtml(input);
                            }
                            else
                            {
                                if (ActiveLabel)
                                {
                                    output.Content.AppendHtml(label);
                                }
                                output.Content.AppendHtml(input);
                            }
                        }
                    }
                    else if (property.Metadata.DataTypeName == "Select")
                    {
                        input = GenerateInputType(property);
                        input.Attributes["class"] = "form-control";

                        //AttrsHelper.SetTagAttrs(AttrsTagDict, input, TagSelectInput);
                        //AttrsHelper.SetTagAttrsOfProp(
                        //    AttrsTagOfPropDict, input, property_name, TagSelectInput);

                        //AttrsHelper.SetTagContent(
                        //    ContentTagDict, input, TagSelectInput, false);
                        //AttrsHelper.SetTagContentOfProp(
                        //    ContentTagOfPropDict, input, property_name, TagSelectInput, false);

                        SetAttrsAndContent(input, property_name, TagSelectInput);

                        var selectModel =
                            SelectListDict.FirstOrDefault(
                                model => model.Key.Equals(
                                    property_name, StringComparison.OrdinalIgnoreCase))
                                    .Value;

                        if (selectModel != null &&
                              SelectListDict.Count() != 0)
                        {
                            for (int i = 0; i < selectModel.Count; i++)
                            {
                                var item = selectModel.ElementAt(i);
                                var tag_number = (i + 1).ToString();

                                TagBuilder option = new TagBuilder("option");

                                option.Attributes["value"] = item.Key;
                                option.InnerHtml.SetHtmlContent(item.Value);

                                //AttrsHelper.SetTagAttrs(
                                //    AttrsTagDict, option, TagSelectOption);
                                //AttrsHelper.SetTagAttrsOfProp(
                                //    AttrsTagOfPropDict, option, property_name,
                                //    TagSelectOption, tag_number);

                                //AttrsHelper.SetTagContent(
                                //    ContentTagDict, option, TagSelectOption, false);
                                //AttrsHelper.SetTagContentOfProp(
                                //    ContentTagOfPropDict, option, property_name,
                                //    TagSelectOption, tag_number, false);
                                SetAttrsAndContent(
                                    option, property_name, TagSelectOption, tag_number);

                                input.InnerHtml.AppendHtml(option);
                            }

                            if (ActiveGroup)
                            {
                                if (ActiveLabel)
                                {
                                    form_group.InnerHtml.AppendHtml(label);
                                }
                                form_group.InnerHtml.AppendHtml(input);
                            }
                            else
                            {
                                if (ActiveLabel)
                                {
                                    output.Content.AppendHtml(label);
                                }
                                output.Content.AppendHtml(input);
                            }
                        }
                        else
                        {
                            TagBuilder NoDataMsg = new TagBuilder("div");
                            NoDataMsg.MergeAttribute("style", "color:red;");
                            NoDataMsg.InnerHtml.SetHtmlContent(
                                "No select list data,please give a data in your view");
                            if (ActiveGroup)
                            {
                                if (ActiveLabel)
                                {
                                    form_group.InnerHtml.AppendHtml(label);
                                }
                                form_group.InnerHtml.AppendHtml(NoDataMsg);
                            }
                            else
                            {
                                if (ActiveLabel)
                                {
                                    output.Content.AppendHtml(label);
                                }
                                output.Content.AppendHtml(NoDataMsg);
                            }
                        }
                    }
                    else
                    {
                        input = GenerateInputType(property);

                        input.Attributes["class"] = "form-control";

                        if (ActiveGroup)
                        {
                            if (ActiveLabel)
                            {
                                form_group.InnerHtml.AppendHtml(label);
                            }
                            form_group.InnerHtml.AppendHtml(input);
                        }
                        else
                        {
                            if (ActiveLabel)
                            {
                                output.Content.AppendHtml(label);
                            }
                            output.Content.AppendHtml(input);
                        }
                    }

                    if (ActiveGroup)
                    {
                        if (ActiveValidation)
                        {
                            TagBuilder span = Generator.GenerateValidationMessage(
                                                    ViewContext,
                                                    property,
                                                    property.Metadata.PropertyName,
                                                    message: null,
                                                    tag: null,
                                                    htmlAttributes: null);
                            //AttrsHelper.SetTagAttrs(AttrsTagDict, span, TagValidation);
                            //AttrsHelper.SetTagAttrsOfProp(
                            //    AttrsTagOfPropDict, span, property_name, TagValidation);

                            //AttrsHelper.SetTagContent(
                            //    ContentTagDict, span, TagValidation, false);
                            //AttrsHelper.SetTagContentOfProp(
                            //    ContentTagOfPropDict, span, property_name, TagValidation, false);

                            SetAttrsAndContent(
                                span, property_name, TagValidation);
                            /*---------------End print your model----------------*/

                            form_group.InnerHtml.AppendHtml(span);
                        }
                    }
                    else
                    {
                        if (ActiveValidation)
                        {
                            TagBuilder span = Generator.GenerateValidationMessage(
                                                    ViewContext,
                                                    property,
                                                    property.Metadata.PropertyName,
                                                    message: null,
                                                    tag: null,
                                                    htmlAttributes: null);
                            //AttrsHelper.SetTagAttrs(AttrsTagDict, span, TagValidation);
                            //AttrsHelper.SetTagAttrsOfProp(
                            //    AttrsTagOfPropDict, span, property_name, TagValidation);

                            //AttrsHelper.SetTagContent(
                            //    ContentTagDict, span, TagValidation, false);
                            //AttrsHelper.SetTagContentOfProp(
                            //    ContentTagOfPropDict, span, property_name, TagValidation, false);

                            SetAttrsAndContent(
                                span, property_name, TagValidation);

                            /*---------------End print your model----------------*/

                            output.Content.AppendHtml(span);
                        }
                    }

                    if (ActiveGroup)
                    {
                        output.Content.AppendHtml(form_group);
                    }

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
            var property_name = modelExplorer.Metadata.PropertyName;

            switch (inputType)
            {
                case "hidden":
                    Input = GenerateHidden(modelExplorer);
                    //AttrsHelper.SetTagAttrs(AttrsTagDict, Input, TagFormInput);
                    //AttrsHelper.SetTagAttrsOfProp(
                    //    AttrsTagOfPropDict, Input, property_name, TagFormInput);

                    //AttrsHelper.SetTagContent(
                    //    ContentTagDict, Input, TagFormInput, false);
                    //AttrsHelper.SetTagContentOfProp(
                    //    ContentTagOfPropDict, Input,
                    //    property_name, TagFormInput, false);
                    SetAttrsAndContent(Input, property_name, TagFormInput);

                    break;

                case "checkbox":
                    Input = Generator.GenerateCheckBox(
                        ViewContext,
                        modelExplorer,
                        modelExplorer.Metadata.PropertyName,
                        isChecked: null,
                        htmlAttributes: null);

                    //AttrsHelper.SetTagAttrs(AttrsTagDict, Input, TagCheckboxInput);
                    //AttrsHelper.SetTagAttrsOfProp(
                    //    AttrsTagOfPropDict, Input,
                    //    property_name, TagCheckboxInput);

                    //AttrsHelper.SetTagContent(
                    //    ContentTagDict, Input, TagCheckboxInput, false);
                    //AttrsHelper.SetTagContentOfProp(
                    //    ContentTagOfPropDict, Input,
                    //    property_name, TagCheckboxInput, false);
                    SetAttrsAndContent(Input, property_name, TagCheckboxInput);

                    break;

                case "password":
                    Input = Generator.GeneratePassword(
                        ViewContext,
                        modelExplorer,
                        modelExplorer.Metadata.PropertyName,
                        value: null,
                        htmlAttributes: null);
                    //AttrsHelper.SetTagAttrs(AttrsTagDict, Input, TagFormInput);
                    //AttrsHelper.SetTagAttrsOfProp(
                    //    AttrsTagOfPropDict, Input,
                    //    property_name, TagFormInput);

                    //AttrsHelper.SetTagContent(
                    //    ContentTagDict, Input, TagFormInput, false);
                    //AttrsHelper.SetTagContentOfProp(
                    //    ContentTagOfPropDict, Input,
                    //    property_name, TagFormInput, false);
                    SetAttrsAndContent(Input, property_name, TagFormInput);

                    break;

                case "radio":
                    Input = Generator.GenerateRadioButton(
                                           ViewContext,
                                           modelExplorer,
                                           modelExplorer.Metadata.PropertyName,
                                           value: radioValue,
                                           isChecked: null,
                                           htmlAttributes: null);
                    //AttrsHelper.SetTagAttrs(AttrsTagDict, Input, TagRadioInput);
                    //AttrsHelper.SetTagAttrsOfProp(
                    //    AttrsTagOfPropDict, Input,
                    //    property_name, TagRadioInput);

                    //AttrsHelper.SetTagContent(
                    //    ContentTagDict, Input, TagRadioInput, false);
                    //AttrsHelper.SetTagContentOfProp(
                    //    ContentTagOfPropDict, Input,
                    //    property_name, TagRadioInput, false);
                    SetAttrsAndContent(Input, property_name, TagRadioInput);
                    break;

                case "select":
                    Input = GenerateSelectList(modelExplorer, inputTypeHint);
                    //AttrsHelper.SetTagAttrs(AttrsTagDict, Input, TagSelectInput);
                    //AttrsHelper.SetTagAttrsOfProp(
                    //    AttrsTagOfPropDict, Input,
                    //    property_name, TagSelectInput);

                    //AttrsHelper.SetTagContent(
                    //    ContentTagDict, Input, TagSelectInput, false);
                    //AttrsHelper.SetTagContentOfProp(
                    //    ContentTagOfPropDict, Input,
                    //    property_name, TagSelectInput, false);
                    SetAttrsAndContent(Input, property_name, TagSelectInput);

                    break;

                case "textarea":
                    Input = Generator.GenerateTextArea(
                        ViewContext,
                        modelExplorer,
                        modelExplorer.Metadata.PropertyName,
                        rows: 0,
                        columns: 0,
                        htmlAttributes: null);
                    //AttrsHelper.SetTagAttrs(AttrsTagDict, Input, TagFormInput);
                    //AttrsHelper.SetTagAttrsOfProp(
                    //    AttrsTagOfPropDict, Input,
                    //    property_name, TagFormInput);

                    //AttrsHelper.SetTagContent(
                    //    ContentTagDict, Input, TagFormInput, false);
                    //AttrsHelper.SetTagContentOfProp(
                    //    ContentTagOfPropDict, Input,
                    //    property_name, TagFormInput, false);
                    SetAttrsAndContent(Input, property_name, TagFormInput);

                    break;

                default:

                    Input = GenerateTextBox(
                        modelExplorer,
                        inputTypeHint,
                        inputType);

                    //AttrsHelper.SetTagAttrs(AttrsTagDict, Input, TagFormInput);
                    //AttrsHelper.SetTagAttrsOfProp(
                    //    AttrsTagOfPropDict, Input, property_name, TagFormInput);

                    //AttrsHelper.SetTagContent(
                    // ContentTagDict, Input, TagFormInput, false);
                    //AttrsHelper.SetTagContentOfProp(
                    //    ContentTagOfPropDict, Input,
                    //    property_name, TagFormInput, false);
                    SetAttrsAndContent(Input, property_name, TagFormInput);

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
            //validation_ul.AddCssClass(ClassValidationSummaryUl);
            AttrsHelper.SetTagAttrs(AttrsTagDict, validation_ul, TagValidationGroup);
            //SetGlobalTagsAttrs(validation_ul, TagValidationGroup);
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


        private TagBuilder SetInputLocation(
            bool top, bool bottom, bool left,
            TagBuilder label, TagBuilder input, TagBuilder group,
            string property_name, string value_num, bool column)

        {
            if (top &&
                !bottom &&
                !left)
            {
                TagBuilder top_div = new TagBuilder("div");

                AttrsHelper.SetTagAttrs(AttrsTagDict, top_div, TagLabelDiv);
                AttrsHelper.SetTagAttrsOfProp(
                    AttrsTagOfPropDict, top_div, property_name,
                    TagLabelDiv, value_num);

                AttrsHelper.SetTagContent(
                    ContentTagDict, top_div, TagLabelDiv, false);
                AttrsHelper.SetTagContentOfProp(
                    ContentTagOfPropDict, top_div, property_name,
                    TagLabelDiv, value_num, false);

                top_div.InnerHtml.AppendHtml(label);

                if (column)
                {
                    TagBuilder cols_list_div = new TagBuilder("div");

                    AttrsHelper.SetTagAttrs(
                        AttrsTagDict, cols_list_div, TagColsListGroup);
                    AttrsHelper.SetTagAttrsOfProp(
                        AttrsTagOfPropDict, cols_list_div, property_name,
                        TagColsListGroup, value_num);

                    AttrsHelper.SetTagContent(
                        ContentTagDict, cols_list_div, TagColsListGroup, false);
                    AttrsHelper.SetTagContentOfProp(
                        ContentTagOfPropDict, cols_list_div, property_name,
                        TagColsListGroup, value_num, false);


                    cols_list_div.InnerHtml.AppendHtml(input);
                    cols_list_div.InnerHtml.AppendHtml(top_div);
                    group.InnerHtml.AppendHtml(cols_list_div);
                    return group;
                }
                group.InnerHtml.AppendHtml(input);
                group.InnerHtml.AppendHtml(top_div);
                return group;
            }
            else if (!top &&
                bottom &&
                !left)
            {
                TagBuilder bottom_div = new TagBuilder("div");
                AttrsHelper.SetTagAttrs(AttrsTagDict, bottom_div, TagColsListGroup);
                AttrsHelper.SetTagAttrsOfProp(
                    AttrsTagOfPropDict, bottom_div, property_name,
                    TagColsListGroup, value_num);

                AttrsHelper.SetTagContent(
                    ContentTagDict, bottom_div, TagColsListGroup, false);
                AttrsHelper.SetTagContentOfProp(
                    ContentTagOfPropDict, bottom_div, property_name,
                    TagColsListGroup, value_num, false);



                bottom_div.InnerHtml.AppendHtml(label);

                if (column)
                {
                    TagBuilder cols_list_div = new TagBuilder("div");
                    AttrsHelper.SetTagAttrs(AttrsTagDict, cols_list_div, TagColsListGroup);
                    AttrsHelper.SetTagAttrsOfProp(
                        AttrsTagOfPropDict, cols_list_div, property_name,
                        TagColsListGroup, value_num);

                    AttrsHelper.SetTagContent(
                        ContentTagDict, cols_list_div, TagColsListGroup, false);
                    AttrsHelper.SetTagContentOfProp(
                        ContentTagOfPropDict, cols_list_div, property_name,
                        TagColsListGroup, value_num, false);



                    cols_list_div.InnerHtml.AppendHtml(bottom_div);
                    cols_list_div.InnerHtml.AppendHtml(input);
                    group.InnerHtml.AppendHtml(cols_list_div);
                    return group;
                }
                group.InnerHtml.AppendHtml(bottom_div);
                group.InnerHtml.AppendHtml(input);
                return group;
            }
            else if (!top &&
                !bottom &&
                left)
            {
                if (column)
                {
                    TagBuilder cols_list_div = new TagBuilder("div");
                    AttrsHelper.SetTagAttrs(AttrsTagDict, cols_list_div, TagColsListGroup);
                    AttrsHelper.SetTagAttrsOfProp(
                        AttrsTagOfPropDict, cols_list_div, property_name,
                        TagColsListGroup, value_num);

                    AttrsHelper.SetTagContent(
                       ContentTagDict, cols_list_div, TagColsListGroup, false);
                    AttrsHelper.SetTagContentOfProp(
                        ContentTagOfPropDict, cols_list_div, property_name,
                        TagColsListGroup, value_num, false);

                    cols_list_div.InnerHtml.AppendHtml(input);
                    cols_list_div.InnerHtml.AppendHtml(label);
                    group.InnerHtml.AppendHtml(cols_list_div);
                    return group;
                }
                group.InnerHtml.AppendHtml(input);
                group.InnerHtml.AppendHtml(label);
                return group;
            }
            else
            {
                if (column)
                {
                    TagBuilder cols_list_div = new TagBuilder("div");
                    AttrsHelper.SetTagAttrs(AttrsTagDict, cols_list_div, TagColsListGroup);
                    AttrsHelper.SetTagAttrsOfProp(
                        AttrsTagOfPropDict, cols_list_div, property_name,
                        TagColsListGroup, value_num);

                    AttrsHelper.SetTagContent(
                       ContentTagDict, cols_list_div, TagColsListGroup, false);
                    AttrsHelper.SetTagContentOfProp(
                        ContentTagOfPropDict, cols_list_div, property_name,
                        TagColsListGroup, value_num, false);

                    cols_list_div.InnerHtml.AppendHtml(label);
                    cols_list_div.InnerHtml.AppendHtml(input);
                    group.InnerHtml.AppendHtml(cols_list_div);
                    return group;
                }
                group.InnerHtml.AppendHtml(label);
                group.InnerHtml.AppendHtml(input);
                return group;
            }
        }

        private void SetAttrsAndContent(
            TagBuilder tag, string tag_name)
        {
            AttrsHelper.SetTagAttrs(AttrsTagDict, tag, tag_name);
            AttrsHelper.SetTagContent(ContentTagDict, tag, tag_name, false);
        }

        private void SetAttrsAndContent(
            TagBuilder tag, string property_name, string tag_name)
        {
            AttrsHelper.SetTagAttrs(AttrsTagDict, tag, tag_name);
            AttrsHelper.SetTagAttrsOfProp(
                AttrsTagOfPropDict, tag, tag_name, property_name);

            AttrsHelper.SetTagContent(
                ContentTagDict, tag, tag_name, false);
            AttrsHelper.SetTagContentOfProp(
                ContentTagOfPropDict, tag, property_name, tag_name, false);
        }

        private void SetAttrsAndContent(
            TagBuilder tag,
            string property_name,
            string tag_name,
            string index)
        {
            AttrsHelper.SetTagAttrs(AttrsTagDict, tag, tag_name);
            AttrsHelper.SetTagAttrsOfProp(
                AttrsTagOfPropDict, tag, property_name, tag_name, index);

            AttrsHelper.SetTagContent(
                ContentTagDict, tag, tag_name, false);
            AttrsHelper.SetTagContentOfProp(
                ContentTagOfPropDict, tag, property_name, tag_name, index, false);
        }
        #endregion
    }
}

