using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.Models;
using src.Models.InputModels;
using src.Models.Tmdb;
using src.Services;

namespace src.Pages.Watchlist
{
    /// <summary>
    /// Page model for creating watchlist items. This model handles TMDB searches,
    /// selection of TMDB results, and creating a watchlist item for the current user.
    /// Requires an authenticated user.
    /// </summary>
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly WatchlistService _watchlistService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TmdbService _tmdbService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CreateModel"/> class.
        /// </summary>
        /// <param name="watchlistService">Service used to manage watchlist persistence.</param>
        /// <param name="userManager">ASP.NET Core Identity user manager.</param>
        /// <param name="tmdbService">Service used to query TMDB for movie data.</param>
        public CreateModel(
            WatchlistService watchlistService,
            UserManager<IdentityUser> userManager,
            TmdbService tmdbService)
        {
            _watchlistService = watchlistService;
            _userManager = userManager;
            _tmdbService = tmdbService;
        }

        /// <summary>
        /// Input model bound for create form POSTs. Validated by data annotations.
        /// </summary>
        [BindProperty]
        public WatchlistItemInput Input { get; set; } = new();

        /// <summary>
        /// Optional search term bound on GET to perform TMDB searches.
        /// Supports GET binding so the term persists between requests.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        /// <summary>
        /// Results returned from TMDB when a search is performed. Limited to 10 items for display.
        /// </summary>
        public List<TmdbMovieResult> SearchResults { get; set; } = new();

        /// <summary>
        /// If a user selects a TMDB result this property holds the chosen TMDB id.
        /// </summary>
        [BindProperty]
        public int? SelectedTmdbId { get; set; }

        /// <summary>
        /// Indicates the user has chosen to enter the movie manually instead of selecting a TMDB result.
        /// </summary>
        [BindProperty]
        public bool IsManualEntry { get; set; }

        /// <summary>
        /// Handles GET requests to optionally perform a TMDB search when a SearchTerm is present.
        /// Populates <see cref="SearchResults"/> with up to 10 results.
        /// </summary>
        public async Task OnGetAsync()
        {
            if (string.IsNullOrWhiteSpace(SearchTerm))
            {
                return;
            }

            var response = await _tmdbService.SearchMoviesAsync(SearchTerm);

            SearchResults = response?.Results
                .Take(10)
                .ToList() ?? new();
        }

        /// <summary>
        /// Handles POST requests to create a new watchlist item for the current user.
        /// Supports either manual entry or a TMDB-backed movie selection.
        /// </summary>
        /// <returns>
        /// Page when validation fails; redirects back to the create page on success.
        /// </returns>
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            WatchlistItem item;

            // Manual entry
            if (IsManualEntry)
            {
                item = new WatchlistItem
                {
                    Title = Input.Title,
                    Status = Input.Status,
                    Rating = Input.Rating,
                    UserId = userId,

                    TmdbId = null,
                    PosterPath = null,
                    Overview = null,
                    VoteAverage = null
                };
            }
            // TMDB entry
            else
            {
                if (!SelectedTmdbId.HasValue)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Please select a movie from TMDB.");

                    return Page();
                }

                var exists = await _watchlistService.ExistsAsync(
                    userId,
                    SelectedTmdbId.Value);

                if (exists)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "This movie is already in your watchlist.");

                    return Page();
                }

                var movie = await _tmdbService.GetMovieAsync(
                    SelectedTmdbId.Value);

                if (movie == null)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "The selected movie could not be retrieved.");

                    return Page();
                }

                item = new WatchlistItem
                {
                    Title = movie.Title,
                    Status = Input.Status,
                    Rating = Input.Rating,
                    UserId = userId,

                    TmdbId = movie.Id,
                    PosterPath = movie.PosterPath,
                    Overview = movie.Overview,
                    VoteAverage = movie.VoteAverage
                };
            }

            await _watchlistService.AddAsync(item);

            TempData["SuccessMessage"] =
                "Movie added to your watchlist.";

            return RedirectToPage("/Watchlist/Create");
        }

        /// <summary>
        /// Handler used when a user selects a movie from TMDB search results. It reloads
        /// the search results and pre-fills the input model with the selected movie's title.
        /// </summary>
        public async Task<IActionResult> OnGetSelectAsync(int tmdbId, string searchTerm)
        {
            var response = await _tmdbService.SearchMoviesAsync(searchTerm);

            var movie = response?.Results
                .FirstOrDefault(movie => movie.Id == tmdbId);

            if (movie == null)
            {
                return NotFound();
            }

            SearchTerm = searchTerm;

            SearchResults = response!.Results
                .Take(10)
                .ToList();

            Input = new WatchlistItemInput
            {
                Title = movie.Title
            };

            SelectedTmdbId = movie.Id;

            return Page();
        }
    }
}