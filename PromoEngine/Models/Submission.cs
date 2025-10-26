namespace PromoEngine.Models;

public class Submission
{
    public int Id { get; set; }

    public string HashedFirstName { get; set; } = string.Empty;
    public string HashedLastName { get; set; } = string.Empty;
    public string HashedEmail { get; set; } = string.Empty;

    public int PromoCodeId { get; set; }
    public PromoCode PromoCode { get; set; } = null!;

    public bool AcceptedPrivacyPolicy { get; set; }
    public bool AcceptedGameRules { get; set; }

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public bool IsWinner { get; set; } = false;
    public string? PrizeType { get; set; }
}
