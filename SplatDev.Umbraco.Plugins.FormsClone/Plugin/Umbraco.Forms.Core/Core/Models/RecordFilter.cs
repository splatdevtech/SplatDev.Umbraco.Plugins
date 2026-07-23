
// Type: Umbraco.Forms.Core.Models.RecordFilter
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Umbraco.Forms.Core.Enums;


#nullable enable
namespace Umbraco.Forms.Core.Models
{
  [DataContract(Name = "recordFilter")]
  public class RecordFilter
  {
    internal const int DefaultPageSize = 20;

    [DataMember(Name = "skip")]
    public int Skip { get; set; }

    [DataMember(Name = "take")]
    public int Take { get; set; } = 20;

    [DataMember(Name = "memberKey")]
    public string? MemberKey { get; set; }

    [DataMember(Name = "sortBy")]
    public string SortBy { get; set; } = string.Empty;

    [DataMember(Name = "sortOrder")]
    public RecordSorting SortOrder { get; set; }

    [DataMember(Name = "startDate")]
    public DateTime StartDate { get; set; }

    [DataMember(Name = "endDate")]
    public DateTime EndDate { get; set; }

    [DataMember(Name = "filter")]
    public string? Filter { get; set; }

    [DataMember(Name = "states")]
    public List<FormState> States { get; set; } = new List<FormState>();

    [DataMember(Name = "localTimeOffset")]
    public int LocalTimeOffset { get; set; }

    [DataMember(Name = "recordId")]
    public Guid RecordId { get; set; }
  }
}
