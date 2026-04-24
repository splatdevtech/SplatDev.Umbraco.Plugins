
// Type: Umbraco.Forms.Core.Services.DynamicRootContentLocator
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;

using Umbraco.Cms.Core.DynamicRoot;
using Umbraco.Cms.Core.DynamicRoot.QuerySteps;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
    internal class DynamicRootContentLocator : IDynamicRootContentLocator
    {
        private readonly IContentService _contentService;
        private readonly IIdKeyMap _idKeyMap;
        private readonly IEntityService _entityService;
        private readonly IDynamicRootService _dynamicRootService;

        public DynamicRootContentLocator(
          IContentService contentService,
          IIdKeyMap idKeyMap,
          IEntityService entityService,
          IDynamicRootService dynamicRootService)
        {
            this._contentService = contentService;
            this._idKeyMap = idKeyMap;
            this._entityService = entityService;
            this._dynamicRootService = dynamicRootService;
        }

        public async Task<IContent?> CreateContent(
          string rootNodeSettingValue,
          string nodeName,
          string contentTypeAlias,
          int currentPageId)
        {
            return await this.LocateDynamicRoot(rootNodeSettingValue, currentPageId, () => this._contentService.Create(nodeName, -1, contentTypeAlias), key => this._contentService.Create(nodeName, key, contentTypeAlias)).ConfigureAwait(false);
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
            Umbraco.Cms.Core.PropertyEditors.DynamicRoot dynamicRoot;
            if (!this.TryDeserializeSettings(rootNodeSettingValue, out dynamicRoot))
                return noRootNodeHandler();
            DynamicRootNodeQuery dynamicRootNodeQuery;
            return !this.TryGenerateDynamicRootNodeQuery(dynamicRoot, currentPageId, out dynamicRootNodeQuery) ? null : handler((await this._dynamicRootService.GetDynamicRootsAsync(dynamicRootNodeQuery).ConfigureAwait(false)).FirstOrDefault<Guid>());
        }

        private bool TryGenerateDynamicRootNodeQuery(
          Umbraco.Cms.Core.PropertyEditors.DynamicRoot dynamicRoot,
          int currentPageId,
          [NotNullWhen(true)] out DynamicRootNodeQuery? dynamicRootNodeQuery)
        {
            dynamicRootNodeQuery = null;
            Umbraco.Cms.Core.Attempt<Guid> keyForId = this._idKeyMap.GetKeyForId(currentPageId, UmbracoObjectTypes.Document);
            if (!keyForId.Success)
                return false;
            IEntitySlim parent = this._entityService.GetParent(currentPageId);
            DynamicRootContext dynamicRootContext = new DynamicRootContext()
            {
                CurrentKey = new Guid?(keyForId.Result),
                ParentKey = parent != null ? parent.Key : Guid.Empty
            };
            dynamicRootNodeQuery = new DynamicRootNodeQuery()
            {
                Context = dynamicRootContext,
                OriginAlias = dynamicRoot.OriginAlias,
                OriginKey = dynamicRoot.OriginKey,
                QuerySteps = dynamicRoot.QuerySteps.Select<QueryStep, DynamicRootQueryStep>(x => new DynamicRootQueryStep()
                {
                    Alias = x.Alias,
                    AnyOfDocTypeKeys = x.AnyOfDocTypeKeys
                })
            };
            return true;
        }

        private bool TryDeserializeSettings(string settingsValue, [NotNullWhen(true)] out Umbraco.Cms.Core.PropertyEditors.DynamicRoot? dynamicRoot)
        {
            dynamicRoot = JsonSerializer.Deserialize<MultiNodePickerConfigurationTreeSource>(settingsValue, FormsJsonSerializerOptions.Default)?.DynamicRoot;
            return dynamicRoot != null;
        }
    }
}
