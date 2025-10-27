namespace PromoEngine.DTOs;

public class SubmissionRequestDto
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PromoCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public bool AcceptedPrivacyPolicy { get; set; }
    public bool AcceptedGameRules { get; set; }
}
