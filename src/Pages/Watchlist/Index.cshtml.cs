using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.Models;
using src.Models.Enums;
using src.Services;

namespace src.Pages.Watchlist
{
    public class IndexModel : PageModel
    {
        private readonly WatchlistService _watchlistService;
        private readonly UserManager<IdentityUser> _userManager;

        public IndexModel(
            WatchlistService watchlistService,
            UserManager<IdentityUser> userManager)
        {
            _watchlistService = watchlistService;
            _userManager = userManager;
        }


        [BindProperty(SupportsGet = true)]
        public string? SearchTerm { get; set; }

        [BindProperty(SupportsGet = true)]
        public WatchStatus? StatusFilter { get; set; }

        public List<WatchlistItem> WatchlistItems { get; set; } = new();

        public async Task<IActionResult> OnGetAsync()
        {
            var userId = _userManager.GetUserId(User);

            if (userId == null)
            {
                return Challenge();
            }

            WatchlistItems = await _watchlistService.GetUserWatchlistAsync(userId, SearchTerm, StatusFilter);

            return Page();
        }
    }
}
