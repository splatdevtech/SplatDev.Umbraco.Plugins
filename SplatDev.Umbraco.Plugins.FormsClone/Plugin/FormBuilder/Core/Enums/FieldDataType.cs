using System.Runtime.Serialization;

namespace FormBuilder.Core.Enums
{
    [DataContract(Name = "fieldDataType")]
    public enum FieldDataType
    {
        String,
        LongString,
        Integer,
        DateTime,
        Bit,
    }
}