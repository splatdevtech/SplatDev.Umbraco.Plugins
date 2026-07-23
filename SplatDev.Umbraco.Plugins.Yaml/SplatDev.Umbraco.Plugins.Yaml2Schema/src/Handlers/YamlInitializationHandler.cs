using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Services;
using SplatDev.Umbraco.Plugins.Yaml2Schema.Services;

namespace SplatDev.Umbraco.Plugins.Yaml2Schema.Handlers
{
    public class YamlInitializationHandler : INotificationAsyncHandler<UmbracoApplicationStartedNotification>
    {
        private readonly YamlParser _yamlParser;
        private readonly DataTypeCreator _dataTypeCreator;
        private readonly DocumentTypeCreator _documentTypeCreator;
        private readonly MediaTypeCreator _mediaTypeCreator;
        private readonly TemplateCreator _templateCreator;
        private readonly ContentCreator _contentCreator;
        private readonly MediaCreator _mediaCreator;
        private readonly StaticAssetCreator _staticAssetCreator;
        private readonly LanguageCreator _languageCreator;
        private readonly DictionaryCreator _dictionaryCreator;
        private readonly MemberCreator _memberCreator;
        private readonly MemberTypeCreator _memberTypeCreator;
        private readonly MemberGroupCreator _memberGroupCreator;
        private readonly UserCreator _userCreator;
        private readonly PackageInstaller _packageInstaller;
        private readonly PackageValidator _packageValidator;
        private readonly PropertyEditorCreator _propertyEditorCreator;
        private readonly ModelsBuilderConfigurator _modelsBuilderConfigurator;
        private readonly PublishedModelsGenerator _publishedModelsGenerator;
        private readonly IRuntimeState _runtimeState;
        private readonly IHostApplicationLifetime _appLifetime;
        private readonly ILogger<YamlInitializationHandler> _logger;
        private readonly IConfiguration _configuration;
        private readonly IHostEnvironment _hostEnvironment;

        public YamlInitializationHandler(
            YamlParser yamlParser,
            DataTypeCreator dataTypeCreator,
            DocumentTypeCreator documentTypeCreator,
            MediaTypeCreator mediaTypeCreator,
            TemplateCreator templateCreator,
            ContentCreator contentCreator,
            MediaCreator mediaCreator,
            StaticAssetCreator staticAssetCreator,
            LanguageCreator languageCreator,
            DictionaryCreator dictionaryCreator,
            MemberCreator memberCreator,
            MemberTypeCreator memberTypeCreator,
            MemberGroupCreator memberGroupCreator,
            UserCreator userCreator,
            PackageInstaller packageInstaller,
            PackageValidator packageValidator,
            PropertyEditorCreator propertyEditorCreator,
            ModelsBuilderConfigurator modelsBuilderConfigurator,
            PublishedModelsGenerator publishedModelsGenerator,
            IRuntimeState runtimeState,
            IHostApplicationLifetime appLifetime,
            ILogger<YamlInitializationHandler> logger,
            IConfiguration configuration,
            IHostEnvironment hostEnvironment)
        {
            _yamlParser = yamlParser ?? throw new ArgumentNullException(nameof(yamlParser));
            _dataTypeCreator = dataTypeCreator ?? throw new ArgumentNullException(nameof(dataTypeCreator));
            _documentTypeCreator = documentTypeCreator ?? throw new ArgumentNullException(nameof(documentTypeCreator));
            _mediaTypeCreator = mediaTypeCreator ?? throw new ArgumentNullException(nameof(mediaTypeCreator));
            _templateCreator = templateCreator ?? throw new ArgumentNullException(nameof(templateCreator));
            _contentCreator = contentCreator ?? throw new ArgumentNullException(nameof(contentCreator));
            _mediaCreator = mediaCreator ?? throw new ArgumentNullException(nameof(mediaCreator));
            _staticAssetCreator = staticAssetCreator ?? throw new ArgumentNullException(nameof(staticAssetCreator));
            _languageCreator = languageCreator ?? throw new ArgumentNullException(nameof(languageCreator));
            _dictionaryCreator = dictionaryCreator ?? throw new ArgumentNullException(nameof(dictionaryCreator));
            _memberCreator = memberCreator ?? throw new ArgumentNullException(nameof(memberCreator));
            _memberTypeCreator = memberTypeCreator ?? throw new ArgumentNullException(nameof(memberTypeCreator));
            _memberGroupCreator = memberGroupCreator ?? throw new ArgumentNullException(nameof(memberGroupCreator));
            _userCreator = userCreator ?? throw new ArgumentNullException(nameof(userCreator));
            _packageInstaller = packageInstaller ?? throw new ArgumentNullException(nameof(packageInstaller));
            _packageValidator = packageValidator ?? throw new ArgumentNullException(nameof(packageValidator));
            _propertyEditorCreator = propertyEditorCreator ?? throw new ArgumentNullException(nameof(propertyEditorCreator));
            _modelsBuilderConfigurator = modelsBuilderConfigurator ?? throw new ArgumentNullException(nameof(modelsBuilderConfigurator));
            _publishedModelsGenerator = publishedModelsGenerator ?? throw new ArgumentNullException(nameof(publishedModelsGenerator));
            _runtimeState = runtimeState ?? throw new ArgumentNullException(nameof(runtimeState));
            _appLifetime = appLifetime ?? throw new ArgumentNullException(nameof(appLifetime));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _hostEnvironment = hostEnvironment ?? throw new ArgumentNullException(nameof(hostEnvironment));
        }

        public async Task HandleAsync(UmbracoApplicationStartedNotification notification, CancellationToken cancellationToken)
        {
            // Only run when Umbraco is fully installed and running — skip during installer/upgrade
            if (_runtimeState.Level != global::Umbraco.Cms.Core.RuntimeLevel.Run)
            {
                _logger.LogInformation(
                    "YamlInitializationHandler: Skipping YAML initialization — runtime level is {Level} (requires Run).",
                    _runtimeState.Level);
                return;
            }

            _logger.LogInformation("YamlInitializationHandler: Umbraco application started, initializing YAML configuration.");

            try
            {
                // Get config path from IConfiguration or use default; resolve relative paths against the content root
                var configPath = _configuration["UmbracoYaml:ConfigPath"] ?? "config/umbraco.yml";
                if (!Path.IsPathRooted(configPath))
                {
                    configPath = Path.Combine(_hostEnvironment.ContentRootPath, configPath);
                }

                _logger.LogInformation("YamlInitializationHandler: Attempting to parse YAML configuration from '{ConfigPath}'.", configPath);

                // Parse YAML
                var yamlRoot = _yamlParser.ParseYaml(configPath);

                if (yamlRoot?.Umbraco == null)
                {
                    _logger.LogWarning("YamlInitializationHandler: YAML configuration is empty or invalid. No items to create.");
                    return;
                }

                // Apply Models Builder settings to appsettings.json (before schema creation)
                if (yamlRoot.Umbraco.ModelsBuilder != null)
                {
                    _logger.LogInformation("YamlInitializationHandler: Configuring Models Builder settings.");
                    _modelsBuilderConfigurator.Configure(yamlRoot.Umbraco.ModelsBuilder);
                }

                // Install packages marked with install: true (download + load into AppDomain)
                if (yamlRoot.Umbraco.Packages?.Count > 0)
                {
                    var installable = yamlRoot.Umbraco.Packages.Count(p => p.Install);
                    if (installable > 0)
                    {
                        _logger.LogInformation("YamlInitializationHandler: Installing {Count} package(s).", installable);
                        var newlyInstalled = _packageInstaller.InstallPackages(yamlRoot.Umbraco.Packages);
                        if (newlyInstalled > 0)
                        {
                            _logger.LogInformation(
                                "YamlInitializationHandler: {Count} package(s) newly installed. " +
                                "Restarting application so DI/IComposer registrations take effect.",
                                newlyInstalled);
                            _appLifetime.StopApplication();
                            return;
                        }
                    }
                }

                // Validate declared NuGet packages (informational — checks assembly presence)
                if (yamlRoot.Umbraco.Packages?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Validating {Count} package(s).", yamlRoot.Umbraco.Packages.Count);
                    _packageValidator.ValidatePackages(yamlRoot.Umbraco.Packages);
                }

                // Create custom property editor manifests (before DataTypes so DataTypes can reference them)
                if (yamlRoot.Umbraco.PropertyEditors?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} PropertyEditor(s).", yamlRoot.Umbraco.PropertyEditors.Count);
                    _propertyEditorCreator.CreatePropertyEditors(yamlRoot.Umbraco.PropertyEditors);
                }

                // Create Languages first (other creators may reference culture codes)
                if (yamlRoot.Umbraco.Languages?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} Languages.", yamlRoot.Umbraco.Languages.Count);
                    _languageCreator.CreateLanguages(yamlRoot.Umbraco.Languages);
                }

                // Create DataTypes
                if (yamlRoot.Umbraco.DataTypes?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} DataTypes.", yamlRoot.Umbraco.DataTypes.Count);
                    _dataTypeCreator.CreateDataTypes(yamlRoot.Umbraco.DataTypes);
                    _logger.LogInformation("YamlInitializationHandler: Successfully created {Count} DataTypes.", yamlRoot.Umbraco.DataTypes.Count);
                }
                else
                {
                    _logger.LogInformation("YamlInitializationHandler: No DataTypes to create.");
                }

                // Create DocumentTypes
                if (yamlRoot.Umbraco.DocumentTypes?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} DocumentTypes.", yamlRoot.Umbraco.DocumentTypes.Count);
                    _documentTypeCreator.CreateDocumentTypes(yamlRoot.Umbraco.DocumentTypes, yamlRoot.Umbraco.DataTypes);
                    _logger.LogInformation("YamlInitializationHandler: Successfully created {Count} DocumentTypes.", yamlRoot.Umbraco.DocumentTypes.Count);
                }
                else
                {
                    _logger.LogInformation("YamlInitializationHandler: No DocumentTypes to create.");
                }

                // Generate published model .cs files (after DocumentTypes exist)
                if (yamlRoot.Umbraco.DocumentTypes?.Count > 0 && yamlRoot.Umbraco.ModelsBuilder != null)
                {
                    _logger.LogInformation("YamlInitializationHandler: Generating published models.");
                    _publishedModelsGenerator.GenerateModels(
                        yamlRoot.Umbraco.DocumentTypes,
                        yamlRoot.Umbraco.DataTypes,
                        yamlRoot.Umbraco.ModelsBuilder.OutputPath);
                }

                // Create MediaTypes
                if (yamlRoot.Umbraco.MediaTypes?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} MediaTypes.", yamlRoot.Umbraco.MediaTypes.Count);
                    _mediaTypeCreator.CreateMediaTypes(yamlRoot.Umbraco.MediaTypes);
                }

                // Create static JavaScript files
                if (yamlRoot.Umbraco.Scripts?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} Scripts.", yamlRoot.Umbraco.Scripts.Count);
                    _staticAssetCreator.CreateScripts(yamlRoot.Umbraco.Scripts);
                    _logger.LogInformation("YamlInitializationHandler: Successfully created {Count} Scripts.", yamlRoot.Umbraco.Scripts.Count);
                }
                else
                {
                    _logger.LogInformation("YamlInitializationHandler: No Scripts to create.");
                }

                // Create static CSS stylesheets
                if (yamlRoot.Umbraco.Stylesheets?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} Stylesheets.", yamlRoot.Umbraco.Stylesheets.Count);
                    _staticAssetCreator.CreateStylesheets(yamlRoot.Umbraco.Stylesheets);
                    _logger.LogInformation("YamlInitializationHandler: Successfully created {Count} Stylesheets.", yamlRoot.Umbraco.Stylesheets.Count);
                }
                else
                {
                    _logger.LogInformation("YamlInitializationHandler: No Stylesheets to create.");
                }

                // Create Templates
                if (yamlRoot.Umbraco.Templates?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} Templates.", yamlRoot.Umbraco.Templates.Count);
                    _templateCreator.CreateTemplates(yamlRoot.Umbraco.Templates);
                    _logger.LogInformation("YamlInitializationHandler: Successfully created {Count} Templates.", yamlRoot.Umbraco.Templates.Count);
                }
                else
                {
                    _logger.LogInformation("YamlInitializationHandler: No Templates to create.");
                }

                // Link templates to document types (runs after both are created)
                if (yamlRoot.Umbraco.DocumentTypes?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Linking templates to DocumentTypes.");
                    _documentTypeCreator.LinkTemplatesToDocumentTypes(yamlRoot.Umbraco.DocumentTypes);
                }

                // Resolve Block List contentElementTypeAlias → contentElementTypeKey
                // (must run after DocumentTypes so element types exist)
                if (yamlRoot.Umbraco.DataTypes?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Linking Block List element types.");
                    _dataTypeCreator.LinkBlockListElementTypes(yamlRoot.Umbraco.DataTypes);
                }

                // Create Media (must run before Content so MediaPicker3 seeds can resolve media by name)
                if (yamlRoot.Umbraco.Media?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} Media items.", yamlRoot.Umbraco.Media.Count);
                    _mediaCreator.CreateMedia(yamlRoot.Umbraco.Media, defaultFolder: yamlRoot.Umbraco.MediaDefaultFolder);
                    _logger.LogInformation("YamlInitializationHandler: Successfully created {Count} Media items.", yamlRoot.Umbraco.Media.Count);
                }
                else
                {
                    _logger.LogInformation("YamlInitializationHandler: No Media items to create.");
                }

                // Create Content
                if (yamlRoot.Umbraco.Content?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} Content items.", yamlRoot.Umbraco.Content.Count);
                    _contentCreator.CreateContent(yamlRoot.Umbraco.Content);
                    _logger.LogInformation("YamlInitializationHandler: Successfully created {Count} Content items.", yamlRoot.Umbraco.Content.Count);
                }
                else
                {
                    _logger.LogInformation("YamlInitializationHandler: No Content items to create.");
                }

                // Create Dictionary Items
                if (yamlRoot.Umbraco.DictionaryItems?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} Dictionary items.", yamlRoot.Umbraco.DictionaryItems.Count);
                    _dictionaryCreator.CreateDictionaryItems(yamlRoot.Umbraco.DictionaryItems);
                }

                // Create Member Groups (before MemberTypes and Members so groups exist for access rules)
                if (yamlRoot.Umbraco.MemberGroups?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} MemberGroups.", yamlRoot.Umbraco.MemberGroups.Count);
                    _memberGroupCreator.CreateMemberGroups(yamlRoot.Umbraco.MemberGroups);
                }

                // Create Member Types (before Members so custom types exist when members are created)
                if (yamlRoot.Umbraco.MemberTypes?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} MemberTypes.", yamlRoot.Umbraco.MemberTypes.Count);
                    _memberTypeCreator.CreateMemberTypes(yamlRoot.Umbraco.MemberTypes, yamlRoot.Umbraco.DataTypes);
                }

                // Create Members
                if (yamlRoot.Umbraco.Members?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} Members.", yamlRoot.Umbraco.Members.Count);
                    _memberCreator.CreateMembers(yamlRoot.Umbraco.Members);
                }

                // Create Users
                if (yamlRoot.Umbraco.Users?.Count > 0)
                {
                    _logger.LogInformation("YamlInitializationHandler: Creating {Count} Users.", yamlRoot.Umbraco.Users.Count);
                    _userCreator.CreateUsers(yamlRoot.Umbraco.Users);
                }

                _logger.LogInformation("YamlInitializationHandler: YAML initialization completed successfully.");

                // Rename the processed YAML file to *.done so it is not re-imported on the next startup.
                var doneFilePath = Path.ChangeExtension(configPath, ".done");
                File.Move(configPath, doneFilePath, overwrite: true);
                _logger.LogInformation(
                    "YamlInitializationHandler: Renamed '{ConfigPath}' to '{DonePath}'.",
                    configPath, doneFilePath);
            }
            catch (FileNotFoundException ex)
            {
                _logger.LogWarning(ex, "YamlInitializationHandler: YAML configuration file not found. Skipping initialization.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "YamlInitializationHandler: An error occurred during YAML initialization.");
                throw;
            }

            await Task.CompletedTask;
        }
    }
}
