using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SplatDev.Payments.Stripe.Data;
using SplatDev.Payments.Stripe.Interfaces;
using SplatDev.Payments.Stripe.Services;

namespace SplatDev.Payments.Stripe.Extensions;

public static class StripeServiceCollectionExtensions
{
    public static IServiceCollection AddSplatDevStripe(
        this IServiceCollection services,
        IConfiguration configuration,
        string configSectionPath = "SplatDev:Payments:Stripe")
    {
        services.Configure<StripeSettings>(configuration.GetSection(configSectionPath));

        var connectionString = configuration.GetConnectionString("umbracoDbDSN")
                            ?? configuration.GetConnectionString("DefaultConnection")
                            ?? throw new InvalidOperationException(
                                "Connection string 'umbracoDbDSN' or 'DefaultConnection' not found.");

        services.AddDbContext<StripePaymentDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
            options.AddInterceptors(new StripeAuditInterceptor(
                services.BuildServiceProvider().GetRequiredService<Microsoft.Extensions.Logging.ILogger<StripeAuditInterceptor>>()));
        });

        services.AddHttpClient<IStripeCheckoutService, StripeCheckoutService>("Stripe", client =>
        {
            client.BaseAddress = new Uri("https://api.stripe.com/");
            client.Timeout = TimeSpan.FromSeconds(30);
            client.DefaultRequestHeaders.Accept.Clear();
        });

        services.AddScoped<IPaymentIntentRepository, PaymentIntentRepository>();
        services.AddScoped<IStripeWebhookHandler, StripeWebhookHandler>();

        return services;
    }
}
