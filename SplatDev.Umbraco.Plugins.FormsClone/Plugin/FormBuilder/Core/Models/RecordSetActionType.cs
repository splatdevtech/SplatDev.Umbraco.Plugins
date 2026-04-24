using FormBuilder.Core.Attributes;
using FormBuilder.Core.Enums;
using FormBuilder.Core.Interfaces;
using FormBuilder.Core.Persistence.Fields;
using FormBuilder.Core.Providers;

using System.Reflection;
using System.Runtime.Serialization;

namespace FormBuilder.Core.Models
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
            get => _needsConfirm;
            set => _needsConfirm = value;
        }

        [DataMember(Name = "confirmMessage")]
        public virtual string ConfirmMessage
        {
            get => _confirmMessage;
            set => _confirmMessage = value;
        }

        [DataMember(Name = "isAvailableForApprovedRecords")]
        public virtual bool IsAvailableForApprovedRecords
        {
            get => _isAvailableForApprovedRecords;
            set => _isAvailableForApprovedRecords = value;
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
                    GetType().InvokeMember(key, BindingFlags.SetProperty, null, this,
                    [
             settings[key]
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

        public virtual bool HasSettings()
        {
            bool flag = false;
            foreach (MemberInfo property in GetType().GetProperties())
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
            Dictionary<string, SettingAttribute> dictionary = new(StringComparer.OrdinalIgnoreCase);
            foreach (PropertyInfo property in GetType().GetProperties())
            {
                object[] customAttributes = property.GetCustomAttributes(true);
                if (customAttributes.Length != 0)
                    dictionary.Add(property.Name, (SettingAttribute)customAttributes[0]);
            }
            return dictionary;
        }
    }
}