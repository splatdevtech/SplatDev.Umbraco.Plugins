#region License
/*
    Copyright (c) 2015, Paweł Hofman (CodeTitans)
    All Rights Reserved.

    Licensed under MIT License
    For more information please visit:

    https://github.com/phofman/zip/blob/master/LICENSE
        or
    http://opensource.org/licenses/MIT


    For latest source code, documentation, samples
    and more information please visit:

    https://github.com/phofman/zip
*/
#endregion

using System.IO.Compression;
using System.Text;

namespace SplatDev.uPlugins.Backups.Extensions
{
    /// <summary>
    /// Helper class to simplify operations over ZIP archive.
    /// </summary>
    public static class ZipExtensions
    {
        /// <summary>
        /// Creates a zip archive that contains the files and directories from the specified directory.
        /// </summary>
        public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName)
        {
            CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, CompressionLevel.Optimal, false, null);
        }

        /// <summary>
        /// Creates a zip archive that contains the files and directories from the specified directory.
        /// </summary>
        public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel compressionLevel, bool includeBaseDirectory)
        {
            CreateFromDirectory(sourceDirectoryName, destinationArchiveFileName, compressionLevel, includeBaseDirectory, null);
        }

        /// <summary>
        /// Creates a zip archive that contains the files and directories from the specified directory.
        /// </summary>
        public static void CreateFromDirectory(string sourceDirectoryName, string destinationArchiveFileName, CompressionLevel compressionLevel, bool includeBaseDirectory, Encoding? entryNameEncoding)
        {
            _ = entryNameEncoding;
            if (string.IsNullOrEmpty(sourceDirectoryName))
                throw new ArgumentNullException(nameof(sourceDirectoryName));
            if (string.IsNullOrEmpty(destinationArchiveFileName))
                throw new ArgumentNullException(nameof(destinationArchiveFileName));

            var filesToAdd = Directory.GetFiles(sourceDirectoryName, "*", SearchOption.AllDirectories);
            var entryNames = GetEntryNames(filesToAdd, sourceDirectoryName, includeBaseDirectory);

            using var zipFileStream = new FileStream(destinationArchiveFileName, FileMode.Create);
            using var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Create);
            for (int i = 0; i < filesToAdd.Length; i++)
            {
                archive.CreateEntryFromFile(filesToAdd[i], entryNames[i], compressionLevel);
            }
        }

        public static void CreateFromDirectory(
            string sourceDirectoryName
        , string destinationArchiveFileName
        , CompressionLevel compressionLevel
        , bool includeBaseDirectory
        , Encoding entryNameEncoding
        , Predicate<string> filter
        )
        {
            if (string.IsNullOrEmpty(sourceDirectoryName))
            {
                throw new ArgumentNullException(nameof(sourceDirectoryName));
            }
            if (string.IsNullOrEmpty(destinationArchiveFileName))
            {
                throw new ArgumentNullException(nameof(destinationArchiveFileName));
            }
            var filesToAdd = Directory.GetFiles(sourceDirectoryName, "*", SearchOption.AllDirectories);
            var entryNames = GetEntryNames(filesToAdd, sourceDirectoryName, includeBaseDirectory);
            using var zipFileStream = new FileStream(destinationArchiveFileName, FileMode.Create);
            using var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Create);
            for (int i = 0; i < filesToAdd.Length; i++)
            {
                // Add the following condition to do filtering:
                if (!filter(filesToAdd[i]))
                {
                    continue;
                }
                _ = entryNameEncoding;
                archive.CreateEntryFromFile(filesToAdd[i], entryNames[i], compressionLevel);
            }
        }

        private static string[] GetEntryNames(string[] names, string sourceFolder, bool includeBaseName)
        {
            if (names == null || names.Length == 0)
                return [];

            if (includeBaseName)
                sourceFolder = Path.GetDirectoryName(sourceFolder)!;

            int length = string.IsNullOrEmpty(sourceFolder) ? 0 : sourceFolder.Length;
            if (length > 0 && sourceFolder != null && sourceFolder[length - 1] != Path.DirectorySeparatorChar && sourceFolder[length - 1] != Path.AltDirectorySeparatorChar)
                length++;

            var result = new string[names.Length];
            for (int i = 0; i < names.Length; i++)
            {
                result[i] = names[i][length..];
            }

            return result;
        }

        /// <summary>
        /// Extracts all the files in the specified zip archive to a directory on the file system.
        /// </summary>
        public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName)
        {
            ExtractToDirectory(sourceArchiveFileName, destinationDirectoryName, null);
        }

        /// <summary>
        /// Extracts all the files in the specified zip archive to a directory on the file system.
        /// </summary>
        public static void ExtractToDirectory(string sourceArchiveFileName, string destinationDirectoryName, Encoding? entryNameEncoding)
        {
            _ = entryNameEncoding;
            if (string.IsNullOrEmpty(sourceArchiveFileName))
                throw new ArgumentNullException(nameof(sourceArchiveFileName));
            if (string.IsNullOrEmpty(destinationDirectoryName))
                throw new ArgumentNullException(nameof(destinationDirectoryName));

            using var zipFileStream = new FileStream(sourceArchiveFileName, FileMode.Open);
            using var archive = new ZipArchive(zipFileStream, ZipArchiveMode.Read);
            archive.ExtractToDirectory(destinationDirectoryName);
        }

        /// <summary>
        /// Opens a zip archive at the specified path and in the specified mode. 
        /// </summary>
        public static ZipArchive Open(string archiveFileName, ZipArchiveMode mode)
        {
            return Open(archiveFileName, mode, null);
        }

        /// <summary>
        /// Opens a zip archive at the specified path and in the specified mode. 
        /// </summary>
        public static ZipArchive Open(string archiveFileName, ZipArchiveMode mode, Encoding? entryNameEncoding)
        {
            _ = entryNameEncoding;
            if (string.IsNullOrEmpty(archiveFileName))
                throw new ArgumentNullException(nameof(archiveFileName));

            return mode switch
            {
                ZipArchiveMode.Create => new ZipArchive(new FileStream(archiveFileName, FileMode.Create), ZipArchiveMode.Create),
                ZipArchiveMode.Update => new ZipArchive(new FileStream(archiveFileName, FileMode.OpenOrCreate), ZipArchiveMode.Update),
                ZipArchiveMode.Read => new ZipArchive(new FileStream(archiveFileName, FileMode.Open), ZipArchiveMode.Read),
                _ => throw new IOException("Unsupported archive mode"),
            };
        }

        /// <summary>
        /// Opens a zip archive for reading at the specified path.
        /// </summary>
        public static ZipArchive OpenRead(string archiveFileName)
        {
            return Open(archiveFileName, ZipArchiveMode.Read, null);
        }

        public static void CopyDirectory(string sourceDir, string destDir, string excludeFolder)
        {
            foreach (string dirPath in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                // Skip copying if the directory is the excluded folder
                if (dirPath.Equals(Path.Combine(sourceDir, excludeFolder), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                Directory.CreateDirectory(dirPath.Replace(sourceDir, destDir));
            }

            foreach (string newPath in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                // Skip copying .zip files in media folder
                string parentFolder = Path.GetDirectoryName(newPath)!;
                if (newPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase) && parentFolder.EndsWith("media"))
                {
                    continue;
                }

                // Skip copying if the file is within the excluded folder
                if (newPath.StartsWith(Path.Combine(sourceDir, "umbraco"), StringComparison.OrdinalIgnoreCase) || newPath.StartsWith(Path.Combine(sourceDir, ".vs"), StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }
                System.IO.File.Copy(newPath, newPath.Replace(sourceDir, destDir), true);
            }
        }

    }
}