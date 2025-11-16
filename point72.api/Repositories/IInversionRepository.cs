using point72.api.Models;

namespace point72.api.Repositories;

/// <summary>
/// Repository interface for InversionRecord entity operations.
/// Follows the Repository pattern to abstract data access logic.
/// </summary>
public interface IInversionRepository
{
    /// <summary>
    /// Adds a new inversion record to the database.
    /// </summary>
    /// <param name="record">The inversion record to add</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The added record with generated ID</returns>
    Task<InversionRecord> AddAsync(InversionRecord record, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves all inversion records ordered by creation date (newest first).
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all inversion records</returns>
    Task<IReadOnlyList<InversionRecord>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for inversion records containing the specified word in either request or response.
    /// </summary>
    /// <param name="word">The word to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching inversion records</returns>
    Task<IReadOnlyList<InversionRecord>> SearchByWordAsync(string word, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an inversion record by its ID.
    /// </summary>
    /// <param name="id">The record ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The inversion record if found, null otherwise</returns>
    Task<InversionRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves an inversion record by its request string.
    /// </summary>
    /// <param name="request">The request string to search for</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The matching inversion record if found, null otherwise</returns>
    Task<InversionRecord?> FindByRequestAsync(string request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing inversion record in the database.
    /// </summary>
    /// <param name="record">The inversion record with updated values</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task that represents the asynchronous operation</returns>
    Task UpdateAsync(InversionRecord record, CancellationToken cancellationToken = default);
}
