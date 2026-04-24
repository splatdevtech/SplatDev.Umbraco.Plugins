using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor.Internal;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.Hosting;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

using System.Runtime.CompilerServices;
using System.Text.Encodings.Web;

using Umbraco.Cms.Web.Common.Views;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Models;

namespace UmbracoForms.StaticAssets
{
    //[RazorSourceChecksum("Sha256", "db9c3980d212bfb98e6a15f9c15118e606896b95290e5059dcd36dda3ecafa3c", "/Views/Partials/Forms/Emails/Example-Template.cshtml")]
    [RazorCompiledItemMetadata("Identifier", "/Views/Partials/Forms/Emails/Example-Template.cshtml")]
    [CreateNewOnMetadataUpdate]
    internal sealed class Views_Partials_Forms_Emails_Example_Template :
      UmbracoViewPage<FormsHtmlModel>
    {
        private static readonly TagHelperAttribute? __tagHelperAttribute_0 = new("style", new HtmlString("background-color: #f4f4f4; margin: 0 !important; padding: 0 !important;"), HtmlAttributeValueStyle.DoubleQuotes);
        private TagHelperExecutionContext? __tagHelperExecutionContext;
        private readonly TagHelperRunner __tagHelperRunner = new();
        private TagHelperScopeManager? __backed__tagHelperScopeManager;
        private HeadTagHelper? __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper;
        private BodyTagHelper? __Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper;

        private TagHelperScopeManager TagHelperScopeManager
        {
            get
            {
                __backed__tagHelperScopeManager ??= new TagHelperScopeManager(new Action<HtmlEncoder>(StartTagHelperWritingScope), new Func<TagHelperContent>(EndTagHelperWritingScope));
                return __backed__tagHelperScopeManager;
            }
        }

        public override async Task ExecuteAsync()
        {
            Views_Partials_Forms_Emails_Example_Template? emailsExampleTemplate = this;
            emailsExampleTemplate.WriteLiteral("\n");
            string? assetUrl = emailsExampleTemplate.Context.Request.Scheme + "://" + emailsExampleTemplate.Context.Request.Host.ToString() + "/App_Plugins/UmbracoForms/assets/Email-Example";
            emailsExampleTemplate.WriteLiteral("<!DOCTYPE html>\n<html>\n");
            emailsExampleTemplate.__tagHelperExecutionContext = emailsExampleTemplate.TagHelperScopeManager.Begin("head", TagMode.StartTagAndEndTag, "db9c3980d212bfb98e6a15f9c15118e606896b95290e5059dcd36dda3ecafa3c5096", async () =>
            {
                WriteLiteral("\n    <title></title>\n    <meta http-equiv=\"Content-Type\" content=\"text/html; charset=utf-8\" />\n    <meta name=\"viewport\" content=\"width=device-width, initial-scale=1\">\n    <meta http-equiv=\"X-UA-Compatible\" content=\"IE=edge\" />\n    <style type=\"text/css\">\n\n        /* CLIENT-SPECIFIC STYLES */\n        body, table, td, a {\n            -webkit-text-size-adjust: 100%;\n            -ms-text-size-adjust: 100%;\n        }\n\n        table, td {\n            mso-table-lspace: 0pt;\n            mso-table-rspace: 0pt;\n        }\n\n        img {\n            -ms-interpolation-mode: bicubic;\n        }\n\n        /* RESET STYLES */\n        img {\n            border: 0;\n            height: auto;\n            line-height: 100%;\n            outline: none;\n            text-decoration: none;\n        }\n\n        table {\n            border-collapse: collapse !important;\n        }\n\n        body {\n            height: 100% !important;\n            margin: 0 !important;\n            padding: 0 !important;\n            width: 100% !important;\n       ");
                WriteLiteral(" }\n\n        /* iOS BLUE LINKS */\n        a[x-apple-data-detectors] {\n            color: inherit !important;\n            text-decoration: none !important;\n            font-size: inherit !important;\n            font-family: inherit !important;\n            font-weight: inherit !important;\n            line-height: inherit !important;\n        }\n\n        /* MOBILE STYLES */\n        ");
                WriteLiteral("@media screen and (max-width:600px) {\n            h1 {\n                font-size: 32px !important;\n                line-height: 32px !important;\n            }\n        }\n\n        /* ANDROID CENTER FIX */\n        div[style*=\"margin: 16px 0;\"] {\n            margin: 0 !important;\n        }\n    </style>\n");
                await Task.CompletedTask;
            });
            emailsExampleTemplate.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper = emailsExampleTemplate.CreateTagHelper<HeadTagHelper>();
            emailsExampleTemplate.__tagHelperExecutionContext.Add(emailsExampleTemplate.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_HeadTagHelper);
            await emailsExampleTemplate.__tagHelperRunner.RunAsync(emailsExampleTemplate.__tagHelperExecutionContext);
            if (!emailsExampleTemplate.__tagHelperExecutionContext.Output.IsContentModified)
                await emailsExampleTemplate.__tagHelperExecutionContext.SetOutputContentAsync();
            emailsExampleTemplate.Write(emailsExampleTemplate.__tagHelperExecutionContext.Output);
            emailsExampleTemplate.__tagHelperExecutionContext = emailsExampleTemplate.TagHelperScopeManager.End();
            emailsExampleTemplate.WriteLiteral("\n");
            emailsExampleTemplate.__tagHelperExecutionContext = emailsExampleTemplate.TagHelperScopeManager.Begin("body", TagMode.StartTagAndEndTag, "db9c3980d212bfb98e6a15f9c15118e606896b95290e5059dcd36dda3ecafa3c7856", async () =>
            {
                WriteLiteral("\n    <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"margin-bottom: 40px;\">\n        <!-- LOGO -->\n        <tr>\n            <td bgcolor=\"#413659\" align=\"center\">\n                <!--[if (gte mso 9)|(IE)]>\n                    <table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"600\">\n                    <tr>\n                    <td align=\"center\" valign=\"top\" width=\"600\">\n                <![endif]-->\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\n                    <tr>\n                        <td align=\"center\" valign=\"top\" style=\"padding: 40px 10px 40px 10px;\">\n                            <a href=\"http://umbraco.com\" target=\"_blank\">\n                                <img alt=\"Logo\"");
                BeginWriteAttribute("src", " src=\"", 3381, "\"", 3413, 2);
                WriteAttributeValue("", 3387, assetUrl, 3387, 9, false);
                WriteAttributeValue("", 3396, "/umbraco-logo.png", 3396, 17, true);
                EndWriteAttribute();
                WriteLiteral(" width=\"40\" height=\"40\" style=\"display: block; width: 40px; max-width: 40px; min-width: 40px; font-family: Helvetica, Arial, sans-serif; color: #ffffff; font-size: 18px;\" border=\"0\">\n                            </a>\n                        </td>\n                    </tr>\n                </table>\n                <!--[if (gte mso 9)|(IE)]>\n                    </td>\n                    </tr>\n                    </table>\n                <![endif]-->\n            </td>\n        </tr>\n\n        <!-- HERO -->\n        <tr>\n            <td bgcolor=\"#413659\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\">\n                <!--[if (gte mso 9)|(IE)]>\n                    <table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"600\">\n                    <tr>\n                    <td align=\"center\" valign=\"top\" width=\"600\">\n                <![endif]-->\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\n                    <tr>\n                        <td b");
                WriteLiteral("gcolor=\"#ffffff\" align=\"center\" valign=\"top\" style=\"padding: 40px 20px 20px 20px; color: #000000; font-family: Helvetica, Arial, sans-serif; font-size: 36px; font-weight: 900; line-height: 48px;\">\n                            <h1 style=\"font-size: 36px; font-weight: 900; margin: 0;\">Umbraco Forms</h1>\n                        </td>\n                    </tr>\n                </table>\n                <!--[if (gte mso 9)|(IE)]>\n                    </td>\n                    </tr>\n                    </table>\n                <![endif]-->\n            </td>\n        </tr>\n\n        <!-- COPY BLOCK -->\n        <tr>\n            <td bgcolor=\"#F3F3F5\" align=\"center\" style=\"padding: 0px 10px 0px 10px;\">\n                <!--[if (gte mso 9)|(IE)]>\n                    <table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"600\">\n                    <tr>\n                    <td align=\"center\" valign=\"top\" width=\"600\">\n                <![endif]-->\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" wi");
                WriteLiteral("dth=\"100%\" style=\"max-width: 600px;\">\n\n                    <!-- COPY -->\n                    <tr>\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 40px 30px; color: #303033; font-family: Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 1.6em;\">\n                            This is an example email template from Umbraco Forms Razor based email templates. You can build forms using any HTML markup you wish.\n                        </td>\n                    </tr>\n\n                    <!-- IMAGE -->\n                    <tr>\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 0;\">\n                            <a href=\"http://umbraco.com\" target=\"_blank\">\n                                <img alt=\"CodeGarden16 Attendees\"");
                BeginWriteAttribute("src", " src=\"", 6270, "\"", 6302, 2);
                WriteAttributeValue("", 6276, assetUrl, 6276, 9, false);
                WriteAttributeValue("", 6285, "/sample-image.jpg", 6285, 17, true);
                EndWriteAttribute();
                WriteLiteral(" width=\"600\" style=\"display: block; width: 100%; max-width: 100%; min-width: 100px;\" border=\"0\" />\n                            </a>\n                        </td>\n                    </tr>\n\n                    <!-- HEADER COPY -->\n");
                if (Model.HeaderHtml is not null)
                {
                    WriteLiteral("                        <tr>\n                            <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 40px 30px; color: #303033; font-family: Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 1.6em;\">\n                                ");
                    Write(Model.HeaderHtml);
                    WriteLiteral("\n                            </td>\n                        </tr>\n");
                }
                WriteLiteral("\n                    <!-- BODY COPY -->\n");
                if (Model.BodyHtml is not null)
                {
                    WriteLiteral("                        <tr>\n                            <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 40px 30px; color: #303033; font-family: Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 1.6em;\">\n                                ");
                    Write(Model.BodyHtml);
                    WriteLiteral("\n                            </td>\n                        </tr>\n");
                }
                WriteLiteral("\n                    <!-- FORM FIELDS HEADING -->\n                    <tr>\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 40px 30px 0px 30px; color: #000000; font-family: Helvetica, Arial, sans-serif; line-height: 25px;\">\n                            <h2 style=\"font-size: 24px; font-weight: 700; margin: 0;\">Form Results</h2>\n                        </td>\n                    </tr>\n\n                    <!-- FORM FIELDS -->\n                    <tr>\n                        <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 40px 30px; color: #303033; font-family: Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\n\n");
                string[]? ignoreFields =
                    [
                      "FieldType.Recaptcha2.cshtml",
                      "FieldType.Recaptcha3.cshtml",
                      "FieldType.RichText.cshtml",
                      "FieldType.Text.cshtml"
                    ];
                WriteLiteral("\n");
                foreach (FormFieldHtmlModel formFieldHtmlModel in Model.Fields.Where(x => !ignoreFields.Contains(x.FieldType)))
                {
                    WriteLiteral("                                <h4 style=\"font-weight: 700; margin: 0; color: #000000;\">");
                    Write(formFieldHtmlModel.Name);
                    WriteLiteral("</h4>\n");
                    WriteLiteral("                                <p style=\"margin-top: 0;\">\n");
                    string? fieldType = formFieldHtmlModel.FieldType;
                    if (!(fieldType == "FieldType.FileUpload.cshtml"))
                    {
                        if (fieldType == "FieldType.DatePicker.cshtml")
                        {
                            object? obj = formFieldHtmlModel.GetValue();
                            if (obj is not null && !string.IsNullOrEmpty(obj.ToString()))
                            {
                                Write(DateTime.TryParse(obj is not null ? obj.ToString() : string.Empty, out DateTime result) ? result.ToString("f") : "");
                            }
                        }
                        else
                        {
                            object[]? values = formFieldHtmlModel.GetValues();
                            if (values is not null)
                            {
                                foreach (object obj in values)
                                {
                                    if (obj is not null)
                                    {
                                        Write(obj is string ? obj.ToString()?.ApplyPrevalueCaptions(formFieldHtmlModel.Id, Model.PrevalueMaps!) : obj);
                                        WriteLiteral("<br />\n");
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        int? num = 0;
                        foreach (object obj in formFieldHtmlModel.GetValues())
                        {
                            if (obj is not null && !string.IsNullOrEmpty(obj.ToString()))
                                ++num;
                        }
                        if (num > 0)
                        {
                            WriteLiteral("                                                <span>");
                            Write(num);
                            WriteLiteral(" file");
                            Write(num == 1 ? string.Empty : "s");
                            WriteLiteral(" uploaded</span>\n");
                        }
                    }
                    WriteLiteral("                                </p>\n");
                }
                WriteLiteral("\n                        </td>\n                    </tr>\n\n                    <!-- FOOTER COPY -->\n");
                if (Model.FooterHtml is not null)
                {
                    WriteLiteral("                        <tr>\n                            <td bgcolor=\"#ffffff\" align=\"left\" style=\"padding: 20px 30px 40px 30px; color: #303033; font-family: Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 1.6em;\">\n                                ");
                    Write(Model.FooterHtml);
                    WriteLiteral("\n                            </td>\n                        </tr>\n");
                }
                WriteLiteral("                </table>\n                <!--[if (gte mso 9)|(IE)]>\n                    </td>\n                    </tr>\n                    </table>\n                <![endif]-->\n            </td>\n        </tr>\n\n        <!-- SUPPORT CALLOUT -->\n        <tr>\n            <td bgcolor=\"#F3F3F5\" align=\"center\" style=\"padding: 30px 10px 0px 10px;\">\n                <!--[if (gte mso 9)|(IE)]>\n                    <table align=\"center\" border=\"0\" cellspacing=\"0\" cellpadding=\"0\" width=\"600\">\n                    <tr>\n                    <td align=\"center\" valign=\"top\" width=\"600\">\n                <![endif]-->\n                <table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" width=\"100%\" style=\"max-width: 600px;\">\n                    <!-- HEADLINE -->\n                    <tr>\n                        <td bgcolor=\"#03BFB3\" align=\"center\" style=\"padding: 30px 30px 30px 30px; color: #ffffff; font-family: Helvetica, Arial, sans-serif; font-size: 18px; font-weight: 400; line-height: 25px;\">\n                            <h2 style=");
                WriteLiteral("\"font-size: 20px; font-weight: 700; color: #ffffff; margin: 0; margin-bottom: 5px;\">Need more help?</h2>\n                            <p style=\"margin: 0;\"><a href=\"https://docs.umbraco.com/umbraco-forms\" target=\"_blank\" style=\"color: #ffffff;\">Find our documentation here</a></p>\n                        </td>\n                    </tr>\n                </table>\n                <!--[if (gte mso 9)|(IE)]>\n                    </td>\n                    </tr>\n                    </table>\n                <![endif]-->\n            </td>\n        </tr>\n\n    </table>\n");
                await Task.CompletedTask;
            });
            emailsExampleTemplate.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper = emailsExampleTemplate.CreateTagHelper<BodyTagHelper>();
            emailsExampleTemplate.__tagHelperExecutionContext.Add(emailsExampleTemplate.__Microsoft_AspNetCore_Mvc_Razor_TagHelpers_BodyTagHelper);
            emailsExampleTemplate.__tagHelperExecutionContext.AddHtmlAttribute(__tagHelperAttribute_0);
            await emailsExampleTemplate.__tagHelperRunner.RunAsync(emailsExampleTemplate.__tagHelperExecutionContext);
            if (!emailsExampleTemplate.__tagHelperExecutionContext.Output.IsContentModified)
                await emailsExampleTemplate.__tagHelperExecutionContext.SetOutputContentAsync();
            emailsExampleTemplate.Write(emailsExampleTemplate.__tagHelperExecutionContext.Output);
            emailsExampleTemplate.__tagHelperExecutionContext = emailsExampleTemplate.TagHelperScopeManager.End();
            emailsExampleTemplate.WriteLiteral("\n</html>\n");
        }

        [RazorInject]
        public IModelExpressionProvider? ModelExpressionProvider { get; private set; }

        [RazorInject]
        public IUrlHelper? Url { get; private set; }

        [RazorInject]
        public IViewComponentHelper? Component { get; private set; }

        [RazorInject]
        public IJsonHelper? Json { get; private set; }

        [RazorInject]
        public IHtmlHelper<object>? Html { get; private set; }
    }
}