namespace PromoEngine.Models;

public class WinningTimestamp
{
    public int Id { get; set; }
    public DateTime TargetTime { get; set; }
    public string Type { get; set; } = "Daily";
    public bool IsClaimed { get; set; } = false;

    public int? WinnerSubmissionId { get; set; }
    public Submission? WinnerSubmission { get; set; }
}
