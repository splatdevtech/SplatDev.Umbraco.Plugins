
// Type: Umbraco.Forms.Core.Providers.FieldTypes.ReCaptchaResponse
// Assembly: Umbraco.Forms.Core.Providers, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 747CF8D1-007A-4431-9ECE-D9510ED65D68

using System;
using System.Runtime.Serialization;


#nullable enable
namespace Umbraco.Forms.Core.Providers.FieldTypes
{
  [DataContract]
  internal sealed class ReCaptchaResponse
  {
    [DataMember(Name = "success")]
    public bool Success { get; set; }

    [DataMember(Name = "challenge_ts")]
    public DateTime ChallengeTimestamp { get; set; }

    [DataMember(Name = "hostname")]
    public string Hostname { get; set; } = string.Empty;

    [DataMember(Name = "score")]
    public double Score { get; set; }

    [DataMember(Name = "action")]
    public string Action { get; set; } = string.Empty;
  }
}
