
using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace SplatDev.Umbraco.Plugins.CacheManager.Models
{
    [TableName(TABLE_NAME)]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class UrlNotFound
    {
        public const string TABLE_NAME = "urlNotFound";

        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Url")]
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string Url { get; set; } = string.Empty;
    }
}
