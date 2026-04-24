
// Type: Umbraco.Forms.Core.Models.Mapping.FolderMapDefinition
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Diagnostics.CodeAnalysis;
using Umbraco.Cms.Core.Mapping;


#nullable enable
namespace Umbraco.Forms.Core.Models.Mapping
{
  internal sealed class FolderMapDefinition : IMapDefinition
  {
    public void DefineMaps(IUmbracoMapper mapper)
    {
      mapper.Define<Folder, FolderEntity>((Func<Folder, MapperContext, FolderEntity>) ((source, context) => new FolderEntity()), new Action<Folder, FolderEntity, MapperContext>(this.Map));
      mapper.Define<FolderEntity, Folder>((Func<FolderEntity, MapperContext, Folder>) ((source, context) => new Folder()), new Action<FolderEntity, Folder, MapperContext>(this.Map));
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
