using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.Models;
using src.Services;

namespace src.Pages
{
    public class IndexModel : PageModel
    {

        private readonly WatchlistService _watchlistService;
        private readonly TmdbService _tmdbService;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(
            WatchlistService watchlistService,
            TmdbService tmdbService,
            UserManager<IdentityUser> userManager)
        {
            _watchlistService = watchlistService;
            _tmdbService = tmdbService;
            _userManager = userManager;
        }

        public bool SupabaseAvailable { get; set; }

        public bool TmdbAvailable { get; set; }

        public int TotalMovies { get; set; }

        public int WatchingMovies { get; set; }

        public int WatchedMovies { get; set; }

        public int PlannedMovies { get; set; }

        public List<WatchlistItem> CurrentlyWatching { get; set; } = new();


        public string? GetPosterUrl(string? posterPath)
        {
            return _tmdbService.GetPosterUrl(posterPath);
        }

        public async Task OnGetAsync()
        {
            var supabaseCheck = _watchlistService.IsAliveAsync();
            var tmdbCheck = _tmdbService.IsAliveAsync();

            await Task.WhenAll(supabaseCheck, tmdbCheck);

            SupabaseAvailable = await supabaseCheck;
            TmdbAvailable = await tmdbCheck;

            if (User.Identity?.IsAuthenticated == false)
            {
                return;
            }

            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return;
            }

            var counts = await _watchlistService.GetDashboardCountsAsync(userId);

            TotalMovies = counts.Total;
            WatchingMovies = counts.Watching;
            WatchedMovies = counts.Watched;
            PlannedMovies = counts.Planned;

            CurrentlyWatching = await _watchlistService.GetCurrentlyWatchingAsync(userId);

        }
    }
}
