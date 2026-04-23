using Microsoft.EntityFrameworkCore;
using SplatDev.Umbraco.Plugins.Surveys.Models;

namespace SplatDev.Umbraco.Plugins.Surveys.Services;

public class SurveysService : ISurveysService
{
    private readonly SurveysDbContext _db;

    public SurveysService(SurveysDbContext db)
    {
        _db = db;
    }

    public async Task<IReadOnlyList<Survey>> GetSurveysAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Surveys
            .Include(s => s.Questions)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<Survey?> GetSurveyAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.Surveys
            .Include(s => s.Questions.OrderBy(q => q.SortOrder))
                .ThenInclude(q => q.Options.OrderBy(o => o.SortOrder))
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Survey> CreateSurveyAsync(Survey survey, CancellationToken cancellationToken = default)
    {
        survey.CreatedAt = DateTime.UtcNow;
        _db.Surveys.Add(survey);
        await _db.SaveChangesAsync(cancellationToken);
        return survey;
    }

    public async Task<Survey?> UpdateSurveyAsync(Survey survey, CancellationToken cancellationToken = default)
    {
        var existing = await _db.Surveys.FindAsync(new object[] { survey.Id }, cancellationToken);
        if (existing is null) return null;

        existing.Title = survey.Title;
        existing.Description = survey.Description;
        existing.IsPublished = survey.IsPublished;
        existing.ExpiresAt = survey.ExpiresAt;

        await _db.SaveChangesAsync(cancellationToken);
        return existing;
    }

    public async Task<bool> DeleteSurveyAsync(int id, CancellationToken cancellationToken = default)
    {
        var survey = await _db.Surveys.FindAsync(new object[] { id }, cancellationToken);
        if (survey is null) return false;

        _db.Surveys.Remove(survey);
        await _db.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<SurveyResponse> SubmitResponseAsync(
        int surveyId,
        string? respondentEmail,
        IEnumerable<(int QuestionId, string? AnswerText, int? SelectedOptionId)> answers,
        CancellationToken cancellationToken = default)
    {
        var response = new SurveyResponse
        {
            SurveyId = surveyId,
            RespondentEmail = respondentEmail,
            SubmittedAt = DateTime.UtcNow
        };

        foreach (var (questionId, answerText, selectedOptionId) in answers)
        {
            response.Answers.Add(new SurveyAnswer
            {
                QuestionId = questionId,
                AnswerText = answerText,
                SelectedOptionId = selectedOptionId
            });
        }

        _db.SurveyResponses.Add(response);
        await _db.SaveChangesAsync(cancellationToken);
        return response;
    }

    public async Task<SurveyResultSummary> GetResultsAsync(int surveyId, CancellationToken cancellationToken = default)
    {
        var survey = await _db.Surveys
            .Include(s => s.Questions.OrderBy(q => q.SortOrder))
                .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(s => s.Id == surveyId, cancellationToken)
            ?? throw new InvalidOperationException($"Survey {surveyId} not found.");

        var totalResponses = await _db.SurveyResponses
            .CountAsync(r => r.SurveyId == surveyId, cancellationToken);

        var results = new List<SurveyResultItem>();

        foreach (var question in survey.Questions)
        {
            if (question.QuestionType == QuestionType.MultipleChoice || question.QuestionType == QuestionType.Rating)
            {
                // Count answers per option
                var counts = await _db.SurveyAnswers
                    .Where(a => a.QuestionId == question.Id && a.SelectedOptionId != null)
                    .GroupBy(a => a.SelectedOptionId)
                    .Select(g => new { OptionId = g.Key, Count = g.Count() })
                    .ToListAsync(cancellationToken);

                foreach (var option in question.Options)
                {
                    var count = counts.FirstOrDefault(c => c.OptionId == option.Id)?.Count ?? 0;
                    results.Add(new SurveyResultItem(question.Id, question.QuestionText, option.OptionText, count));
                }
            }
            else
            {
                // FreeText: just count answers
                var count = await _db.SurveyAnswers
                    .CountAsync(a => a.QuestionId == question.Id && a.AnswerText != null, cancellationToken);
                results.Add(new SurveyResultItem(question.Id, question.QuestionText, null, count));
            }
        }

        return new SurveyResultSummary(surveyId, survey.Title, totalResponses, results);
    }
}
