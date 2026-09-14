using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using src.Models.Tmdb;
using src.Services;

namespace src.Pages
{
    public class TmdbTestModel : PageModel
    {
        private readonly TmdbService _tmdbService;

        public TmdbTestModel(TmdbService tmdbService)
        {
            _tmdbService = tmdbService;
        }

        public List<TmdbMovieResult> Results { get; set; } = new();

        public async Task OnGetAsync()
        {
            var response = await _tmdbService.SearchMoviesAsync("The Matrix");

            Results = response?.Results ?? new();
        }
    }
}
