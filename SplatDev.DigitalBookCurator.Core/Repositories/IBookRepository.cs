using SplatDev.DigitalBookCurator.Core.Models;
using SplatDev.DigitalBookCurator.Core.ViewModels;

namespace SplatDev.DigitalBookCurator.Core.Repositories;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllBooksAsync();
    Task<PagedResults<BookViewModel>> GetFilteredBooksAsync(PagedFilter filter);
    Task<Book?> GetBookByIdAsync(int id);
    Task<Book?> UpdateBookAsync(Book book);
    Task DeleteBookAsync(int id);
}
