using PromoEngine.Data;
using PromoEngine.DTOs;
using PromoEngine.Models;
using PromoEngine.Utils;
using Microsoft.EntityFrameworkCore;

namespace PromoEngine.Services;

public class SubmissionService
{
    private readonly ApplicationDbContext _context;

    public SubmissionService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<SubmissionResponseDto> ProcessSubmissionAsync(SubmissionRequestDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.FirstName) ||
            string.IsNullOrWhiteSpace(dto.LastName) ||
            string.IsNullOrWhiteSpace(dto.Email) ||
            string.IsNullOrWhiteSpace(dto.PromoCode))
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "Kérlek, töltsd ki az összes kötelező mezőt!"
            };
        }

        if (!dto.AcceptedPrivacyPolicy || !dto.AcceptedGameRules)
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "A beküldéshez el kell fogadnod a szabályzatokat!"
            };
        }

        if (!dto.Email.Contains("@") || !dto.Email.Contains("."))
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "Az e-mail cím formátuma érvénytelen."
            };
        }

        var code = await _context.PromoCodes
            .FirstOrDefaultAsync(p => p.Code == dto.PromoCode.ToUpper());

        if (code == null)
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "A megadott promóciós kód érvénytelen."
            };
        }

        if (code.IsUsed)
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "Ezt a promóciós kódot már felhasználták."
            };
        }

        var hashedFirst = Hasher.HashString(dto.FirstName);
        var hashedLast = Hasher.HashString(dto.LastName);
        var hashedEmail = Hasher.HashString(dto.Email);

        var submission = new Submission
        {
            HashedFirstName = hashedFirst,
            HashedLastName = hashedLast,
            HashedEmail = hashedEmail,
            PromoCodeId = code.Id,
            AcceptedPrivacyPolicy = dto.AcceptedPrivacyPolicy,
            AcceptedGameRules = dto.AcceptedGameRules,
            SubmittedAt = DateTime.UtcNow
        };

        _context.Submissions.Add(submission);

        code.IsUsed = true;
        code.UsedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        var (isWinner, prizeType) = await CheckIfWinnerAsync(submission);

        if (isWinner)
        {
            submission.IsWinner = true;
            submission.PrizeType = prizeType;
            await _context.SaveChangesAsync();
        }

        return new SubmissionResponseDto
        {
            Success = true,
            Message = isWinner
                ? "Nyertél, Bajnok! Megnyerted a napi nyereményt!"
                : "A kód sikeresen beküldve.",
            IsWinner = isWinner,
            PrizeType = prizeType
        };
    }

    private async Task<(bool IsWinner, string? PrizeType)> CheckIfWinnerAsync(Submission submission)
    {
        bool alreadyWinner = await _context.Submissions
            .AnyAsync(s => s.HashedEmail == submission.HashedEmail && s.IsWinner);

        if (alreadyWinner)
        {
            return (false, null);
        }

        var unclaimedTimestamps = await _context.WinningTimestamps
            .Where(w => !w.IsClaimed)
            .OrderBy(w => w.TargetTime)
            .ToListAsync();

        if (!unclaimedTimestamps.Any())
        {
            return (false, null);
        }

        var closest = unclaimedTimestamps
            .Select(w => new
            {
                Timestamp = w,
                Diff = Math.Abs((w.TargetTime - submission.SubmittedAt).TotalSeconds)
            })
            .OrderBy(x => x.Diff)
            .First();

        var winningTimestamp = closest.Timestamp;

        winningTimestamp.IsClaimed = true;
        winningTimestamp.WinnerSubmissionId = submission.Id;

        await _context.SaveChangesAsync();

        return (true, winningTimestamp.Type);
    }
}
