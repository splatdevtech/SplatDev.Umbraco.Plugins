using UmbracoCms.Plugins.Surveys.Models;

namespace UmbracoCms.Plugins.Surveys.Services;

public record SurveyResultItem(int QuestionId, string QuestionText, string? OptionText, int Count);

public record SurveyResultSummary(int SurveyId, string Title, int TotalResponses, IReadOnlyList<SurveyResultItem> Results);

public interface ISurveysService
{
    Task<IReadOnlyList<Survey>> GetSurveysAsync(CancellationToken cancellationToken = default);
    Task<Survey?> GetSurveyAsync(int id, CancellationToken cancellationToken = default);
    Task<Survey> CreateSurveyAsync(Survey survey, CancellationToken cancellationToken = default);
    Task<Survey?> UpdateSurveyAsync(Survey survey, CancellationToken cancellationToken = default);
    Task<bool> DeleteSurveyAsync(int id, CancellationToken cancellationToken = default);
    Task<SurveyResponse> SubmitResponseAsync(int surveyId, string? respondentEmail, IEnumerable<(int QuestionId, string? AnswerText, int? SelectedOptionId)> answers, CancellationToken cancellationToken = default);
    Task<SurveyResultSummary> GetResultsAsync(int surveyId, CancellationToken cancellationToken = default);
}
