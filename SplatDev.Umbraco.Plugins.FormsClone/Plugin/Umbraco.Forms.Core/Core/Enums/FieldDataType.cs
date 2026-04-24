
// Type: Umbraco.Forms.Core.Enums.FieldDataType
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System.Runtime.Serialization;

namespace Umbraco.Forms.Core.Enums
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
