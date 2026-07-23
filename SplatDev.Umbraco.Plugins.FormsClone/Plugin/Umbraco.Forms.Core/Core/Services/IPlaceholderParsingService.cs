
// Type: Umbraco.Forms.Core.Services.IPlaceholderParsingService
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections;
using System.Collections.Generic;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;


#nullable enable
namespace Umbraco.Forms.Core.Services
{
  public interface IPlaceholderParsingService
  {
    string ParsePlaceHolders(
      string value,
      bool htmlEncodeValues,
      Record? record = null,
      Form? form = null,
      Hashtable? pageElements = null,
      IDictionary<string, string?>? additionalData = null);
  }
}
