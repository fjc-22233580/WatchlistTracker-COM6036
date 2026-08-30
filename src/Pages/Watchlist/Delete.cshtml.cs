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

        public async Task<IActionResult> OnPostAsync(int id)
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

            await _watchlistService.DeleteAsync(item);

            TempData["SuccessMessage"] = "Movie removed from your watchlist.";

            return RedirectToPage("/Watchlist/Index");
        }
    }
}
