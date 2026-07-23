using FormBuilder.Core.Attributes;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Providers;

using System.Reflection;

namespace FormBuilder.Core.Prevalues
{
    public abstract class FieldPrevalueSourceType : ProviderBase, IFieldPrevalueSourceType
    {
        public FieldPrevalueSource? FieldPrevalueSource { get; private set; }

        public abstract Task<List<Prevalue>> GetPreValuesAsync(Field? field, Form? form);

        public abstract List<Exception> ValidateSettings();

        public void LoadSettings(FieldPrevalueSource fieldPreValueSource)
        {
            FieldPrevalueSource = fieldPreValueSource;
            foreach (string key in fieldPreValueSource.Settings.Keys)
            {
                try
                {
                    GetType().InvokeMember(key, BindingFlags.SetProperty, null, this,
                    [
                        fieldPreValueSource.Settings[key]
                    ]);
                }
                catch (MissingMethodException)
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
            Dictionary<string, SettingAttribute> dictionary = new(StringComparer.OrdinalIgnoreCase);
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                object[] customAttributes = property.GetCustomAttributes(typeof(SettingAttribute), true);
                if (customAttributes.Length != 0)
                    dictionary.Add(property.Name, (SettingAttribute)customAttributes[0]);
            }
            return dictionary;
        }
    }
}