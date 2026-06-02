# =============================================================================
# SplatDev Umbraco 13 Test Baseline — Multi-stage Docker Build
# Builds all 80+ SplatDev plugins and installs them into an Umbraco 13 site
# using local project references (NuGet packages not yet published).
# =============================================================================

# ---------------------------------------------------------------------------
# Stage 1: Build Vite/Lit frontend clients for plugins that need them
# ---------------------------------------------------------------------------
FROM node:22-bookworm-slim AS frontend-build

WORKDIR /src

COPY . .

RUN set -e; \
    for client_dir in $(find . -maxdepth 3 -path '*/client/vite.config.*' -exec dirname {} \;); do \
      echo "=== Building frontend: $client_dir ==="; \
      cd /src/$client_dir && \
      npm ci --ignore-scripts 2>/dev/null || npm install --ignore-scripts 2>/dev/null && \
      npx vite build; \
    done

# ---------------------------------------------------------------------------
# Stage 2: Build .NET solution + scaffold Umbraco 13 baseline
# ---------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS dotnet-build

ENV DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1

WORKDIR /src/plugins

COPY . .

COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.2fa/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.2fa/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.Blog/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.Blog/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.CopyValue/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.CopyValue/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.CustomLogin/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.CustomLogin/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.DefaultValue/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.DefaultValue/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.Faqs/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.Faqs/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.Forums/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.Forums/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.HiddenContent/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.HiddenContent/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.MemberGroups/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.MemberGroups/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.MemberLogin/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.MemberLogin/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.MemberRegistration/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.MemberRegistration/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.MemberTypes/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.MemberTypes/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.MostViewed/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.MostViewed/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.NewsTicker/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.NewsTicker/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.Newsletters/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.Newsletters/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.OnOff/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.OnOff/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.PasswordSettings/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.PasswordSettings/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.QuickPoll/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.QuickPoll/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.Restricted/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.Restricted/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.Rsvp/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.Rsvp/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.StarRatings/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.StarRatings/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.Surveys/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.Surveys/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.Tweets/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.Tweets/App_Plugins/
COPY --from=frontend-build /src/SplatDev.Umbraco.Plugins.VisitorCounter/App_Plugins/ \
     ./SplatDev.Umbraco.Plugins.VisitorCounter/App_Plugins/

# Install Umbraco 13 template and scaffold baseline site
RUN dotnet new install Umbraco.Templates::13.12.0

WORKDIR /src/baseline
RUN dotnet new umbraco -n UmbracoBaseline --no-restore \
    --connection-string "Data Source=|DataDirectory|/umbraco_baseline.db;Cache=Shared;Foreign Keys=True;Pooling=True" \
    --development-database-type SQLite

# Generate the baseline .csproj with all plugin project references
RUN cat > /src/baseline/UmbracoBaseline/UmbracoBaseline.csproj <<'CSPROJ'
<Project Sdk="Microsoft.NET.Sdk.Web">

  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

  <ItemGroup>
    <PackageReference Include="Umbraco.Cms" Version="13.12.0" />
  </ItemGroup>

  <!-- ====================================================================
       SplatDev Plugin Project References (local, NuGet not yet published)
       ==================================================================== -->

  <!-- Core / shared libraries -->
  <ItemGroup>
    <ProjectReference Include="../../plugins/SplatDev.Reflection.Helpers/SplatDev.Reflection.Helpers.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Content/SplatDev.Content.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Database/SplatDev.Database.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Database.MSSQLAdmin/SplatDev.Database.MSSQLAdmin.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.GeoLocation/SplatDev.GeoLocation.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Logger/SplatDev.Logger.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Routing/SplatDev.Routing.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.ScheduledTasks/SplatDev.ScheduledTasks.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Security/SplatDev.Security.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Mobile.Detection/SplatDev.Mobile.Detection.csproj" />
  </ItemGroup>

  <!-- Messaging -->
  <ItemGroup>
    <ProjectReference Include="../../plugins/SplatDev.Messaging/SplatDev.Messaging.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Messaging.Mailgun/SplatDev.Messaging.Mailgun.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Messaging.Newsletter/SplatDev.Messaging.Newsletter.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Messaging.SendGrid/SplatDev.Messaging.SendGrid.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Messaging.Smtp/SplatDev.Messaging.Smtp.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Messaging.SocketLabs/SplatDev.Messaging.SocketLabs.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Messaging.Twilio/SplatDev.Messaging.Twilio.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Messaging.SMSTools/SplatDev.Messaging.SMSTools.csproj" />
  </ItemGroup>

  <!-- Payments -->
  <ItemGroup>
    <ProjectReference Include="../../plugins/SplatDev.Payments/SplatDev.Payments.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Payments.MercadoPago/SplatDev.Payments.MercadoPago.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Payments.PagSeguro/SplatDev.Payments.PagSeguro.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Payments.Stripe/SplatDev.Payments.Stripe.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Payments.BancoInter/SplatDev.Payments.BancoInter.csproj" />
  </ItemGroup>

  <!-- Umbraco shared / infrastructure -->
  <ItemGroup>
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.2FA/SplatDev.Umbraco.2FA.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Common/SplatDev.Umbraco.Common.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.EntityFramework/SplatDev.Umbraco.EntityFramework.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Pagination/SplatDev.Umbraco.Pagination.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Examine/SplatDev.Umbraco.Examine.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.DataTypes.USStates/SplatDev.Umbraco.DataTypes.USStates.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Markup/SplatDev.Umbraco.Markup.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Membership/SplatDev.Umbraco.Membership.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.NPoco/SplatDev.Umbraco.NPoco.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.QueryStringFilters/SplatDev.Umbraco.QueryStringFilters.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Authorization.Ldap/SplatDev.Umbraco.Authorization.Ldap.csproj" />
  </ItemGroup>

  <!-- Umbraco plugins -->
  <ItemGroup>
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Core/SplatDev.Umbraco.Plugins.Core.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.2fa/SplatDev.Umbraco.Plugins.2fa.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.AdminBar/SplatDev.Umbraco.Plugins.AdminBar.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Analytics/SplatDev.Umbraco.Plugins.Analytics.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Backups/SplatDev.Umbraco.Plugins.Backups.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Blog/SplatDev.Umbraco.Plugins.Blog.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.CacheManager/SplatDev.Umbraco.Plugins.CacheManager.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.CharLimit/SplatDev.Umbraco.Plugins.CharLimit.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.CodeFirst/SplatDev.Umbraco.Plugins.CodeFirst.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.CopyValue/SplatDev.Umbraco.Plugins.CopyValue.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Countries/SplatDev.Umbraco.Plugins.Countries.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.CustomLogin/SplatDev.Umbraco.Plugins.CustomLogin.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.D4Sign/SplatDev.Umbraco.Plugins.D4Sign.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.DefaultValue/SplatDev.Umbraco.Plugins.DefaultValue.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.DictionaryManager/SplatDev.Umbraco.Plugins.DictionaryManager.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Dropzone/SplatDev.Umbraco.Plugins.Dropzone.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.ENotAssina/SplatDev.Umbraco.Plugins.ENotAssina.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.EmailNotifications/SplatDev.Umbraco.Plugins.EmailNotifications.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.EmailTemplates/SplatDev.Umbraco.Plugins.EmailTemplates.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.ExamineExtensions/SplatDev.Umbraco.Plugins.ExamineExtensions.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.ExceptionManager/SplatDev.Umbraco.Plugins.ExceptionManager.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Exif/SplatDev.Umbraco.Plugins.Exif.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Faqs/SplatDev.Umbraco.Plugins.Faqs.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Forums/SplatDev.Umbraco.Plugins.Forums.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Gdrp/SplatDev.Umbraco.Plugins.Gdrp.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.HiddenContent/SplatDev.Umbraco.Plugins.HiddenContent.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.IspServices/SplatDev.Umbraco.Plugins.IspServices.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.JsonRpc/SplatDev.Umbraco.Plugins.JsonRpc.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.LazyLoad/SplatDev.Umbraco.Plugins.LazyLoad.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.LiveVideo/SplatDev.Umbraco.Plugins.LiveVideo.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Mailer/SplatDev.Umbraco.Plugins.Mailer.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.MemberGroups/SplatDev.Umbraco.Plugins.MemberGroups.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.MemberLogin/SplatDev.Umbraco.Plugins.MemberLogin.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.MemberNotifications/SplatDev.Umbraco.Plugins.MemberNotifications.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.MemberRegistration/SplatDev.Umbraco.Plugins.MemberRegistration.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.MemberTypes/SplatDev.Umbraco.Plugins.MemberTypes.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.MostViewed/SplatDev.Umbraco.Plugins.MostViewed.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.NewsTicker/SplatDev.Umbraco.Plugins.NewsTicker.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Newsletter/SplatDev.Umbraco.Plugins.Newsletter.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Newsletters/SplatDev.Umbraco.Plugins.Newsletters.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.OAuth/SplatDev.Umbraco.Plugins.OAuth.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.OnOff/SplatDev.Umbraco.Plugins.OnOff.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.PasswordSettings/SplatDev.Umbraco.Plugins.PasswordSettings.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Payments.BancoInter/SplatDev.Umbraco.Plugins.Payments.BancoInter.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Payments.MercadoPago/SplatDev.Umbraco.Plugins.Payments.MercadoPago.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Payments.PagSeguro/SplatDev.Umbraco.Plugins.Payments.PagSeguro.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.PdfCurator/SplatDev.Umbraco.Plugins.PdfCurator.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.PhotoGallery/SplatDev.Umbraco.Plugins.PhotoGallery.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.PropertiesReport/SplatDev.Umbraco.Plugins.PropertiesReport.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.QuickPoll/SplatDev.Umbraco.Plugins.QuickPoll.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.RdpManager/SplatDev.Umbraco.Plugins.RdpManager.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.RedirectManager/SplatDev.Umbraco.Plugins.RedirectManager.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Restricted/SplatDev.Umbraco.Plugins.Restricted.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Rsvp/SplatDev.Umbraco.Plugins.Rsvp.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.SEO/SplatDev.Umbraco.Plugins.SEO.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Security/SplatDev.Umbraco.Plugins.Security.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Settings/SplatDev.Umbraco.Plugins.Settings.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.ShopCart/SplatDev.Umbraco.Plugins.ShopCart.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.ShortUrls/SplatDev.Umbraco.Plugins.ShortUrls.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Slider/SplatDev.Umbraco.Plugins.Slider.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Smtp/SplatDev.Umbraco.Plugins.Smtp.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.SocialMedia.Channels/SplatDev.Umbraco.Plugins.SocialMedia.Channels.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.SocialMedia.Login/SplatDev.Umbraco.Plugins.SocialMedia.Login.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.SocialMedia.Share/SplatDev.Umbraco.Plugins.SocialMedia.Share.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.StarRatings/SplatDev.Umbraco.Plugins.StarRatings.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Surveys/SplatDev.Umbraco.Plugins.Surveys.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.SvgViewer/SplatDev.Umbraco.Plugins.SvgViewer.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.ToastNotifications/SplatDev.Umbraco.Plugins.ToastNotifications.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Tweets/SplatDev.Umbraco.Plugins.Tweets.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.VideoPreview/SplatDev.Umbraco.Plugins.VideoPreview.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.VisitorCounter/SplatDev.Umbraco.Plugins.VisitorCounter.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.WhatsApp/SplatDev.Umbraco.Plugins.WhatsApp.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.WordsApi/SplatDev.Umbraco.Plugins.WordsApi.csproj" />
  </ItemGroup>

  <!-- Nested plugin projects -->
  <ItemGroup>
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.FormBuilder/FormBuilder.Extension/FormBuilder.Extension.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Yaml/SplatDev.Umbraco.Plugins.Schema2Yaml/SplatDev.Umbraco.Plugins.Schema2Yaml.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Plugins.Yaml/SplatDev.Umbraco.Plugins.Yaml2Schema/SplatDev.Umbraco.Plugins.Yaml2Schema.csproj" />
  </ItemGroup>

  <!-- Umbraco Tools -->
  <ItemGroup>
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Tools.PackageActions/SplatDev.Umbraco.Tools.PackageActions.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Tools.Packager/SplatDev.Umbraco.Tools.Packager.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Tools.T4.Plugins/SplatDev.Umbraco.Tools.T4.Plugins.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Tools.T4.Themes/SplatDev.Umbraco.Tools.T4.Themes.csproj" />
  </ItemGroup>

  <!-- Themes -->
  <ItemGroup>
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Themes.Base/SplatDev.Umbraco.Themes.Base.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Themes.Blog/SplatDev.Umbraco.Themes.Blog.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Themes.Commerce/SplatDev.Umbraco.Themes.Commerce.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Themes.Conference/SplatDev.Umbraco.Themes.Conference.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Themes.Corporate/SplatDev.Umbraco.Themes.Corporate.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Themes.Forum/SplatDev.Umbraco.Themes.Forum.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Themes.Hotel/SplatDev.Umbraco.Themes.Hotel.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Themes.Landing/SplatDev.Umbraco.Themes.Landing.csproj" />
    <ProjectReference Include="../../plugins/SplatDev.Umbraco.Themes.Portfolio/SplatDev.Umbraco.Themes.Portfolio.csproj" />
  </ItemGroup>

</Project>
CSPROJ

# Configure appsettings for unattended install with SQLite
RUN cat > /src/baseline/UmbracoBaseline/appsettings.json <<'APPSETTINGS'
{
  "ConnectionStrings": {
    "umbracoDbDSN": "Data Source=|DataDirectory|/umbraco_baseline.db;Cache=Shared;Foreign Keys=True;Pooling=True",
    "umbracoDbDSN_ProviderName": "Microsoft.Data.Sqlite"
  },
  "Umbraco": {
    "CMS": {
      "Unattended": {
        "InstallUnattended": true,
        "UnattendedUserName": "Admin",
        "UnattendedUserEmail": "admin@splatdev.local",
        "UnattendedUserPassword": "SplatDev2024!"
      },
      "Global": {
        "MainDomScheme": "http",
        "MainDomPort": 5001,
        "UseHttps": false
      },
      "Hosting": {
        "Debug": true
      }
    }
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "Kestrel": {
    "Endpoints": {
      "Http": {
        "Url": "http://0.0.0.0:5001"
      }
    }
  }
}
APPSETTINGS

# Build and publish the baseline with all plugins (net8.0 only)
RUN dotnet publish /src/baseline/UmbracoBaseline/UmbracoBaseline.csproj \
    -c Release \
    -f net8.0 \
    -o /app/publish \
    --no-self-contained

# ---------------------------------------------------------------------------
# Stage 3: Runtime image with Playwright for E2E testing
# ---------------------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime

ENV ASPNETCORE_URLS=http://0.0.0.0:5001 \
    ASPNETCORE_ENVIRONMENT=Development \
    DOTNET_CLI_TELEMETRY_OPTOUT=1 \
    DOTNET_NOLOGO=1

RUN apt-get update && apt-get install -y --no-install-recommends \
    curl \
    && rm -rf /var/lib/apt/lists/*

# Install Node.js 22 for Playwright
RUN curl -fsSL https://deb.nodesource.com/setup_22.x | bash - \
    && apt-get install -y --no-install-recommends nodejs \
    && rm -rf /var/lib/apt/lists/*

# Install Playwright with Chromium
RUN npx -y playwright@latest install --with-deps chromium

WORKDIR /app

COPY --from=dotnet-build /app/publish .

# Copy shared E2E test suite + Playwright config
COPY docker/test/e2e/ /app/e2e/
RUN cd /app/e2e && npm ci --ignore-scripts

RUN mkdir -p \
    /app/umbraco/Data \
    /output/screenshots \
    /output/videos \
    /output/test-results \
    /output/playwright-report

COPY docker/test/u13/scripts/test-runner.sh /app/test-runner.sh
RUN chmod +x /app/test-runner.sh

EXPOSE 5001

HEALTHCHECK --interval=15s --timeout=5s --start-period=60s --retries=5 \
    CMD curl -sf http://localhost:5001/umbraco || exit 1

ENTRYPOINT ["dotnet", "UmbracoBaseline.dll"]
