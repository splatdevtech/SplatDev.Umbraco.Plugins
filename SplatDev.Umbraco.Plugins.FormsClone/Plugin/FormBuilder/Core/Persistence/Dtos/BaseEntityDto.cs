using NPoco;

using Umbraco.Cms.Infrastructure.Persistence.DatabaseAnnotations;

namespace FormBuilder.Core.Persistence.Dtos
{
    public class BaseEntityDto : BaseDto
    {
        [Column(Name = "CreatedBy")]
        [NullSetting]
        public int? CreatedBy { get; set; }

        [Column(Name = "UpdatedBy")]
        [NullSetting]
        public int? UpdatedBy { get; set; }
    }
}