using System.Net.Http.Headers;
using System.Net.Http.Json;
using src.Models.Tmdb;

namespace src.Services;

/// <summary>
/// Service for interacting with The Movie Database (TMDB) API.
/// Provides methods for searching movies, retrieving movie details, and managing poster URLs.
/// </summary>
public class TmdbService
{
    private readonly HttpClient _httpClient;
    private readonly string _accessToken;

    /// <summary>
    /// Initializes a new instance of the <see cref="TmdbService"/> class.
    /// </summary>
    /// <param name="httpClient">The HTTP client used for making requests to the TMDB API.</param>
    /// <param name="configuration">The application configuration containing the TMDB access token.</param>
    /// <exception cref="InvalidOperationException">Thrown when the TMDB access token is not configured.</exception>
    public TmdbService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;

        // Check we have a TMDB access key, if we do then add it to the auth header (bearer token)
        _accessToken = configuration["Tmdb:AccessToken"] ?? throw new InvalidOperationException("TMDB access token is not configured.");
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _accessToken);
    }

    /// <summary>
    /// Checks if the TMDB API is accessible and responding.
    /// </summary>
    /// <returns>A task that represents the asynchronous operation. The task result is <c>true</c> if the API is reachable; otherwise, <c>false</c>.</returns>
    public async Task<bool> IsAliveAsync()
    {
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

            var url = "https://api.themoviedb.org/3/configuration";
            var response = await _httpClient.GetAsync(url, cts.Token);

            return response.IsSuccessStatusCode;
        }
        catch (HttpRequestException)
        {
            return false;
        }
        catch (TaskCanceledException)
        {
            return false;
        }
    }

    /// <summary>
    /// Retrieves detailed information about a movie from the TMDB API.
    /// </summary>
    /// <param name="tmdbId">The TMDB ID of the movie to retrieve.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the movie details, or <c>null</c> if the movie is not found or an error occurs.</returns>
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

    /// <summary>
    /// Constructs a full URL for a TMDB poster image.
    /// </summary>
    /// <param name="posterPath">The poster path from the TMDB API (typically starting with '/'), or <c>null</c> if no poster is available.</param>
    /// <returns>The full URL to the poster image, or <c>null</c> if the poster path is null or empty.</returns>
    public string? GetPosterUrl(string? posterPath)
    {
        if (string.IsNullOrWhiteSpace(posterPath))
        {
            return null;
        }

        return $"https://image.tmdb.org/t/p/w185{posterPath}";
    }

    /// <summary>
    /// Searches for movies on TMDB matching the specified query.
    /// </summary>
    /// <param name="query">The search query text.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the search results, or <c>null</c> if the query is empty or an error occurs.</returns>
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