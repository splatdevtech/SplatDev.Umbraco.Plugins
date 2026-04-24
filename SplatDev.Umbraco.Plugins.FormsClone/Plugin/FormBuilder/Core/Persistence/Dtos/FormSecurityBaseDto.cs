using FormBuilder.Core.Enums;

using NPoco;

using System.Runtime.Serialization;

namespace FormBuilder.Core.Persistence.Dtos
{
    [DataContract(Name = "formSecurityBase")]
    public abstract class FormSecurityBaseDto
    {
        [Ignore]
        [DataMember(Name = "formName")]
        public string FormName { get; set; } = string.Empty;

        [Ignore]
        [DataMember(Name = "formCreated")]
        public DateTime FormCreated { get; set; }

        [Ignore]
        [DataMember(Name = "fields")]
        public string Fields { get; set; } = string.Empty;

        [DataMember(Name = "hasAccess")]
        public bool HasAccess { get; set; }

        [Ignore]
        public FormSecurityType SecurityType { get; set; }

        [DataMember(Name = "allowInEditor")]
        public bool AllowInEditor { get; set; }

        [Column(Name = "SecurityType")]
        public int SecurityTypeInt { get; set; }
    }
}