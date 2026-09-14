using System.Text.Json.Serialization;

namespace src.Models.Tmdb;

public class TmdbMovieResult
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty;

    [JsonPropertyName("poster_path")]
    public string? PosterPath { get; set; }

    [JsonPropertyName("release_date")]
    public string? ReleaseDate { get; set; }

    [JsonPropertyName("vote_average")]
    public double? VoteAverage { get; set; }

    public string? Overview { get; set; }
}