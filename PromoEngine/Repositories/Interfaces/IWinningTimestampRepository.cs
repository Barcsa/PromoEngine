using PromoEngine.Models;

namespace PromoEngine.Repositories.Interfaces;

public interface IWinningTimestampRepository : IRepository<WinningTimestamp>
{
    Task<List<WinningTimestamp>> GetUnclaimedAsync();
}
