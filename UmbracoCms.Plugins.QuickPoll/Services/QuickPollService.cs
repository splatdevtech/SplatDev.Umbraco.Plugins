using Microsoft.EntityFrameworkCore;
using UmbracoCms.Plugins.QuickPoll.Models;

namespace UmbracoCms.Plugins.QuickPoll.Services;

public class QuickPollService : IQuickPollService
{
    private readonly QuickPollDbContext _db;

    public QuickPollService(QuickPollDbContext db)
    {
        _db = db;
    }

    public async Task<Poll?> GetActivePollAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _db.Polls
            .Include(p => p.Options.OrderBy(o => o.SortOrder))
            .Where(p => p.IsActive && (p.ExpiresAt == null || p.ExpiresAt > now))
            .OrderByDescending(p => p.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Poll>> GetAllPollsAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Polls
            .Include(p => p.Options.OrderBy(o => o.SortOrder))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Poll?> GetPollAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.Polls
            .Include(p => p.Options.OrderBy(o => o.SortOrder))
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Poll> CreatePollAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        poll.CreatedAt = DateTime.UtcNow;
        _db.Polls.Add(poll);
        await _db.SaveChangesAsync(cancellationToken);
        return poll;
    }

    public async Task<bool> DeletePollAsync(int id, CancellationToken cancellationToken = default)
    {
        var poll = await _db.Polls.FindAsync(new object[] { id }, cancellationToken);
        if (poll is null) return false;
        _db.Polls.Remove(poll);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(bool Success, string Message)> VoteAsync(int pollId, int optionId, string voterIp, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var poll = await _db.Polls
            .Include(p => p.Options)
            .FirstOrDefaultAsync(p => p.Id == pollId, cancellationToken);

        if (poll is null) return (false, "Poll not found.");
        if (!poll.IsActive) return (false, "Poll is not active.");
        if (poll.ExpiresAt.HasValue && poll.ExpiresAt.Value < now) return (false, "Poll has expired.");

        var option = poll.Options.FirstOrDefault(o => o.Id == optionId);
        if (option is null) return (false, "Option not found.");

        // Check if voter already voted
        var alreadyVoted = await _db.PollVotes
            .AnyAsync(v => v.PollId == pollId && v.VoterIp == voterIp, cancellationToken);
        if (alreadyVoted) return (false, "You have already voted in this poll.");

        // Record vote and increment counter
        _db.PollVotes.Add(new PollVote
        {
            PollId = pollId,
            OptionId = optionId,
            VoterIp = voterIp,
            VotedAt = now
        });

        option.VoteCount++;
        await _db.SaveChangesAsync(cancellationToken);
        return (true, "Vote recorded successfully.");
    }

    public async Task<PollResults?> GetResultsAsync(int pollId, CancellationToken cancellationToken = default)
    {
        var poll = await _db.Polls
            .Include(p => p.Options.OrderBy(o => o.SortOrder))
            .FirstOrDefaultAsync(p => p.Id == pollId, cancellationToken);

        if (poll is null) return null;

        var totalVotes = poll.Options.Sum(o => o.VoteCount);

        var optionResults = poll.Options
            .Select(o => new PollOptionResult(
                o.Id,
                o.OptionText,
                o.VoteCount,
                totalVotes > 0 ? Math.Round((double)o.VoteCount / totalVotes * 100, 1) : 0.0))
            .ToList();

        return new PollResults(poll.Id, poll.Question, totalVotes, optionResults);
    }
}
