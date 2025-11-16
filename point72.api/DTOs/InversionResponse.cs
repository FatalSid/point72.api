namespace point72.api.DTOs;

/// <summary>
/// Data Transfer Object for word inversion responses.
/// Encapsulates the result of a word inversion operation.
/// </summary>
public class InversionResponse
{
    /// <summary>
    /// Unique identifier of the inversion record.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// The original sentence that was provided.
    /// </summary>
    public string Request { get; set; } = string.Empty;

    /// <summary>
    /// The inverted sentence with all words reversed.
    /// </summary>
    public string Response { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when the inversion was performed.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// The number of times the request has been made.
    /// </summary>
    public int RequestCount { get; set; }

    /// <summary>
    /// UTC timestamp when the record was last updated.
    /// </summary>
    public DateTime LastUpdatedAt { get; set; }
}
