using SplatDev.Umbraco.Plugins.QuickPoll.Models;

namespace SplatDev.Umbraco.Plugins.QuickPoll.Services;

public record PollResults(int PollId, string Question, int TotalVotes, IReadOnlyList<PollOptionResult> Options);

public record PollOptionResult(int OptionId, string OptionText, int VoteCount, double Percentage);

public interface IQuickPollService
{
    Task<Poll?> GetActivePollAsync(CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Poll>> GetAllPollsAsync(CancellationToken cancellationToken = default);
    Task<Poll?> GetPollAsync(int id, CancellationToken cancellationToken = default);
    Task<Poll> CreatePollAsync(Poll poll, CancellationToken cancellationToken = default);
    Task<bool> DeletePollAsync(int id, CancellationToken cancellationToken = default);
    Task<(bool Success, string Message)> VoteAsync(int pollId, int optionId, string voterIp, CancellationToken cancellationToken = default);
    Task<PollResults?> GetResultsAsync(int pollId, CancellationToken cancellationToken = default);
}
