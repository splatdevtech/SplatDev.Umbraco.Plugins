
using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace Umbraco.Plugins.CacheManager.Models
{
    [TableName(TABLE_NAME)]
    [PrimaryKey("Id", AutoIncrement = true)]
    [ExplicitColumns]
    public class CacheWarmerEntry
    {
        public const string TABLE_NAME = "cacheWarmerLog";

        [PrimaryKeyColumn(AutoIncrement = true, IdentitySeed = 1)]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Url")]
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string Url { get; set; } = string.Empty;

        [Column("Description")]
        [SpecialDbType(SpecialDbTypes.NVARCHARMAX)]
        public string Description { get; set; } = string.Empty;

        [Column("CacheTime")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime? CacheTime { get; set; }

        [Column("Expires")]
        [NullSetting(NullSetting = NullSettings.Null)]
        public DateTime? Expires { get; set; }

        [Column("Task")]
        public int Task { get; set; }
    }
}
