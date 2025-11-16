using System.ComponentModel.DataAnnotations;

namespace point72.api.DTOs;

/// <summary>
/// Data Transfer Object for word inversion requests.
/// </summary>
public class InversionRequest
{
    /// <summary>
    /// The sentence to be inverted. Each word will be reversed.
    /// Example: "abc def" becomes "cba fed"
    /// </summary>
    [Required(ErrorMessage = "Sentence is required")]
    [StringLength(2000, MinimumLength = 1, ErrorMessage = "Sentence must be between 1 and 2000 characters")]
    public string Sentence { get; set; } = string.Empty;
}
