
// Type: Umbraco.Forms.Core.Persistence.Factories.FolderFactory
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Factories
{
  internal sealed class FolderFactory : IFolderFactory
  {
    public IEnumerable<FolderEntity> BuildEntities(
      IEnumerable<FolderDto> dtos)
    {
      return dtos.Select<FolderDto, FolderEntity>(new Func<FolderDto, FolderEntity>(this.BuildEntity));
    }

    public FolderEntity BuildEntity(FolderDto dto)
    {
      FolderEntity folderEntity = new FolderEntity();
      folderEntity.Name = dto.Name;
      folderEntity.Id = dto.Id;
      folderEntity.Key = dto.Key;
      folderEntity.CreateDate = dto.CreateDate;
      folderEntity.UpdateDate = dto.UpdateDate;
      folderEntity.ParentKey = dto.ParentKey;
      return folderEntity;
    }

    public FolderDto BuildDto(FolderEntity entity)
    {
      FolderDto folderDto = new FolderDto();
      folderDto.Name = entity.Name;
      folderDto.Id = entity.Id;
      folderDto.Key = entity.Key;
      folderDto.CreateDate = entity.CreateDate;
      folderDto.UpdateDate = entity.UpdateDate;
      folderDto.ParentKey = entity.ParentKey;
      folderDto.Definition = string.Empty;
      return folderDto;
    }
  }
}
