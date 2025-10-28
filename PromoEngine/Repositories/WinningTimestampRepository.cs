using Microsoft.EntityFrameworkCore;
using PromoEngine.Data;
using PromoEngine.Models;
using PromoEngine.Repositories.Interfaces;

namespace PromoEngine.Repositories;

public class WinningTimestampRepository : IWinningTimestampRepository
{
    private readonly ApplicationDbContext _context;
    public WinningTimestampRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<WinningTimestamp>> GetUnclaimedAsync()
        => await _context.WinningTimestamps
            .Where(w => !w.IsClaimed)
            .OrderBy(w => w.TargetTime)
            .ToListAsync();

    public async Task AddAsync(WinningTimestamp entity)
        => await _context.WinningTimestamps.AddAsync(entity);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();
}
