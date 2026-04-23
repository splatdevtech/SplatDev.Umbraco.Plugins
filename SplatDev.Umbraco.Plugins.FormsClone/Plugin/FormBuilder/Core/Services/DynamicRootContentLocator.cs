using FormBuilder.Core.Options;
using FormBuilder.Core.Services.Interfaces;

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using Umbraco.Cms.Core.DynamicRoot;
using Umbraco.Cms.Core.DynamicRoot.QuerySteps;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;

namespace FormBuilder.Core.Services
{
    internal class DynamicRootContentLocator(
        IContentService contentService,
        IIdKeyMap idKeyMap,
        IEntityService entityService,
        IDynamicRootService dynamicRootService) : IDynamicRootContentLocator
    {
        private readonly IContentService _contentService = contentService;
        private readonly IIdKeyMap _idKeyMap = idKeyMap;
        private readonly IEntityService _entityService = entityService;
        private readonly IDynamicRootService _dynamicRootService = dynamicRootService;

        public async Task<IContent?> CreateContent(
            string rootNodeSettingValue,
            string nodeName,
            string contentTypeAlias,
            int currentPageId)
        {
            IContent ContentFactory() => _contentService.Create(nodeName, -1, contentTypeAlias);
            IContent ContentFactoryWithKey(Guid key) => _contentService.Create(nodeName, key, contentTypeAlias);

            return await LocateDynamicRoot(
                rootNodeSettingValue,
                currentPageId,
                ContentFactory,
                ContentFactoryWithKey
            ).ConfigureAwait(false);
        }

        public async Task<IContent?> GetContent(string rootNodeSettingValue, int currentPageId)
        {
            IContent? DefaultContentFactory() => _contentService.GetRootContent().FirstOrDefault();
            IContent? ContentByIdFactory(Guid key) => _contentService.GetById(key);

            return await LocateDynamicRoot(
                rootNodeSettingValue,
                currentPageId,
                DefaultContentFactory,
                ContentByIdFactory
            ).ConfigureAwait(false);
        }

        private async Task<IContent?> LocateDynamicRoot(
            string rootNodeSettingValue,
            int currentPageId,
            Func<IContent?> noRootNodeHandler,
            Func<Guid, IContent?> handler)
        {
            if (!TryDeserializeSettings(rootNodeSettingValue, out var dynamicRoot))
            {
                return noRootNodeHandler();
            }

            if (!TryGenerateDynamicRootNodeQuery(dynamicRoot, currentPageId, out var query))
            {
                return null;
            }

            var roots = await _dynamicRootService.GetDynamicRootsAsync(query).ConfigureAwait(false);
            return handler(roots.FirstOrDefault());
        }

        private bool TryGenerateDynamicRootNodeQuery(
            DynamicRoot dynamicRoot,
            int currentPageId,
            [NotNullWhen(true)] out DynamicRootNodeQuery? query)
        {
            query = null;

            var keyAttempt = _idKeyMap.GetKeyForId(currentPageId, UmbracoObjectTypes.Document);
            if (!keyAttempt.Success)
            {
                return false;
            }

            var parent = _entityService.GetParent(currentPageId);
            var context = new DynamicRootContext
            {
                CurrentKey = keyAttempt.Result,
                ParentKey = parent?.Key ?? Guid.Empty
            };

            query = new DynamicRootNodeQuery
            {
                Context = context,
                OriginAlias = dynamicRoot.OriginAlias,
                OriginKey = dynamicRoot.OriginKey,
                QuerySteps = dynamicRoot.QuerySteps.Select(ConvertQueryStep)
            };

            return true;

            static DynamicRootQueryStep ConvertQueryStep(QueryStep step) => new()
            {
                Alias = step.Alias,
                AnyOfDocTypeKeys = step.AnyOfDocTypeKeys
            };
        }

        private static bool TryDeserializeSettings(
            string settingsValue,
            [NotNullWhen(true)] out DynamicRoot? dynamicRoot)
        {
            var config = JsonSerializer.Deserialize<MultiNodePickerConfigurationTreeSource>(
                settingsValue,
                FormsJsonSerializerOptions.Default
            );

            dynamicRoot = config?.DynamicRoot;
            return dynamicRoot != null;
        }
    }
}