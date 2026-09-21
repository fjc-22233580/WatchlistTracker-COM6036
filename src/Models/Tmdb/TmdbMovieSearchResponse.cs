namespace src.Models.Tmdb;

/// <summary>
/// Represents the response returned by TMDB for a movie search query.
/// Contains paging information and the list of result items.
/// </summary>
public class TmdbMovieSearchResponse
{
    /// <summary>
    /// The current page of results (1-based).
    /// </summary>
    public int Page { get; set; }

    /// <summary>
    /// The collection of movie results for the current page.
    /// </summary>
    public List<TmdbMovieResult> Results { get; set; } = new();

    /// <summary>
    /// Total number of pages available for the query.
    /// </summary>
    public int TotalPages { get; set; }

    /// <summary>
    /// Total number of results found for the query across all pages.
    /// </summary>
    public int TotalResults { get; set; }
}
