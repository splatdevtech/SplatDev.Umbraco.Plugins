using FormBuilder.Core.Models;

using System.Diagnostics.CodeAnalysis;

using Umbraco.Cms.Core.Mapping;

namespace FormBuilder.Core.Mapping
{
    internal sealed class FolderMapDefinition : IMapDefinition
    {
        public void DefineMaps(IUmbracoMapper mapper)
        {
            mapper.Define((source, context) => new FolderEntity(), new Action<Folder, FolderEntity, MapperContext>(Map));
            mapper.Define((source, context) => new Folder(), new Action<FolderEntity, Folder, MapperContext>(Map));
        }

        internal void Map(Folder source, FolderEntity target, MapperContext context)
        {
            target.ParentKey = source.ParentId;
            target.Name = source.Name;
            target.CreateDate = source.Created;
            target.Key = source.Id;
        }

        [ExcludeFromCodeCoverage]
        internal void Map(FolderEntity source, Folder target, MapperContext context)
        {
            target.ParentId = source.ParentKey;
            target.Name = source.Name;
            target.Created = source.CreateDate;
            target.Id = source.Key;
        }
    }
}