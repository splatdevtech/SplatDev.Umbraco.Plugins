
// Type: Umbraco.Forms.Core.Providers.Models.DocumentMapper
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Providers.Models
{
  [DataContract(Name = "documentMapper")]
  public class DocumentMapper
  {
    [DataMember(Name = "doctype")]
    [JsonPropertyName("doctype")]
    public string DocTypeAlias { get; set; } = string.Empty;

    [DataMember(Name = "nameField")]
    public string NameField { get; set; } = string.Empty;

    [DataMember(Name = "nameStaticValue")]
    public string NameStaticValue { get; set; } = string.Empty;

    [DataMember(Name = "properties")]
    public IEnumerable<DocumentMapping> Properties { get; set; } = Enumerable.Empty<DocumentMapping>();
  }
}
