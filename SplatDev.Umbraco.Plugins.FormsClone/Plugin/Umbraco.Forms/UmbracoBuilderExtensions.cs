
// Type: Umbraco.Forms.UmbracoBuilderExtensions
// Assembly: Umbraco.Forms, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 35E95609-BA13-4414-B9E2-6820FE57987F

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Forms.Core;
using Umbraco.Forms.Core.Extensions;
using Umbraco.Forms.Core.Providers.Extensions;
using Umbraco.Forms.Core.Searchers;
using Umbraco.Forms.Examine.Extensions;
using Umbraco.Forms.Web.Extensions;


#nullable enable
namespace Umbraco.Forms
{
    public static class UmbracoBuilderExtensions
    {
        public static IUmbracoBuilder AddUmbracoForms(this IUmbracoBuilder builder)
        {
            if (builder.Services.Any<ServiceDescriptor>(x => x.ServiceType == typeof(IFormRecordSearcher)))
                return builder;
            builder.AddUmbracoFormsCore();
            builder.AddUmbracoFormsCoreProviders();
            builder.AddUmbracoFormsWeb();
            if (UmbracoBuilderExtensions.IsRecordIndexingEnabled(builder))
                builder.AddUmbracoFormsExamine();
            return builder;
        }

        private static bool IsRecordIndexingEnabled(IUmbracoBuilder builder) => !builder.Config.GetSection(Constants.Configuration.SectionKeys.PackageOptions).GetValue<bool>("DisableRecordIndexing");
    }
}
