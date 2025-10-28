using PromoEngine.Models;

namespace PromoEngine.Repositories.Interfaces;

public interface IPromoCodeRepository : IRepository<PromoCode>
{
    Task<PromoCode?> GetByCodeAsync(string code);
}
