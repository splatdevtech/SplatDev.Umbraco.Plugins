using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

using SplatDev.uPlugins.Backups.Extensions;
using SplatDev.uPlugins.Backups.Models;

using Microsoft.Data.SqlClient;
using System.IO.Compression;
using System.Net;

using Umbraco.Cms.Web.Common.Authorization;
using Umbraco.Cms.Web.Common.Controllers;

namespace SplatDev.uPlugins.Backups.Controllers
{
    public class BackupController : UmbracoApiController
    {
        public string DestinationPath { get; private set; }
        public string RootPath { get; private set; }
        public string WWWRootPath { get; private set; }
        public string DatabasePath { get; private set; }
        public string FilesPath { get; private set; }
        private readonly SqlConnection? sqlConnection;
        private const string files = "Files_{0}.files.zip";
        private const string database = "Database_{0}__{1}.bak";
        private const string TIMESTAMP = "DATE--yyyy-MM-dd---HH-mm-ss";
        private const string MEDIA_VIRTUAL_PATH = "/media/";
        public BackupDetails? BackupDetails { get; private set; }

        private readonly IConfiguration configuration;
        private readonly IWebHostEnvironment _env;
        public BackupController(IConfiguration config, IWebHostEnvironment env)
        {
            configuration = config;
            _env = env;

            sqlConnection = GetSqlConnection();
            WWWRootPath = $"{_env.WebRootPath}\\media";
            RootPath = _env.ContentRootPath;
            DestinationPath = WWWRootPath;
            DatabasePath = Path.Combine(DestinationPath, database);
            FilesPath = Path.Combine(WWWRootPath, files);

            if (!Directory.Exists(DestinationPath))
            {
                Directory.CreateDirectory(DestinationPath);
            }

            if (sqlConnection is null) return;
            BackupDetails = new BackupDetails
            {
                ConnectionString = sqlConnection?.ConnectionString!,
                Database = sqlConnection?.Database!,
                Server = sqlConnection?.DataSource!,
                DatabasePath = DatabasePath,
                FilesPath = FilesPath,
                RootPath = RootPath,
                DestinationPath = DestinationPath
            };
        }

        [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
        [Route("umbraco/backoffice/api/Backup/GetBackupDetails")]
        public async Task<BackupDetails?> GetBackupDetails()
        {
            await Task.FromResult(0);
            return BackupDetails;
        }

        [HttpDelete]
        [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
        [Route("umbraco/backoffice/api/Backup/DeleteBackup")]
        public async Task<bool> DeleteBackup(string filename)
        {
            var path = Path.Combine(DestinationPath, filename);
            System.IO.File.Delete(path);
            await Task.FromResult(0);
            return true;
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
        [Route("umbraco/backoffice/api/Backup/GetBackupsPerformed")]
        public async Task<BackupPerformedDetails> GetBackupsPerformed()
        {
            await Task.FromResult(0);
            var backupsPerformed = new BackupPerformedDetails();
            if (Directory.Exists(DestinationPath))
            {
                var files = Directory.GetFiles(DestinationPath, "*.files.zip");
                var dbs = Directory.GetFiles(DestinationPath, "*.bak.zip");

                foreach (var file in files)
                {
                    var d = new FileInfo(file);
                    backupsPerformed.FileBackups.Add(d.Name, new FileDetails
                    {
                        Fullname = $"{MEDIA_VIRTUAL_PATH}{d.Name}",
                        CreateDate = d.CreationTimeUtc
                    });
                }

                foreach (var db in dbs)
                {
                    var d = new FileInfo(db);
                    backupsPerformed.DatabaseBackups.Add(d.Name, new FileDetails
                    {
                        Fullname = $"{MEDIA_VIRTUAL_PATH}{d.Name}",
                        CreateDate = d.CreationTimeUtc
                    });
                }
            }
            return backupsPerformed;
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
        [Route("umbraco/backoffice/api/Backup/FilesBackup")]
        public async Task<bool> FilesBackup()
        {
            string folderToBackup = _env.ContentRootPath;

            // Specify the path for the zip file
            string backupZipPath = _env.WebRootPath + "\\media\\" + files;
            await Task.FromResult(0);

            if (System.IO.File.Exists(string.Format(backupZipPath, DateTime.Now.ToString(TIMESTAMP))))
            {
                System.IO.File.Delete(string.Format(backupZipPath, DateTime.Now.ToString(TIMESTAMP)));
            }

            try
            {
                // Create a temporary directory for backup
                folderToBackup ??= "";
                string tempBackupDir = Path.Combine(Path.GetDirectoryName(folderToBackup)!, "BackupTemp");
                Directory.CreateDirectory(tempBackupDir);

                // Copy the entire folder and its contents to the temporary directory, excluding "ExcludeFolder"
                ZipExtensions.CopyDirectory(folderToBackup, tempBackupDir, "umbraco");

                // Create a zip file that includes the backup folder
                ZipFile.CreateFromDirectory(tempBackupDir, string.Format(backupZipPath, DateTime.Now.ToString(TIMESTAMP)));

                // Clean up: delete the temporary backup directory
                Directory.Delete(tempBackupDir, true);

                Console.WriteLine("Backup created successfully: " + backupZipPath);

                return true;


            }
            catch (Exception ex)
            {
                Console.WriteLine("Error creating backup: " + ex.Message);
                return false;

            }

        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
        [Route("umbraco/backoffice/api/Backup/DatabaseBackup")]
        public async Task<bool> DatabaseBackup()
        {
            if (sqlConnection is null)
            {
                //Db file will be backed up along with the files
                return true;
            }

            var path = string.Format(DatabasePath, sqlConnection.Database, DateTime.Now.ToString(TIMESTAMP));
            SqlCommand command = new("BACKUP DATABASE [" + sqlConnection.Database + "] TO DISK ='" + path + "';", sqlConnection);
            sqlConnection.Open();
            var result = await command.ExecuteNonQueryAsync();
            sqlConnection.Close();
            if (result == -1)
            {
                var entryName = string.Format(database, sqlConnection.Database, DateTime.Now.ToString(TIMESTAMP));
                using ZipArchive zip = ZipFile.Open($"{path}.zip", ZipArchiveMode.Create);
                zip.CreateEntryFromFile(path, entryName);
                System.IO.File.Delete(path);
            }
            return true;
        }

        [HttpPost]
        [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
        [Route("umbraco/backoffice/api/Backup/FullBackup")]
        public async Task<Response> FullBackup()
        {
            try
            {
                var success = await FilesBackup();
                success = success && await DatabaseBackup();

                if (success)
                {
                    return new Response
                    {
                        StatusCode = HttpStatusCode.OK,
                        Success = true
                    };
                }
                else
                {
                    return new Response
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        Success = false
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Details = ex,
                    Message = ex.Message,
                    StatusCode = HttpStatusCode.InternalServerError,
                    Success = false
                };
            }
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
        [Route("umbraco/backoffice/api/Backup/DownloadBackupFiles")]
        public PhysicalFileResult DownloadBackupFiles()
        {
            return new PhysicalFileResult(FilesPath, "application/zip, application/octet-stream, application/x-zip-compressed, multipart/x-zip");
        }

        [HttpGet]
        [Authorize(Policy = AuthorizationPolicies.BackOfficeAccess)]
        [Route("umbraco/backoffice/api/Backup/DownloadBackupDb")]
        public PhysicalFileResult DownloadBackupDb()
        {
            return new PhysicalFileResult(DatabasePath, "application/octet-stream");
        }

        #region Helpers

        private string? GetConnectionString()
        {
            var connStringSettings = configuration.GetConnectionString("umbracoDbDSN");
            return connStringSettings;
        }

        private SqlConnection? GetSqlConnection()
        {
            var connectionString = GetConnectionString();
            if (connectionString is null || connectionString.Contains("|DataDirectory|")) return default;
            return new SqlConnection(connectionString);
        }
        #endregion
    }
}
