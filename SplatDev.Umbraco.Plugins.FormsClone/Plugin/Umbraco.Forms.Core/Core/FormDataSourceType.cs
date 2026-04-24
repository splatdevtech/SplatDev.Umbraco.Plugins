
// Type: Umbraco.Forms.Core.FormDataSourceType
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Reflection;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Models;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Core
{
    public abstract class FormDataSourceType : ProviderBase, IFormDataSourceType, IDisposable
    {
        private bool _supportsGetRecords = true;
        private bool _supportsInsert = true;
        private bool _supportsPrevalues = true;

        public virtual bool SupportsGetRecords
        {
            get => this._supportsGetRecords;
            set => this._supportsGetRecords = value;
        }

        public virtual bool SupportsInsert
        {
            get => this._supportsInsert;
            set => this._supportsInsert = value;
        }

        public virtual bool SupportsPreValues
        {
            get => this._supportsPrevalues;
            set => this._supportsPrevalues = value;
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
            this.DataSource = datasource;
            foreach (string key in datasource.Settings.Keys)
            {
                try
                {
                    this.GetType().InvokeMember(key, BindingFlags.SetProperty, null, this, new object[1]
                    {
             datasource.Settings[key]
                    });
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
            Dictionary<string, SettingAttribute> dictionary = new Dictionary<string, SettingAttribute>(StringComparer.OrdinalIgnoreCase);
            foreach (PropertyInfo property in this.GetType().GetProperties())
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
