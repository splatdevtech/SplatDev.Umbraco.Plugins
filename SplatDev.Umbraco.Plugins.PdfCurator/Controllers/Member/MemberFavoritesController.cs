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
    private readonly IDbContextFactory<MemberDbContext> _memberDbFactory;
    private readonly IDbContextFactory<CuratorDbContext> _curatorDbFactory;
    private readonly IMemberManager _memberManager;

    public MemberFavoritesController(
        IDbContextFactory<MemberDbContext> memberDbFactory,
        IDbContextFactory<CuratorDbContext> curatorDbFactory,
        IMemberManager memberManager)
    {
        _memberDbFactory = memberDbFactory;
        _curatorDbFactory = curatorDbFactory;
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

        await using var memberDb = await _memberDbFactory.CreateDbContextAsync(ct);
        var favorites = await memberDb.Favorites
            .Where(favorite => favorite.MemberKey == memberKey.Value)
            .OrderByDescending(favorite => favorite.CreatedAt)
            .Select(favorite => new { favorite.BookId, favorite.CreatedAt })
            .ToListAsync(ct);

        if (favorites.Count == 0)
        {
            return Ok(Array.Empty<object>());
        }

        var bookIds = favorites.Select(favorite => favorite.BookId).ToArray();
        await using var curatorDb = await _curatorDbFactory.CreateDbContextAsync(ct);
        var books = await curatorDb.Books
            .Where(book => bookIds.Contains(book.Id) && book.Status == BookStatus.Filed)
            .Select(book => new { book.Id, book.Title, book.Author })
            .ToDictionaryAsync(book => book.Id, ct);

        var result = favorites
            .Where(favorite => books.ContainsKey(favorite.BookId))
            .Select(favorite =>
            {
                var book = books[favorite.BookId];
                return new
                {
                    favorite.BookId,
                    bookTitle = book.Title,
                    bookAuthor = book.Author,
                    favorite.CreatedAt,
                };
            });

        return Ok(result);
    }

    [HttpPut("{bookId:int}")]
    public async Task<IActionResult> AddFavorite(int bookId, CancellationToken ct = default)
    {
        var memberKey = await GetMemberKeyAsync();
        if (memberKey is null)
        {
            return Unauthorized();
        }

        await using var db = await _memberDbFactory.CreateDbContextAsync(ct);
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

        await using var db = await _memberDbFactory.CreateDbContextAsync(ct);
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
