namespace UmbracoCms.CodeFirst.Helpers
{
    using Examine;

    using System;
    using System.IO;
    using System.Linq;
    using System.Text;

    using Umbraco.Cms.Core;
    using Umbraco.Cms.Core.Models;
    using Umbraco.Cms.Core.Services;

    using UmbracoCms.Plugins;

    using File = System.IO.File;
    using IMediaType = UmbracoCms.CodeFirst.Interfaces.IMediaType;

    public static class MediaHelpers
    {
        #region Media Type

        // TODO: Inject IMediaTypeService and IDataTypeService via DI rather than static parameters.
        public static void AddMediaType(IMediaType mediaType, string propertyGroupName, IDataTypeService dataTypeService, IMediaTypeService? mediaTypeService = null)
        {
            //Allowed child nodes
            //var children = new List<ContentTypeSort>
            //    {
            //        new ContentTypeSort(FOLDER_ID, 0),
            //        new ContentTypeSort(IMAGE_ID, 1)
            //    };

            MediaType type = new MediaType(-1)
            {
                AllowedAsRoot = true,
                Name = mediaType.Name,
                Description = mediaType.Description,
                IsContainer = true,
                Icon = mediaType.Icon,
                Alias = mediaType.Alias,
                AllowedContentTypes = mediaType.AllowedContentTypes
            };


            foreach (var property in mediaType.Properties)
            {
                var prop = new PropertyType(dataTypeService.GetDataType(property.DataTypeDefinitionId)!, property.Alias)
                {
                    Name = property.Name,
                    Description = property.Description,
                    SortOrder = property.SortOrder,
                    Mandatory = property.Mandatory,
                    PropertyEditorAlias = property.PropertyEditorAlias,
                    ValidationRegExp = property.ValidationRegExp
                };
                type.AddPropertyType(prop, propertyGroupName);
            }
            mediaTypeService?.Save(type);
        }

        public static void RemoveMediaType(string alias, IMediaTypeService mediaTypeService)
        {
            var mediaType = mediaTypeService.Get(alias);
            mediaTypeService.Delete(mediaType!);
        }

        #endregion

        #region Folder
        public static void RemoveFolder(int mediaTypeId, string folderName, IMediaService mediaService)
        {
            var rootFolder = mediaService.GetPagedOfType(mediaTypeId, 1, 1, out long totalRecords, null).FirstOrDefault(x => x.Name == folderName);
            if (rootFolder != null) mediaService.Delete(rootFolder);
        }

        public static int AddFolder(string folderName, int parentId, IMediaService mediaService)
        {
            var folder = mediaService.CreateMedia(folderName, parentId, Default.MediaTypes.Alias.Folder);
            mediaService.Save(folder);
            return folder.Id;
        }

        public static int GetFolderId(string folderName)
        {
            IExamineManager examineManager = ExamineManager.Instance;
            if (!examineManager.TryGetIndex(Umbraco.Cms.Core.Constants.UmbracoIndexes.ExternalIndexName, out
            IIndex? index))
            {
                throw new InvalidOperationException($"No index found by name {Umbraco.Cms.Core.Constants.UmbracoIndexes.ExternalIndexName}");
            }
            var searcher = index.GetSearcher();
            var node = searcher.CreateQuery().NodeTypeAlias(Default.MediaTypes.Alias.Folder).And().Field("name", folderName).Execute().FirstOrDefault();
            return int.Parse(node!.Id);
        }

        #endregion

        #region Media
        public static int CreateMedia(string filePath, string name, string mediaType, int? parentId, IMediaService mediaService)
        {
            var media = mediaService.CreateMedia(name, parentId ?? -1, mediaType);
            FileStream stream = new FileStream(filePath, FileMode.Open);
            var fileName = new FileInfo(filePath).Name;
            // TODO: SetValue for media files in Umbraco 13+ requires IMediaFileManager - wire up via DI if needed
            // media.SetValue(contentTypeBaseServiceProvider, "umbracoFile", fileName, stream);
            media.SetValue("umbracoFile", fileName);
            mediaService.Save(media);

            return media.Id;
        }

        public static int CreateMedia(Stream? stream, string name, string mediaTypeAlias, int? parentId, IMediaService mediaService)
        {
            var media = mediaService.CreateMedia(name, parentId ?? -1, mediaTypeAlias);
            // TODO: SetValue for media files in Umbraco 13+ requires IMediaFileManager - wire up via DI if needed
            if (stream != null)
                media.SetValue("umbracoFile", name);
            mediaService.Save(media);

            return media.Id;
        }
        #endregion

        #region Upload
        // NOTE: HttpPostedFileBase (System.Web) is not available in .NET 8/10.
        // Use IFormFile from Microsoft.AspNetCore.Http instead.
        // TODO: Replace HttpPostedFileBase with IFormFile for ASP.NET Core compatibility.
        public static int Upload(Microsoft.AspNetCore.Http.IFormFile file, int parentId, IMediaService mediaService, string mediaType = "File")
        {
            var mediaFile = mediaService.CreateMedia(file.FileName, parentId, mediaType);
            // TODO: wire up file stream via IMediaFileManager in Umbraco 13+
            mediaService.Save(mediaFile);
            return mediaFile.Id;
        }

        #endregion

        #region Web.config
        public static void CreateMediaFolderWebConfig(string path)
        {
            const string config = @"
<?xml version='1.0' encoding='UTF-8'?>
<configuration>
  <system.webServer>
    <handlers>
      <clear />
      <add name='StaticFile' path='*' verb='*' modules='StaticFileModule,DefaultDocumentModule,DirectoryListingModule' resourceType='Either' requireAccess='Read' />
    </handlers>
  </system.webServer>
</configuration>
";
            using (var fileStream = File.Create(Path.Combine(path, "web.config")))
            {
                byte[] info = new UTF8Encoding(true).GetBytes(config);
                fileStream.Write(info, 0, info.Length);
                fileStream.Flush();
                fileStream.Close();
            }
        }

        #endregion
    }
}
