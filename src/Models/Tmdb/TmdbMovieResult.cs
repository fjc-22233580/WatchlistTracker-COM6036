using System.Text.Json.Serialization;

namespace src.Models.Tmdb;

/// <summary>
/// Represents a single movie result returned by The Movie Database (TMDB) search API.
/// Property names match the TMDB JSON payload where necessary via JsonPropertyName.
/// </summary>
public class TmdbMovieResult
{
    /// <summary>
    /// The TMDB identifier for the movie.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The movie title.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Path to the poster image (as returned by TMDB). Combine with the image base URL when showing posters.
    /// </summary>
    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    /// <summary>
    /// Release date string from TMDB.
    /// </summary>
    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }

    /// <summary>
    /// Average vote score from TMDB (0-10). Nullable when not present.
    /// </summary>
    [JsonPropertyName("vote_average")]
    public double? VoteAverage { get; set; }

    /// <summary>
    /// Overview / synopsis text for the movie.
    /// </summary>
    public string? Overview { get; set; }
}
