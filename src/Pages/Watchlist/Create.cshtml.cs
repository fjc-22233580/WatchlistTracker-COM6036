using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.Models;
using src.Models.InputModels;
using src.Services;

namespace src.Pages.Watchlist
{
    [Authorize]
    public class CreateModel : PageModel
    {
        private readonly WatchlistService _watchlistService;
        private readonly UserManager<IdentityUser> _userManager;

        public CreateModel(
            WatchlistService watchlistService,
            UserManager<IdentityUser> userManager)
        {
            _watchlistService = watchlistService;
            _userManager = userManager;
        }

        [BindProperty]
        public WatchlistItemInput Input { get; set; } = new();

        public void OnGet()
        {
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

            var item = new WatchlistItem
            {
                Title = Input.Title,
                Status = Input.Status,
                Rating = Input.Rating,
                UserId = userId
            };

            await _watchlistService.AddAsync(item);

            // Set response message for next request
            TempData["SuccessMessage"] = "Movie added to your watchlist.";

            return RedirectToPage("/Watchlist/Create");
        }
    }
}
