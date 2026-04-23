
// Type: Umbraco.Forms.Core.Models.RecordSetActionType
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Reflection;
using System.Runtime.Serialization;

using Umbraco.Forms.Core.Attributes;
using Umbraco.Forms.Core.Enums;
using Umbraco.Forms.Core.Interfaces;
using Umbraco.Forms.Core.Persistence.Dtos;
using Umbraco.Forms.Core.Providers;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
    [DataContract(Name = "recordSetActionType")]
    public abstract class RecordSetActionType : ProviderBase, IRecordSetActionType
    {
        private bool _needsConfirm;
        private string _confirmMessage = string.Empty;
        private bool _isAvailableForApprovedRecords = true;

        [DataMember(Name = "icon")]
        public new virtual string Icon { get; set; } = string.Empty;

        [DataMember(Name = "needsConfirm")]
        public virtual bool NeedsConfirm
        {
            get => this._needsConfirm;
            set => this._needsConfirm = value;
        }

        [DataMember(Name = "confirmMessage")]
        public virtual string ConfirmMessage
        {
            get => this._confirmMessage;
            set => this._confirmMessage = value;
        }

        [DataMember(Name = "isAvailableForApprovedRecords")]
        public virtual bool IsAvailableForApprovedRecords
        {
            get => this._isAvailableForApprovedRecords;
            set => this._isAvailableForApprovedRecords = value;
        }

        public abstract Task<RecordActionStatus> ExecuteAsync(
          List<Record> records,
          Form form);

        public abstract List<Exception> ValidateSettings();

        public void LoadSettings(Dictionary<string, string> settings)
        {
            foreach (string key in settings.Keys)
            {
                try
                {
                    this.GetType().InvokeMember(key, BindingFlags.SetProperty, null, this, new object[1]
                    {
             settings[key]
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

        public virtual bool HasSettings()
        {
            bool flag = false;
            foreach (MemberInfo property in this.GetType().GetProperties())
            {
                if (property.GetCustomAttributes(true).Length != 0)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        public virtual Dictionary<string, SettingAttribute> Settings()
        {
            Dictionary<string, SettingAttribute> dictionary = new Dictionary<string, SettingAttribute>(StringComparer.OrdinalIgnoreCase);
            foreach (PropertyInfo property in this.GetType().GetProperties())
            {
                object[] customAttributes = property.GetCustomAttributes(true);
                if (customAttributes.Length != 0)
                    dictionary.Add(property.Name, (SettingAttribute)customAttributes[0]);
            }
            return dictionary;
        }
    }
}
