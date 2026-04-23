using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.Models;
using UmbracoCms.Plugins.Dropzone.Models;

namespace UmbracoCms.Plugins.Dropzone.Services;

public interface IDropzoneService
{
    Task<UploadResult> UploadFileAsync(IFormFile file, UploadRequest request);
    Task<IEnumerable<IMedia>> GetMediaItemsAsync(int? parentId);
    Task<bool> DeleteMediaAsync(Guid mediaKey);
}
