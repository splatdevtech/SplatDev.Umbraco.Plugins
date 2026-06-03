using System.Runtime.CompilerServices;
using SplatDev.DigitalBookCurator.Core.Context;
using SplatDev.DigitalBookCurator.Core.Models;

namespace SplatDev.DigitalBookCurator.Core.Repositories;

public class FileManagerService(CuratorDbContext context)
{
    public async IAsyncEnumerable<BookImportResult> ProcessUploadedAsync(
        string sourcePath,
        string destinationPath,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        if (!Directory.Exists(sourcePath))
        {
            yield return new BookImportResult { Success = false, ErrorMessage = $"Source path not found: {sourcePath}" };
            yield break;
        }

        Directory.CreateDirectory(destinationPath);

        var files = Directory.GetFiles(sourcePath, "*.pdf");
        foreach (var file in files)
        {
            if (cancellationToken.IsCancellationRequested) yield break;

            var fileInfo = new FileInfo(file);
            var destFile = Path.Combine(destinationPath, fileInfo.Name);

            BookImportResult result;
            try
            {
                File.Move(file, destFile, overwrite: true);

                var book = new Book
                {
                    Title = Path.GetFileNameWithoutExtension(fileInfo.Name),
                    FileName = fileInfo.Name,
                    FilePath = destFile,
                    FileSize = fileInfo.Length,
                    FileType = fileInfo.Extension,
                    CreatedAt = DateTime.UtcNow
                };

                context.Books.Add(book);
                await context.SaveChangesAsync(cancellationToken);

                result = new BookImportResult { Success = true, FileName = fileInfo.Name, Book = book };
            }
            catch (Exception ex)
            {
                result = new BookImportResult { Success = false, FileName = fileInfo.Name, ErrorMessage = ex.Message };
            }

            yield return result;
        }
    }
}
