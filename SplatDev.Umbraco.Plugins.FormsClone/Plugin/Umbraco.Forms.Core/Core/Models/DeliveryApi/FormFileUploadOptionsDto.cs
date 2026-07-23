
// Type: Umbraco.Forms.Core.Models.DeliveryApi.FormFileUploadOptionsDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using System.Linq;
using Umbraco.Forms.Core.Interfaces;


#nullable enable
namespace Umbraco.Forms.Core.Models.DeliveryApi
{
  public class FormFileUploadOptionsDto : ISupportFileUploads
  {
    public bool AllowAllUploadExtensions { get; set; }

    public IEnumerable<string> AllowedUploadExtensions { get; set; } = Enumerable.Empty<string>();

    public bool AllowMultipleFileUploads { get; set; }
  }
}
