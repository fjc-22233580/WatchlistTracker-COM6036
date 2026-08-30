using src.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace src.Models.InputModels;

public class WatchlistItemInput
{
    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    public WatchStatus Status { get; set; }

    [Range(1, 10)]
    public int? Rating { get; set; }
}