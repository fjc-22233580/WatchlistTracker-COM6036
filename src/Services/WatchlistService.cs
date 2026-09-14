using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Models;
using src.Models.Enums;

namespace src.Services;

public class WatchlistService
{
    private readonly ApplicationDbContext _context;

    public WatchlistService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<WatchlistItem?> GetByIdAsync(int id, string userId)
    {
        return await _context.WatchlistItems
            .FirstOrDefaultAsync(item =>
                item.Id == id &&
                item.UserId == userId);
    }

    public async Task UpdateAsync(WatchlistItem item)
    {
        _context.WatchlistItems.Update(item);
        await _context.SaveChangesAsync();
    }


    public async Task<List<WatchlistItem>> GetUserWatchlistAsync(string userId, string? searchTerm = null, WatchStatus? status = null)
    {
        var query = _context.WatchlistItems
            .Where(item => item.UserId == userId);

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(item =>
                item.Title.Contains(searchTerm));
        }

        if (status.HasValue)
        {
            query = query.Where(item =>
                item.Status == status.Value);
        }

        return await query.ToListAsync();
    }

    public async Task<(int Total, int Watching, int Watched, int Planned)> GetDashboardCountsAsync(string userId)
    {
        var items = await _context.WatchlistItems
            .Where(item => item.UserId == userId)
            .ToListAsync();

        return (
            items.Count,
            items.Count(item => item.Status == WatchStatus.Watching),
            items.Count(item => item.Status == WatchStatus.Watched),
            items.Count(item => item.Status == WatchStatus.Planned)
        );
    }

    public async Task AddAsync(WatchlistItem item)
    {
        _context.WatchlistItems.Add(item);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(WatchlistItem item)
    {
        _context.WatchlistItems.Remove(item);
        await _context.SaveChangesAsync();
    }

    public async Task<List<WatchlistItem>> GetCurrentlyWatchingAsync(string userId, int count = 3)
    {
        return await _context.WatchlistItems
            .Where(item => item.UserId == userId && item.Status == WatchStatus.Watching)
            .Take(count)
            .ToListAsync();
    }


    public async Task<bool> ExistsAsync(string userId, int tmdbId)
    {
        return await _context.WatchlistItems
            .AnyAsync(item => item.UserId == userId && item.TmdbId == tmdbId);
    }
}