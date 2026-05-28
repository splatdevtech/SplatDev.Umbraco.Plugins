using FormBuilder.Extension.Interfaces;
using FormBuilder.Extension.Repositories;
using FormBuilder.Extension.Services;
using FormBuilder.Extension.Validators;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace FormBuilder.Extension.Composers
{
    public class FormBuilderComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
#if !NET10_0_OR_GREATER
            builder.Sections().Append<FormBuilderSection>();
#endif

            builder.Services.AddScoped<IFormSubmissionValidator, FormSubmissionValidator>();
            builder.Services.AddScoped<IFormRepository, FormRepository>();
            builder.Services.Configure<EmailServiceOptions>(
                builder.Config.GetSection(EmailServiceOptions.Section));
            builder.Services.AddSingleton<IEmailService, EmailService>();
        }
    }
}
