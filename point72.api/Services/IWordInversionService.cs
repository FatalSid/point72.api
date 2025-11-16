using point72.api.DTOs;
using point72.api.Models;

namespace point72.api.Services;

/// <summary>
/// Service interface for word inversion business logic.
/// Follows the Service pattern to encapsulate business operations.
/// </summary>
public interface IWordInversionService
{
    /// <summary>
    /// Inverts all words in a sentence and persists the request/response pair.
    /// </summary>
    /// <param name="sentence">The sentence to invert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The inversion response with record details</returns>
    Task<InversionResponse> InvertAndSaveAsync(string sentence, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all stored inversion records.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all inversion responses</returns>
    Task<IReadOnlyList<InversionResponse>> GetAllInversionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for inversion records containing a specific word.
    /// </summary>
    /// <param name="word">The word to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching inversion responses</returns>
    Task<IReadOnlyList<InversionResponse>> SearchByWordAsync(string word, CancellationToken cancellationToken = default);
}
