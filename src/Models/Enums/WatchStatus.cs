namespace src.Models.Enums;

/// <summary>
/// Represents the viewing lifecycle state for an item in the watch list.
/// </summary>
public enum WatchStatus
{
    /// <summary>
    /// The item added to the list but viewing has not started.
    /// </summary>
    Planned,

    /// <summary>
    /// The item is currently being watched.
    /// </summary>
    Watching,

    /// <summary>
    /// The item has been watched.
    /// </summary>
    Watched
}