using Microsoft.EntityFrameworkCore;
using PromoEngine.Data;
using PromoEngine.DTOs;
using PromoEngine.Models;
using PromoEngine.Utils;
using System.Text.RegularExpressions;

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
        if (string.IsNullOrWhiteSpace(dto.FirstName))
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "Kérlek, add meg a keresztnevedet."
            };
        }

        if (string.IsNullOrWhiteSpace(dto.LastName))
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "Kérlek, add meg a vezetéknevedet."
            };
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "Add meg az e-mail címedet."
            };
        }

        if (string.IsNullOrWhiteSpace(dto.PhoneNumber))
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "Add meg a telefonszámodat."
            };
        }

        if (string.IsNullOrWhiteSpace(dto.PromoCode))
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "Add meg a promóciós kódot."
            };
        }

        if (!Regex.IsMatch(dto.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "Az e-mail cím formátuma nem megfelelő."
            };
        }

        if (!Regex.IsMatch(dto.PhoneNumber, @"^\+?[0-9\s\-]{7,15}$"))
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "A megadott telefonszám formátuma nem érvényes."
            };
        }

        if (!dto.AcceptedGameRules)
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "A játékszabályzat elfogadása kötelező."
            };
        }

        if (!dto.AcceptedPrivacyPolicy)
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "Az adatvédelmi szabályzat elfogadása kötelező."
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
        var hashedPhone = Hasher.HashString(dto.PhoneNumber);

        var submission = new Submission
        {
            HashedFirstName = hashedFirst,
            HashedLastName = hashedLast,
            HashedEmail = hashedEmail,
            HashedPhoneNumber = hashedPhone,
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
