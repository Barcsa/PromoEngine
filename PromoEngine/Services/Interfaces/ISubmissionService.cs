using PromoEngine.DTOs;

namespace PromoEngine.Services.Interfaces
{
    public interface ISubmissionService
    {
        Task<SubmissionResponseDto> ProcessSubmissionAsync(SubmissionRequestDto dto);
    }
}
