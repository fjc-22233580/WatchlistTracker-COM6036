using Microsoft.EntityFrameworkCore;
using src.Data;
using src.Models;
using src.Models.Enums;

namespace src.Services;

/// <summary>
/// Service for managing watchlist items in persistent storage.
/// Provides methods for retrieving, adding, updating, and deleting watchlist items.
/// </summary>
public class WatchlistService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="WatchlistService"/> class.
    /// </summary>
    /// <param name="context">The application database context used for data access.</param>
    public WatchlistService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Checks if the database is accessible and responding.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if the database is reachable; otherwise, <c>false</c>.</returns>
    public async Task<bool> IsAliveAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(
                TimeSpan.FromSeconds(3));

            return await _context.Database
                .CanConnectAsync(cts.Token);
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Retrieves a watchlist item by its identifier and user ID.
    /// </summary>
    /// <param name="id">The ID of the watchlist item to retrieve.</param>
    /// <param name="userId">The user ID to verify ownership of the watchlist item.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the watchlist item, or <c>null</c> if not found.</returns>
    public async Task<WatchlistItem?> GetByIdAsync(int id, string userId)
    {
        return await _context.WatchlistItems
            .FirstOrDefaultAsync(item =>
                item.Id == id &&
                item.UserId == userId);
    }

    /// <summary>
    /// Updates an existing watchlist item in the database.
    /// </summary>
    /// <param name="item">The watchlist item to update.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task UpdateAsync(WatchlistItem item)
    {
        _context.WatchlistItems.Update(item);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves all watchlist items for a user, with optional filtering by search term and watch status.
    /// </summary>
    /// <param name="userId">The user ID to filter watchlist items by.</param>
    /// <param name="searchTerm">Optional search term to filter items by title. Case-sensitive partial matching.</param>
    /// <param name="status">Optional watch status to filter items by.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of matching watchlist items.</returns>
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

    /// <summary>
    /// Retrieves dashboard statistics for a user's watchlist.
    /// </summary>
    /// <param name="userId">The user ID to retrieve dashboard counts for.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a tuple with:
    /// <list type="bullet">
    /// <item><description><c>Total</c> - Total number of items in the watchlist.</description></item>
    /// <item><description><c>Watching</c> - Number of items currently being watched.</description></item>
    /// <item><description><c>Watched</c> - Number of items that have been watched.</description></item>
    /// <item><description><c>Planned</c> - Number of items planned to watch.</description></item>
    /// </list>
    /// </returns>
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

    /// <summary>
    /// Adds a new watchlist item to the database.
    /// </summary>
    /// <param name="item">The watchlist item to add.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task AddAsync(WatchlistItem item)
    {
        _context.WatchlistItems.Add(item);
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Deletes the specified watchlist item from persistent storage.
    /// </summary>
    /// <param name="item">The watchlist item to delete.</param>
    public async Task DeleteAsync(WatchlistItem item)
    {
        // Remove the item from the tracked watchlist collection.
        _context.WatchlistItems.Remove(item);

        // Persist the deletion to the database.
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Retrieves currently watched items for a user.
    /// </summary>
    /// <param name="userId">The user ID to retrieve currently watched items for.</param>
    /// <param name="count">The maximum number of items to retrieve. Defaults to 3.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a list of items with <see cref="WatchStatus.Watching"/> status, limited to the specified count.</returns>
    public async Task<List<WatchlistItem>> GetCurrentlyWatchingAsync(string userId, int count = 3)
    {
        return await _context.WatchlistItems
            .Where(item => item.UserId == userId && item.Status == WatchStatus.Watching)
            .Take(count)
            .ToListAsync();
    }

    /// <summary>
    /// Checks if a watchlist item exists for a specific user and TMDB movie ID.
    /// </summary>
    /// <param name="userId">The user ID to check for.</param>
    /// <param name="tmdbId">The TMDB ID of the movie to check for.</param>
    /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if an item exists; otherwise, <c>false</c>.</returns>
    public async Task<bool> ExistsAsync(string userId, int tmdbId)
    {
        return await _context.WatchlistItems
            .AnyAsync(item => item.UserId == userId && item.TmdbId == tmdbId);
    }
}