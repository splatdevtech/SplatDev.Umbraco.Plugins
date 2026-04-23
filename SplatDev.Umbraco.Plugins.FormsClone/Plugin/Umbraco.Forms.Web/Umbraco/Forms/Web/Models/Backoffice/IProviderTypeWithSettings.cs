
// Type: Umbraco.Forms.Web.Models.Backoffice.IProviderTypeWithSettings
// Assembly: Umbraco.Forms.Web, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 2D22F92E-1771-4BC0-AF55-EE124F158F73

using System;
using System.Collections.Generic;


#nullable enable
namespace Umbraco.Forms.Web.Models.Backoffice
{
  public interface IProviderTypeWithSettings
  {
    Guid Id { get; }

    IEnumerable<Setting> Settings { get; set; }
  }
}
