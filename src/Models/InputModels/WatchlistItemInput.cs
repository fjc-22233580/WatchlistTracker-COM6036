using src.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace src.Models.InputModels;

/// <summary>
/// Input model used to capture and validate data when creating or updating a watch list item.
/// This is a DTO for incoming requests and is not the domain entity stored in a repository.
/// </summary>
public class WatchlistItemInput
{
    /// <summary>
    /// The display title of the item. Required and limited to 200 characters.
    /// </summary>
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// The current watch status of the item (Planned, Watching, Watched).
    /// </summary>
    public WatchStatus Status { get; set; }

    /// <summary>
    /// Optional user rating for the item on a 1-10 scale. Null when not provided.
    /// </summary>
    [Range(1, 10)]
    public int? Rating { get; set; }
}