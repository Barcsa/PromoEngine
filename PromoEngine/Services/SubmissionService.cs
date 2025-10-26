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
                Message = "All fields are required."
            };
        }

        if (!dto.AcceptedPrivacyPolicy || !dto.AcceptedGameRules)
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "You must accept the required policies."
            };
        }

        if (!dto.Email.Contains("@") || !dto.Email.Contains("."))
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "Invalid email format."
            };
        }

        var code = await _context.PromoCodes
            .FirstOrDefaultAsync(p => p.Code == dto.PromoCode.ToUpper());

        if (code == null)
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "Invalid promo code."
            };
        }

        if (code.IsUsed)
        {
            return new SubmissionResponseDto
            {
                Success = false,
                Message = "This promo code has already been used."
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

        return new SubmissionResponseDto
        {
            Success = true,
            Message = "Submission successful.",
            IsWinner = false
        };
    }
}
