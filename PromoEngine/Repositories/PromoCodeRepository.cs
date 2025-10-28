using Microsoft.EntityFrameworkCore;
using PromoEngine.Data;
using PromoEngine.Models;
using PromoEngine.Repositories.Interfaces;

namespace PromoEngine.Repositories;

public class PromoCodeRepository : IPromoCodeRepository
{
    private readonly ApplicationDbContext _context;
    public PromoCodeRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PromoCode?> GetByCodeAsync(string code)
        => await _context.PromoCodes.FirstOrDefaultAsync(p => p.Code == code.ToUpper());

    public async Task AddAsync(PromoCode entity)
        => await _context.PromoCodes.AddAsync(entity);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
