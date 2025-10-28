using Microsoft.EntityFrameworkCore;
using PromoEngine.Data;
using PromoEngine.Models;
using PromoEngine.Repositories.Interfaces;

namespace PromoEngine.Repositories;

public class SubmissionRepository : ISubmissionRepository
{
    private readonly ApplicationDbContext _context;
    public SubmissionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Submission entity)
        => await _context.Submissions.AddAsync(entity);

    public async Task SaveChangesAsync()
        => await _context.SaveChangesAsync();

    public async Task<bool> HasAlreadyWonAsync(string hashedEmail)
        => await _context.Submissions.AnyAsync(s => s.HashedEmail == hashedEmail && s.IsWinner);
}
