using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.Models;
using src.Services;

namespace src.Pages.Watchlist
{
    /// <summary>
    /// Page model responsible for showing the delete confirmation for a watchlist item
    /// and handling the delete action. Ensures the authenticated user owns the item
    /// before allowing deletion.
    /// </summary>
    public class DeleteModel : PageModel
    {
        private readonly WatchlistService _watchlistService;
        private readonly UserManager<IdentityUser> _userManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="DeleteModel"/> class.
        /// </summary>
        /// <param name="watchlistService">Service used to access and modify the watchlist.</param>
        /// <param name="userManager">ASP.NET Core Identity user manager.</param>
        public DeleteModel(
            WatchlistService watchlistService,
            UserManager<IdentityUser> userManager)
        {
            _watchlistService = watchlistService;
            _userManager = userManager;
        }

        /// <summary>
        /// The watchlist item being considered for deletion. Populated by OnGetAsync.
        /// </summary>
        public WatchlistItem WatchlistItem { get; set; } = null!;

        /// <summary>
        /// Handles GET requests to display the delete confirmation for the specified item id.
        /// Verifies that the currently authenticated user owns the item before showing details.
        /// </summary>
        /// <param name="id">Identifier of the watchlist item to display.</param>
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
