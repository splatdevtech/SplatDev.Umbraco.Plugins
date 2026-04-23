
// Type: Umbraco.Forms.Core.FieldPreValueSourceType
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Core
{
  public abstract class FieldPreValueSourceType : ProviderBase, IFieldPreValueSourceType
  {
    public FieldPreValueSource? FieldPreValueSource { get; private set; }

    public abstract Task<List<PreValue>> GetPreValuesAsync(Field? field, Form? form);

    public abstract List<Exception> ValidateSettings();

    public void LoadSettings(FieldPreValueSource fieldPreValueSource)
    {
      this.FieldPreValueSource = fieldPreValueSource;
      foreach (string key in fieldPreValueSource.Settings.Keys)
      {
        try
        {
          this.GetType().InvokeMember(key, BindingFlags.SetProperty, (Binder) null, (object) this, new object[1]
          {
            (object) fieldPreValueSource.Settings[key]
          });
        }
        catch (MissingMethodException ex)
        {
        }
        catch
        {
          throw;
        }
      }
    }

    public virtual Dictionary<string, SettingAttribute> Settings()
    {
      Dictionary<string, SettingAttribute> dictionary = new Dictionary<string, SettingAttribute>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (PropertyInfo property in this.GetType().GetProperties())
      {
        object[] customAttributes = property.GetCustomAttributes(typeof (SettingAttribute), true);
        if (customAttributes.Length != 0)
          dictionary.Add(property.Name, (SettingAttribute) customAttributes[0]);
      }
      return dictionary;
    }
  }
}
