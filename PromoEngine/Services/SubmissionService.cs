using PromoEngine.DTOs;
using PromoEngine.Models;
using PromoEngine.Repositories.Interfaces;
using PromoEngine.Services.Interfaces;
using PromoEngine.Utils;
using System.Text.RegularExpressions;

namespace PromoEngine.Services;

public class SubmissionService : ISubmissionService
{
    private readonly IPromoCodeRepository _promoRepo;
    private readonly ISubmissionRepository _submissionRepo;
    private readonly IWinningTimestampRepository _timestampRepo;

    public SubmissionService(
        IPromoCodeRepository promoRepo,
        ISubmissionRepository submissionRepo,
        IWinningTimestampRepository timestampRepo)
    {
        _promoRepo = promoRepo;
        _submissionRepo = submissionRepo;
        _timestampRepo = timestampRepo;
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

        var code = await _promoRepo.GetByCodeAsync(dto.PromoCode);
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

        await _submissionRepo.AddAsync(submission);

        code.IsUsed = true;
        code.UsedAt = DateTime.UtcNow;

        await _promoRepo.SaveChangesAsync();
        await _submissionRepo.SaveChangesAsync();

        var (isWinner, prizeType) = await CheckIfWinnerAsync(submission);

        if (isWinner)
        {
            submission.IsWinner = true;
            submission.PrizeType = prizeType;
            await _submissionRepo.SaveChangesAsync();
        }

        return new SubmissionResponseDto
        {
            Success = true,
            Message = isWinner
                ? "Nyertél, Bajnok! Megnyerted a napi nyereményt!"
                : "Most nem volt szerencséd, Bajnok! A nagy győzelemhez sok küzdelem kell. Próbáld újra!",
            IsWinner = isWinner,
            PrizeType = prizeType
        };
    }

    private async Task<(bool IsWinner, string? PrizeType)> CheckIfWinnerAsync(Submission submission)
    {
        bool alreadyWinner = await _submissionRepo.HasAlreadyWonAsync(submission.HashedEmail);
        if (alreadyWinner)
            return (false, null);

        var unclaimedTimestamps = await _timestampRepo.GetUnclaimedAsync();
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

        await _timestampRepo.SaveChangesAsync();

        return (true, winningTimestamp.Type);
    }
}
