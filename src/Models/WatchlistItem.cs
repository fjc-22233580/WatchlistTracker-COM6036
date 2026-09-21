using src.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace src.Models;

/// <summary>
/// Domain model representing an item on a user's watch list.
/// This model is intended to be stored/persisted and contains additional metadata
/// returned from external sources (e.g. TMDB) where available.
/// </summary>
public class WatchlistItem
{
    /// <summary>
    /// Primary key for the watch list item.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Optional TMDB identifier for the item when linked to an external movie record.
    /// </summary>
    public int? TmdbId { get; set; }

    /// <summary>
    /// Optional poster path returned by TMDB. Combine with the image base URL to render.
    /// </summary>
    public string? PosterPath { get; set; }

    /// <summary>
    /// Title of the item. Required and limited by the StringLength attribute.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Current watch status for the item (Planned, Watching, Watched).
    /// </summary>
    [Required]
    public WatchStatus Status { get; set; }

    /// <summary>
    /// Optional user rating on a 1-10 scale.
    /// </summary>
    [Range(1, 10)]
    public int? Rating { get; set; }

    /// <summary>
    /// Identifier of the user who owns this watch list item. Required for multi-user scenarios.
    /// </summary>
    [Required]
    public string UserId { get; set; } = string.Empty;

    /// <summary>
    /// Optional synopsis or overview text for the item.
    /// </summary>
    public string? Overview { get; set; }

    /// <summary>
    /// Average vote score from TMDB (0-10). Nullable when not present.
    /// </summary>
    public double? VoteAverage { get; set; }
}
