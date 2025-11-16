using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace point72.api.Models;

/// <summary>
/// Entity representing a word inversion request-response pair stored in the database.
/// Maintains an audit trail of all word inversion operations.
/// </summary>
[Table("InversionRecords")]
public class InversionRecord
{
    /// <summary>
    /// Unique identifier for the inversion record.
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    /// <summary>
    /// The original sentence provided by the user.
    /// </summary>
    [Required]
    [MaxLength(2000)]
    public string Request { get; set; } = string.Empty;

    /// <summary>
    /// The sentence with all words inverted.
    /// </summary>
    [Required]
    [MaxLength(2000)]
    public string Response { get; set; } = string.Empty;

    /// <summary>
    /// UTC timestamp when the record was created.
    /// </summary>
    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// The number of times this record has been requested.
    /// </summary>
    [Required]
    public int RequestCount { get; set; } = 0;

    /// <summary>
    /// UTC timestamp when the record was last updated.
    /// </summary>
    [Required]
    public DateTime LastUpdatedAt { get; set; } = DateTime.UtcNow;
}
