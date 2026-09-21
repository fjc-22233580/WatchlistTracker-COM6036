using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.Models;
using src.Services;

namespace src.Pages
{
    /// <summary>
    /// Page model for the application home/dashboard page. Provides health checks
    /// for external services (Supabase and TMDB) and prepares a per-user dashboard
    /// with counts and a short list of currently-watching items when the user is authenticated.
    /// </summary>
    public class IndexModel : PageModel
    {
        private readonly WatchlistService _watchlistService;
        private readonly TmdbService _tmdbService;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexModel"/> class.
        /// </summary>
        /// <param name="watchlistService">Service used to read watchlist data.</param>
        /// <param name="tmdbService">Service used to call TMDB for images and metadata.</param>
        /// <param name="userManager">ASP.NET Core Identity user manager.</param>
        public IndexModel(
            WatchlistService watchlistService,
            TmdbService tmdbService,
            UserManager<IdentityUser> userManager)
        {
            _watchlistService = watchlistService;
            _tmdbService = tmdbService;
            _userManager = userManager;
        }

        /// <summary>
        /// True when the Supabase backend is reachable.
        /// </summary>
        public bool SupabaseAvailable { get; set; }

        /// <summary>
        /// True when the TMDB service is reachable.
        /// </summary>
        public bool TmdbAvailable { get; set; }

        /// <summary>
        /// Total number of movies in the current user's watchlist.
        /// </summary>
        public int TotalMovies { get; set; }

        /// <summary>
        /// Number of movies with status 'Watching'.
        /// </summary>
        public int WatchingMovies { get; set; }

        /// <summary>
        /// Number of movies with status 'Watched'.
        /// </summary>
        public int WatchedMovies { get; set; }

        /// <summary>
        /// Number of movies with status 'Planned'.
        /// </summary>
        public int PlannedMovies { get; set; }

        /// <summary>
        /// Short list of items the user is currently watching shown on the dashboard.
        /// </summary>
        public List<WatchlistItem> CurrentlyWatching { get; set; } = new();

        /// <summary>
        /// Helper used by the Razor page to convert a poster path into a full URL via the TMDB service.
        /// Returns null when no poster path is provided.
        /// </summary>
        public string? GetPosterUrl(string? posterPath)
        {
            return _tmdbService.GetPosterUrl(posterPath);
        }

        /// <summary>
        /// Handles GET requests to the index page. Performs parallel health checks for
        /// external services and, when the user is authenticated, loads dashboard counts
        /// and the currently-watching list for that user.
        /// </summary>
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
