
// Type: Umbraco.Forms.Core.Persistence.Dtos.FolderDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;
using System;
using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Forms.Core.Persistence.Dtos
{
  [TableName("UFFolders")]
  public class FolderDto : BaseDto
  {
    [Column(Name = "ParentKey")]
    [NullSetting]
    [ForeignKey(typeof (FolderDto), Column = "Key")]
    public Guid? ParentKey { get; set; }
  }
}
