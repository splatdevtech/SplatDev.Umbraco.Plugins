
// Type: Umbraco.Forms.Core.Persistence.Dtos.BaseDto
// Assembly: Umbraco.Forms.Core, Version=15.1.1.0, Culture=neutral, PublicKeyToken=null
// MVID: 5D2CA832-F431-4612-A60D-6A240EFE1389

using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;


#nullable enable
namespace Umbraco.Forms.Core.Persistence.Dtos
{
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class BaseDto
    {
        [Column("Id")]
        [PrimaryKeyColumn(IdentitySeed = 0)]
        public int Id { get; set; }

        [Column(Name = "Key")]
        [Index(IndexTypes.UniqueNonClustered)]
        public Guid Key { get; set; }

        [Column(Name = "Name")]
        public string Name { get; set; } = string.Empty;

        [Column(Name = "Definition")]
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string Definition { get; set; } = string.Empty;

        [Column(Name = "Created")]
        public DateTime CreateDate { get; set; }

        [Column(Name = "Updated")]
        public DateTime UpdateDate { get; set; }
    }
}
