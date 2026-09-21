using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.Models.InputModels;
using src.Services;

namespace src.Pages.Watchlist
{
    /// <summary>
    /// Page model responsible for editing an existing watchlist item.
    /// Ensures the current user owns the item and only allows title edits
    /// for manually-entered items (not TMDB-linked items).
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly WatchlistService _watchlistService;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="EditModel"/> class.
        /// </summary>
        /// <param name="watchlistService">Service for accessing and updating watchlist items.</param>
        /// <param name="userManager">ASP.NET Core Identity user manager.</param>
        public EditModel(
            WatchlistService watchlistService,
            UserManager<IdentityUser> userManager)
        {
            _watchlistService = watchlistService;
            _userManager = userManager;
        }

        /// <summary>
        /// True when the item is linked to TMDB and therefore its title should not be editable.
        /// </summary>
        public bool IsTmdbMovie { get; set; }

        /// <summary>
        /// Bound input model for editing. Validated via data annotations on POST.
        /// </summary>
        [BindProperty]
        public WatchlistItemInput Input { get; set; } = new();

        /// <summary>
        /// Optional return URL preserved across requests so the user can be redirected back.
        /// Bound on GET so the value can be round-tripped.
        /// </summary>
        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

        /// <summary>
        /// Handles GET requests to load the edit form for the specified watchlist item.
        /// Verifies ownership and populates the input model with the current values.
        /// </summary>
        /// <param name="id">The identifier of the watchlist item to edit.</param>
        public async Task<IActionResult> OnGetAsync(int id)
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            var item = await _watchlistService.GetByIdAsync(id, userId);

            if (item == null)
            {
                return NotFound();
            }

            IsTmdbMovie = item.TmdbId.HasValue;

            Input = new WatchlistItemInput
            {
                Title = item.Title,
                Status = item.Status,
                Rating = item.Rating
            };

            return Page();
        }

        /// <summary>
        /// Handles POST requests to update an existing watchlist item. Validates the
        /// input, ensures the current user owns the item, and applies permitted changes.
        /// Titles are only updated for manually-entered items (not TMDB-linked items).
        /// </summary>
        /// <param name="id">The identifier of the watchlist item to update.</param>
        public async Task<IActionResult> OnPostAsync(int id)
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

            var item = await _watchlistService.GetByIdAsync(id, userId);

            if (item == null)
            {
                return NotFound();
            }

            // Only manually entered movies can have their title changed.
            if (!item.TmdbId.HasValue)
            {
                item.Title = Input.Title;
            }

            item.Status = Input.Status;
            item.Rating = Input.Rating;

            await _watchlistService.UpdateAsync(item);

            TempData["SuccessMessage"] = "Movie updated successfully.";

            return LocalRedirect(ReturnUrl ?? "/Watchlist/Index");
        }

    }
}
