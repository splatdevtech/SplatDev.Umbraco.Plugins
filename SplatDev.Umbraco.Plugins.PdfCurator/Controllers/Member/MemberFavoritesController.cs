using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using PdfCurator.Core.Data;
using PdfCurator.Core.Entities;

using SplatDev.Umbraco.Plugins.PdfCurator.Authorization;
using SplatDev.Umbraco.Plugins.PdfCurator.Entities;
using SplatDev.Umbraco.Plugins.PdfCurator.Migrations;

using Umbraco.Cms.Core.Security;

namespace SplatDev.Umbraco.Plugins.PdfCurator.Controllers.Member;

[ApiController]
[MemberAuthorize]
[Route("umbraco/pdfcurator/api/v1/member/favorites")]
public class MemberFavoritesController : ControllerBase
{
    private readonly IDbContextFactory<MemberDbContext> _dbFactory;
    private readonly IMemberManager _memberManager;

    public MemberFavoritesController(
        IDbContextFactory<MemberDbContext> dbFactory,
        IMemberManager memberManager)
    {
        _dbFactory = dbFactory;
        _memberManager = memberManager;
    }

    [HttpGet]
    public async Task<IActionResult> GetFavorites(CancellationToken ct = default)
    {
        var memberKey = await GetMemberKeyAsync();
        if (memberKey is null)
        {
            return Unauthorized();
        }

        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        var favs = await (
            from favorite in db.Favorites
            join book in db.Set<Book>() on favorite.BookId equals book.Id
            where favorite.MemberKey == memberKey.Value && book.Status == BookStatus.Filed
            orderby favorite.CreatedAt descending
            select new
            {
                favorite.BookId,
                bookTitle = book.Title,
                bookAuthor = book.Author,
                favorite.CreatedAt,
            }).ToListAsync(ct);

        return Ok(favs);
    }

    [HttpPut("{bookId:int}")]
    public async Task<IActionResult> AddFavorite(int bookId, CancellationToken ct = default)
    {
        var memberKey = await GetMemberKeyAsync();
        if (memberKey is null)
        {
            return Unauthorized();
        }

        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        var existing = await db.Favorites
            .FirstOrDefaultAsync(f => f.MemberKey == memberKey.Value && f.BookId == bookId, ct);

        if (existing is not null)
        {
            return Ok(new { favorited = true, bookId });
        }

        db.Favorites.Add(new MemberFavorite { MemberKey = memberKey.Value, BookId = bookId });
        await db.SaveChangesAsync(ct);
        return Ok(new { favorited = true, bookId });
    }

    [HttpDelete("{bookId:int}")]
    public async Task<IActionResult> RemoveFavorite(int bookId, CancellationToken ct = default)
    {
        var memberKey = await GetMemberKeyAsync();
        if (memberKey is null)
        {
            return Unauthorized();
        }

        await using var db = await _dbFactory.CreateDbContextAsync(ct);
        var fav = await db.Favorites
            .FirstOrDefaultAsync(f => f.MemberKey == memberKey.Value && f.BookId == bookId, ct);

        if (fav is null)
        {
            return NotFound();
        }

        db.Favorites.Remove(fav);
        await db.SaveChangesAsync(ct);
        return Ok(new { removed = true, bookId });
    }

    private async Task<Guid?> GetMemberKeyAsync()
    {
        var member = await _memberManager.GetCurrentMemberAsync();
        return member?.Key;
    }
}
