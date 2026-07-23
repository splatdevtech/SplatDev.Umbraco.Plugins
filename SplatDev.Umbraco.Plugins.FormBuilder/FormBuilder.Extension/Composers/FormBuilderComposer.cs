using FormBuilder.Extension.Interfaces;
using FormBuilder.Extension.Repositories;
using FormBuilder.Extension.Services;
using FormBuilder.Extension.Validators;

using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;

namespace FormBuilder.Extension.Composers
{
    public class FormBuilderComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddScoped<IFormSubmissionValidator, FormSubmissionValidator>();
            builder.Services.AddScoped<IFormRepository, FormRepository>();
            builder.Services.AddSingleton<IEmailService, EmailService>();
        }
    }
}
