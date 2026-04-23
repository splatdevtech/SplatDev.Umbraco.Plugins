using Microsoft.AspNetCore.Http;
using Umbraco.Cms.Core.IO;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.PropertyEditors;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Core.Strings;
using SplatDev.Umbraco.Plugins.Dropzone.Models;

namespace SplatDev.Umbraco.Plugins.Dropzone.Services;

public class DropzoneService : IDropzoneService
{
    private readonly IMediaService _mediaService;
    private readonly IContentTypeBaseServiceProvider _contentTypeBaseServiceProvider;
    private readonly MediaFileManager _mediaFileManager;
    private readonly MediaUrlGeneratorCollection _mediaUrlGenerators;
    private readonly IShortStringHelper _shortStringHelper;

    public DropzoneService(
        IMediaService mediaService,
        IContentTypeBaseServiceProvider contentTypeBaseServiceProvider,
        MediaFileManager mediaFileManager,
        MediaUrlGeneratorCollection mediaUrlGenerators,
        IShortStringHelper shortStringHelper)
    {
        _mediaService = mediaService;
        _contentTypeBaseServiceProvider = contentTypeBaseServiceProvider;
        _mediaFileManager = mediaFileManager;
        _mediaUrlGenerators = mediaUrlGenerators;
        _shortStringHelper = shortStringHelper;
    }

    public async Task<UploadResult> UploadFileAsync(IFormFile file, UploadRequest request)
    {
        try
        {
            var parentId = request.ParentMediaId ?? -1;
            var mediaItem = _mediaService.CreateMedia(
                Path.GetFileNameWithoutExtension(file.FileName),
                parentId,
                "Image");

            await using var stream = file.OpenReadStream();
            mediaItem.SetValue(
                _contentTypeBaseServiceProvider,
                "umbracoFile",
                file.FileName,
                stream);

            _mediaService.Save(mediaItem);

            var url = mediaItem.GetUrl("umbracoFile", _mediaUrlGenerators);

            return new UploadResult
            {
                Success = true,
                MediaKey = mediaItem.Key.ToString(),
                Url = url
            };
        }
        catch (Exception ex)
        {
            return new UploadResult
            {
                Success = false,
                Error = ex.Message
            };
        }
    }

    public Task<IEnumerable<IMedia>> GetMediaItemsAsync(int? parentId)
    {
        var items = parentId.HasValue
            ? _mediaService.GetPagedChildren(parentId.Value, 0, 100, out _)
            : _mediaService.GetPagedRootMedia(0, 100, out _);

        return Task.FromResult(items);
    }

    public Task<bool> DeleteMediaAsync(Guid mediaKey)
    {
        var media = _mediaService.GetById(mediaKey);
        if (media == null) return Task.FromResult(false);

        _mediaService.Delete(media);
        return Task.FromResult(true);
    }
}
