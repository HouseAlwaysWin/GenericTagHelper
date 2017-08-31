using GenericTagHelper;
using GenericTagHelperUnitTest.TestCommon;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Routing;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace GenericTagHelperUnitTest
{
    public class GenericFormTagHelperTest
    {
        public static TheoryData MutiAttributeFormGroupData
        {
            get
            {
                return new TheoryData<TagHelperAttributeList, string>
                {
                    {
                        new TagHelperAttributeList
                        {
                            { "hello", "world" },
                            { "hello", "world2" }
                        },
                        "hello=\"HtmlEncode[[world]]\" hello=\"HtmlEncode[[world2]]\""
                    },
                    {
                        new TagHelperAttributeList
                        {
                            { "hello", "world" },
                            { "hello", "world2" },
                            { "hello", "world3" }
                        },
                        "hello=\"HtmlEncode[[world]]\" hello=\"HtmlEncode[[world2]]\" hello=\"HtmlEncode[[world3]]\""
                    },
                    {
                        new TagHelperAttributeList
                        {
                            { "HelLO", "world" },
                            { "HELLO", "world2" }
                        },
                        "HelLO=\"HtmlEncode[[world]]\" HELLO=\"HtmlEncode[[world2]]\""
                    },
                    {
                        new TagHelperAttributeList
                        {
                            { "Hello", "world" },
                            { "HELLO", "world2" },
                            { "hello", "world3" }
                        },
                        "Hello=\"HtmlEncode[[world]]\" HELLO=\"HtmlEncode[[world2]]\" hello=\"HtmlEncode[[world3]]\""
                    },
                    {
                        new TagHelperAttributeList
                        {
                            { "HeLlO", "world" },
                            { "hello", "world2" }
                        },
                        "HeLlO=\"HtmlEncode[[world]]\" hello=\"HtmlEncode[[world2]]\""
                    },
                };
            }
        }

        [Fact]
        public void Process_FormGroupLabel_GenerateExceptedOutput()
        {
            var metadataProvider = new EmptyModelMetadataProvider();
            var htmlGenerator = new TestableHtmlGenerator(metadataProvider);
            var viewContext = TestableHtmlGenerator.GetViewContext(
                model: null,
                htmlGenerator: htmlGenerator,
                metadataProvider: metadataProvider);
            var urlHelper = new Mock<IUrlHelper>();
            urlHelper
                .Setup(urlhelper => urlhelper.Content(It.IsAny<string>()))
                .Returns(new Func<string, string>(url => url.Replace("~/", "virtualRoot/")));
            var urlHelperFactory = new Mock<IUrlHelperFactory>();
            urlHelperFactory
                .Setup(f => f.GetUrlHelper(It.IsAny<ActionContext>()))
                .Returns(urlHelper.Object);

            var context = new TagHelperContext(
                tagName: "label",
                allAttributes: new TagHelperAttributeList(
                    Enumerable.Empty<TagHelperAttribute>()),
                items:new Dictionary<object,object>(),
                uniqueId:"test");

            GenericFormTagHelper formTaghelper = 
                new GenericFormTagHelper(urlHelperFactory.Object,htmlGenerator);

            formTaghelper.AttrsAllFormGroup = "form-group";
            
          
        }

     
    }
}
