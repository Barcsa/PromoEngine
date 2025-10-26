namespace PromoEngine.DTOs;

public class SubmissionResponseDto
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public bool IsWinner { get; set; }
    public string? PrizeType { get; set; }
}
