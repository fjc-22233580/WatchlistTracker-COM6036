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
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly WatchlistService _watchlistService;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly TmdbService _tmdbService;

        public CreateModel(
            WatchlistService watchlistService,
            UserManager<IdentityUser> userManager,
            TmdbService tmdbService)
        {
            _watchlistService = watchlistService;
            _userManager = userManager;
            _tmdbService = tmdbService;
        }

        [BindProperty]
        public WatchlistItemInput Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        public List<TmdbMovieResult> SearchResults { get; set; } = new();

        [BindProperty]
        public int? SelectedTmdbId { get; set; }

        [BindProperty]
        public bool IsManualEntry { get; set; }

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

            if (SelectedTmdbId.HasValue)
            {
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
            }

            var movie = await _tmdbService.GetMovieAsync(SelectedTmdbId.Value);

            if (movie == null)
            {
                ModelState.AddModelError(string.Empty, "The selected movie could not be retrieved.");

                return Page();
            }

            var item = new WatchlistItem
            {
                Title = Input.Title,
                Status = Input.Status,
                Rating = Input.Rating,
                UserId = userId,

                TmdbId = movie.Id,
                PosterPath = movie.PosterPath,
                Overview = movie.Overview,
                VoteAverage = movie.VoteAverage
            };

            await _watchlistService.AddAsync(item);

            // Set response message for next request
            TempData["SuccessMessage"] = "Movie added to your watchlist.";

            return RedirectToPage("/Watchlist/Create");
        }

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