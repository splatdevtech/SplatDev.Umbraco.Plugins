using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Plugins.RedirectManager.Models
{
    [TableName(TABLE_NAME)]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class RedirectUrl
    {
        public const string TABLE_NAME = "redirectUrls";

        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Url")]
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string Url { get; set; } = string.Empty;

        [Column("RedirectToUrl")]
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string RedirectToUrl { get; set; } = string.Empty;

        [Column("CreatedOn")]
        public DateTime? CreatedOn { get; set; }
    }
}
