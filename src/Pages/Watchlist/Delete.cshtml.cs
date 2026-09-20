using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.Models;
using src.Services;

namespace src.Pages.Watchlist
{
    public class DeleteModel : PageModel
    {
        private readonly WatchlistService _watchlistService;
        private readonly UserManager<IdentityUser> _userManager;

        public DeleteModel(
            WatchlistService watchlistService,
            UserManager<IdentityUser> userManager)
        {
            _watchlistService = watchlistService;
            _userManager = userManager;
        }

        public WatchlistItem WatchlistItem { get; set; } = null!;

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

            WatchlistItem = item;

            return Page();
        }

        /// <summary>
        /// Handles submission of the delete form.
        /// Validates the authenticated user and verifies that the requested movie
        /// belongs to that user before deleting it.
        /// </summary>
        /// <param name="id">The identifier of the movie to delete.</param>
        public async Task<IActionResult> OnPostAsync(int id)
        {
            // Get the identifier of the currently authenticated user.
            var userId = _userManager.GetUserId(User);
            if (userId == null)
            {
                return Challenge();
            }

            // Retrieve the requested movie and verify that it belongs to the current user.
            var item = await _watchlistService.GetByIdAsync(id, userId);
            if (item == null)
            {
                return NotFound();
            }

            // Delete the movie from the user's watchlist.
            await _watchlistService.DeleteAsync(item);

            // Notify the user that the movie was successfully removed.
            TempData["SuccessMessage"] = "Movie removed from your watchlist.";

            // Return the user to the watchlist index page.
            return RedirectToPage("/Watchlist/Index");
        }
    }
}
