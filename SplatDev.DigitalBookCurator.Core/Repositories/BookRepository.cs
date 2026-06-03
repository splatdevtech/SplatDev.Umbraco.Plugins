using Microsoft.EntityFrameworkCore;
using SplatDev.DigitalBookCurator.Core.Context;
using SplatDev.DigitalBookCurator.Core.Models;
using SplatDev.DigitalBookCurator.Core.ViewModels;

namespace SplatDev.DigitalBookCurator.Core.Repositories;

public class BookRepository(CuratorDbContext context) : IBookRepository
{
    public async Task<IEnumerable<Book>> GetAllBooksAsync()
    {
        return await context.Books.OrderByDescending(b => b.CreatedAt).ToListAsync();
    }

    public async Task<PagedResults<BookViewModel>> GetFilteredBooksAsync(PagedFilter filter)
    {
        var query = context.Books.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var term = filter.SearchTerm.ToLowerInvariant();
            query = query.Where(b =>
                b.Title.ToLower().Contains(term) ||
                b.Author.ToLower().Contains(term) ||
                b.FileName.ToLower().Contains(term));
        }

        var totalCount = await query.CountAsync();

        query = filter.SortBy?.ToLowerInvariant() switch
        {
            "title" => filter.SortDescending ? query.OrderByDescending(b => b.Title) : query.OrderBy(b => b.Title),
            "author" => filter.SortDescending ? query.OrderByDescending(b => b.Author) : query.OrderBy(b => b.Author),
            "size" => filter.SortDescending ? query.OrderByDescending(b => b.FileSize) : query.OrderBy(b => b.FileSize),
            _ => query.OrderByDescending(b => b.CreatedAt)
        };

        var items = await query
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .Select(b => new BookViewModel
            {
                Id = b.Id,
                Title = b.Title,
                Author = b.Author,
                FileName = b.FileName,
                FileType = b.FileType,
                FileSize = b.FileSize,
                PageCount = b.PageCount,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();

        return new PagedResults<BookViewModel>
        {
            Items = items,
            TotalCount = totalCount,
            Page = filter.Page,
            PageSize = filter.PageSize
        };
    }

    public async Task<Book?> GetBookByIdAsync(int id)
    {
        return await context.Books.FindAsync(id);
    }

    public async Task<Book?> UpdateBookAsync(Book book)
    {
        var existing = await context.Books.FindAsync(book.Id);
        if (existing is null) return null;

        existing.Title = book.Title;
        existing.Author = book.Author;
        existing.Description = book.Description;
        existing.Isbn = book.Isbn;
        existing.ModifiedAt = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task DeleteBookAsync(int id)
    {
        var book = await context.Books.FindAsync(id);
        if (book is null) return;

        book.IsDeleted = true;
        book.ModifiedAt = DateTime.UtcNow;
        await context.SaveChangesAsync();
    }
}
