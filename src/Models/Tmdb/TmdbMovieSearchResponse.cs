namespace src.Models.Tmdb;

public class TmdbMovieSearchResponse
{
    public int Page { get; set; }

    public List<TmdbMovieResult> Results { get; set; } = new();

    public int TotalPages { get; set; }

    public int TotalResults { get; set; }
}