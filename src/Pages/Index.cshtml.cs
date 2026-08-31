using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.Services;

namespace src.Pages
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

        public int TotalMovies { get; set; }

        public int WatchingMovies { get; set; }

        public int WatchedMovies { get; set; }

        public int PlannedMovies { get; set; }

        public async Task OnGetAsync()
        {
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
        }
    }
}
