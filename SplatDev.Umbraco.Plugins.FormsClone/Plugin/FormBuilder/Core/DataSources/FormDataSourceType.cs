using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Fields;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Models;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Providers;

using System.Reflection;

namespace FormBuilder.Core.DataSources
{
    public abstract class FormDataSourceType : ProviderBase, IFormDataSourceType, IDisposable
    {
        private bool _supportsGetRecords = true;
        private bool _supportsInsert = true;
        private bool _supportsPrevalues = true;

        public virtual bool SupportsGetRecords
        {
            get => _supportsGetRecords;
            set => _supportsGetRecords = value;
        }

        public virtual bool SupportsInsert
        {
            get => _supportsInsert;
            set => _supportsInsert = value;
        }

        public virtual bool SupportsPreValues
        {
            get => _supportsPrevalues;
            set => _supportsPrevalues = value;
        }

        public FormDataSource? DataSource { get; private set; }

        public abstract List<Record> GetRecords(
          Form form,
          int page,
          int maxItems,
          object sortByField,
          RecordSorting order);

        public abstract Record InsertRecord(Record record);

        public abstract Dictionary<object, FormDataSourceField> GetAvailableFields();

        public abstract Dictionary<object, FormDataSourceField> GetMappedFields();

        public abstract Dictionary<object, string> GetPreValues(Field field, Form form);

        public void LoadSettings(FormDataSource datasource)
        {
            DataSource = datasource;
            foreach (string key in datasource.Settings.Keys)
            {
                try
                {
                    GetType().InvokeMember(key, BindingFlags.SetProperty, null, this,
                    [
             datasource.Settings[key]
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

        public abstract List<Exception> ValidateSettings();

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

        public abstract void Dispose();
    }
}