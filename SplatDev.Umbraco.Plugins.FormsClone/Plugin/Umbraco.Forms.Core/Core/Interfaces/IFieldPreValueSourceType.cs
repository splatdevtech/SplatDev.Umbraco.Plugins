
// Type: Umbraco.Forms.Core.Interfaces.IFieldPreValueSourceType
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Models;


#nullable enable
namespace Umbraco.Forms.Core.Interfaces
{
  public interface IFieldPreValueSourceType
  {
    string Alias { get; set; }

    Dictionary<string, SettingAttribute> Settings();

    Task<List<PreValue>> GetPreValuesAsync(Field? field, Form? form);

    List<Exception> ValidateSettings();
  }
}
