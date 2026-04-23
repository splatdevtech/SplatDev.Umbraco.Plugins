using FormBuilder.Core.Interfaces;

namespace FormBuilder.Core.Dto
{
    public class FormFileUploadOptionsDto : ISupportFileUploads
    {
        public bool AllowAllUploadExtensions { get; set; }

        public IEnumerable<string> AllowedUploadExtensions { get; set; } = [];

        public bool AllowMultipleFileUploads { get; set; }
    }
}