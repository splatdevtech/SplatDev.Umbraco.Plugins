using System.Runtime.Serialization;

namespace FormBuilder.Core.Providers.FieldTypes
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