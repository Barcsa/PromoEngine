using PromoEngine.Models;

namespace PromoEngine.Repositories.Interfaces;

public interface ISubmissionRepository : IRepository<Submission>
{
    Task<bool> HasAlreadyWonAsync(string hashedEmail);
}
