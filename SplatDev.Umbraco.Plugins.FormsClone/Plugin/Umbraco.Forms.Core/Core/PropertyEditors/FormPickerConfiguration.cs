
// Type: Umbraco.Forms.Core.PropertyEditors.FormPickerConfiguration
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Collections.Generic;
using System.Linq;
using Umbraco.Cms.Core.PropertyEditors;


#nullable enable
namespace Umbraco.Forms.Core.PropertyEditors
{
  public class FormPickerConfiguration
  {
    [ConfigurationField("allowedFolders")]
    public IEnumerable<string> AllowedFolders { get; set; } = Enumerable.Empty<string>();

    [ConfigurationField("allowedForms")]
    public IEnumerable<string> AllowedForms { get; set; } = Enumerable.Empty<string>();
  }
}
