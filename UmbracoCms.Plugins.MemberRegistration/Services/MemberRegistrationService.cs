using Microsoft.Extensions.Logging;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using UmbracoCms.Plugins.MemberRegistration.Models;

namespace UmbracoCms.Plugins.MemberRegistration.Services;

public class MemberRegistrationService : IMemberRegistrationService
{
    private readonly IMemberService _memberService;
    private readonly IMemberTypeService _memberTypeService;
    private readonly MemberRegistrationDbContext _db;
    private readonly ILogger<MemberRegistrationService> _logger;

    public MemberRegistrationService(
        IMemberService memberService,
        IMemberTypeService memberTypeService,
        MemberRegistrationDbContext db,
        ILogger<MemberRegistrationService> logger)
    {
        _memberService = memberService;
        _memberTypeService = memberTypeService;
        _db = db;
        _logger = logger;
    }

    public async Task<RegistrationResult> RegisterAsync(
        string name, string email, string username, string password,
        string memberTypeAlias = "Member")
    {
        try
        {
            var existing = _memberService.GetByEmail(email);
            if (existing is not null)
                return new RegistrationResult(false, -1, "An account with this email already exists.");

            var existingByUsername = _memberService.GetByUsername(username);
            if (existingByUsername is not null)
                return new RegistrationResult(false, -1, "Username is already taken.");

            // Ensure member type exists
            var memberType = _memberTypeService.GetAll().FirstOrDefault(t => t.Alias == memberTypeAlias)
                          ?? _memberTypeService.GetAll().First();

            var member = _memberService.CreateMember(username, email, name, memberType.Alias);
            member.IsApproved = false; // Require email verification
            member.IsLockedOut = false;

            _memberService.Save(member);
            _memberService.SavePassword(member, password);

            // Create verification token
            var token = new RegistrationToken
            {
                MemberId = member.Id,
                Token = Guid.NewGuid().ToString("N"),
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IsUsed = false
            };
            _db.RegistrationTokens.Add(token);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Member {Email} registered. Verification token created.", email);
            return new RegistrationResult(true, member.Id, null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error registering member {Email}.", email);
            return new RegistrationResult(false, -1, "An error occurred during registration.");
        }
    }

    public async Task<bool> VerifyEmailAsync(string email, string tokenValue)
    {
        try
        {
            var member = _memberService.GetByEmail(email);
            if (member is null) return false;

            var token = _db.RegistrationTokens
                .Where(t => t.MemberId == member.Id && t.Token == tokenValue && !t.IsUsed)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefault();

            if (token is null || token.ExpiresAt < DateTime.UtcNow)
                return false;

            token.IsUsed = true;
            member.IsApproved = true;
            member.IsLockedOut = false;

            _memberService.Save(member);
            await _db.SaveChangesAsync();

            _logger.LogInformation("Email verified for member {Email}.", email);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error verifying email for {Email}.", email);
            return false;
        }
    }

    public async Task<bool> ApproveAsync(int memberId)
    {
        try
        {
            var member = _memberService.GetById(memberId);
            if (member is null) return false;

            member.IsApproved = true;
            member.IsLockedOut = false;
            _memberService.Save(member);

            _logger.LogInformation("Member {MemberId} approved.", memberId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error approving member {MemberId}.", memberId);
            return false;
        }
    }

    public async Task<IEnumerable<IMember>> GetPendingAsync()
    {
        try
        {
            var allMembers = _memberService.GetAllMembers();
            return allMembers.Where(m => !m.IsApproved);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving pending members.");
            return Enumerable.Empty<IMember>();
        }
    }
}
