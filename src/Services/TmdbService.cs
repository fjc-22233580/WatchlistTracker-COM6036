using System.Net.Http.Headers;
using System.Net.Http.Json;
using src.Models.Tmdb;

namespace src.Services;

public class TmdbService
{
    private readonly HttpClient _httpClient;
    private readonly string _accessToken;

    public TmdbService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        // Check we have a TMDB access key, if we do then add it to the auth header (bearer token)
        _accessToken = configuration["Tmdb:AccessToken"] ?? throw new InvalidOperationException("TMDB access token is not configured.");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
    }

    public async Task<TmdbMovieResult?> GetMovieAsync(int tmdbId)
    {
        try
        {
            var url =
                $"https://api.themoviedb.org/3/movie/{tmdbId}" +
                $"?language=en-US";

            return await _httpClient.GetFromJsonAsync<TmdbMovieResult>(url);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null;
        }
    }

    public string? GetPosterUrl(string? posterPath)
    {
        if (string.IsNullOrWhiteSpace(posterPath))
        {
            return null;
        }

        return $"https://image.tmdb.org/t/p/w185{posterPath}";
    }

    public async Task<TmdbMovieSearchResponse?> SearchMoviesAsync(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            return null;
        }

        try
        {
            var url =
                $"https://api.themoviedb.org/3/search/movie" +
                $"?query={Uri.EscapeDataString(query)}" +
                $"&include_adult=false" +
                $"&language=en-US" +
                $"&page=1";

            return await _httpClient.GetFromJsonAsync<TmdbMovieSearchResponse>(url);
        }
        catch (HttpRequestException)
        {
            return null;
        }
        catch (TaskCanceledException)
        {
            return null; 
        }
    }
}