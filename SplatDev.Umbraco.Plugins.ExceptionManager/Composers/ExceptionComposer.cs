using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Web.Common.ApplicationBuilder;

namespace SplatDev.Umbraco.Plugins.ExceptionManager.Composers
{
    public class ExceptionComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            var enableExceptionManager = builder.Config.GetValue<bool>("EnableExceptionManager");

            builder.Services.Configure<UmbracoPipelineOptions>(options =>
            {
                var isProduction = string.IsNullOrEmpty(builder.Config.GetValue<string>("Umbraco:CMS:WebRouting:UmbracoApplicationUrl"));
                options.AddFilter(new UmbracoPipelineFilter("EnableExceptionManager")
                {
                    PrePipeline = app =>
                    {
                        if (isProduction && enableExceptionManager)
                        {
                            app.UseExceptionHandler("/Error");

                        }
                        else
                        {
                            app.UseDeveloperExceptionPage();
                        }

                    }
                });
            });
        }
    }
}
