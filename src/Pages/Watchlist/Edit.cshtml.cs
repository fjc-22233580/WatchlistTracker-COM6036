using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.Models.InputModels;
using src.Services;

namespace src.Pages.Watchlist
{
    public class EditModel : PageModel
    {
        private readonly WatchlistService _watchlistService;
        private readonly UserManager<IdentityUser> _userManager;

        public EditModel(
            WatchlistService watchlistService,
            UserManager<IdentityUser> userManager)
        {
            _watchlistService = watchlistService;
            _userManager = userManager;
        }

        public bool IsTmdbMovie { get; set; }

        [BindProperty]
        public WatchlistItemInput Input { get; set; } = new();

        [BindProperty(SupportsGet = true)]
        public string? ReturnUrl { get; set; }

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
