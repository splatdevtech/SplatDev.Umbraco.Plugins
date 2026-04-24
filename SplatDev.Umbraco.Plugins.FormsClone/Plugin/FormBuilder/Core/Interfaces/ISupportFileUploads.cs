namespace FormBuilder.Core.Interfaces
{
    internal interface ISupportFileUploads
    {
        bool AllowAllUploadExtensions { get; set; }

        IEnumerable<string> AllowedUploadExtensions { get; set; }

        bool AllowMultipleFileUploads { get; set; }
    }
}