

using NPoco;

using System.Runtime.Serialization;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace SplatDev.Umbraco.Plugins.Countries.Models
{
    [TableName(TABLE_NAME)]
    [ExplicitColumns]
    [PrimaryKey(PRIMARY_KEY)]
    [DataContract]
    public class Country
    {
        #region Schema
        public const string TABLE_NAME = "countries";
        public const string PRIMARY_KEY = "Id";

        [Column("id")]
        [DataMember(IsRequired = true, Name = "id")]
        [PrimaryKeyColumn(AutoIncrement = true, Clustered = false, IdentitySeed = 0)]
        public int Id { get; set; }

        [Column("numCode")]
        [DataMember(IsRequired = false, Name = "numCode")]
        public int NumCode { get; set; }

        [Column("alpha2Code")]
        [DataMember(IsRequired = false, Name = "alpha2Code")]
        public string Alpha2Code { get; set; } = string.Empty;

        [Column("alpha3Code")]
        [DataMember(IsRequired = false, Name = "alpha3Code")]
        public string Alpha3Code { get; set; } = string.Empty;

        [Column("enShortName")]
        [DataMember(IsRequired = false, Name = "enShortName")]
        public string EnShortName { get; set; } = string.Empty;

        [Column("nationality")]
        [DataMember(IsRequired = false, Name = "nationality")]
        public string Nationality { get; set; } = string.Empty;
        #endregion
    }
}
