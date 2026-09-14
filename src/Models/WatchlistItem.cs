using src.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace src.Models;

public class WatchlistItem
{
    public int Id { get; set; }

    public int? TmdbId { get; set; }

    public string? PosterPath { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public WatchStatus Status { get; set; }

    [Range(1, 10)]
    public int? Rating { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public string? Overview { get; set; }

    public double? VoteAverage { get; set; }
}