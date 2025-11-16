using point72.api.DTOs;
using point72.api.Models;
using point72.api.Repositories;

namespace point72.api.Services;

/// <summary>
/// Service implementation for word inversion operations.
/// Implements business logic for inverting words and managing inversion records.
/// </summary>
public class WordInversionService : IWordInversionService
{
    private readonly IInversionRepository _repository;
    private readonly ILogger<WordInversionService> _logger;

    public WordInversionService(
        IInversionRepository repository,
        ILogger<WordInversionService> logger)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<InversionResponse> InvertAndSaveAsync(
        string sentence,
        CancellationToken cancellationToken = default)
    {
        // Validate input
        if (string.IsNullOrWhiteSpace(sentence))
        {
            _logger.LogWarning("Attempted to invert null or empty sentence");
            throw new ArgumentException("Sentence cannot be null or empty", nameof(sentence));
        }

        try
        {
            var trimmedSentence = sentence.Trim();
            var existingRecord = await _repository.FindByRequestAsync(trimmedSentence, cancellationToken);

            if (existingRecord != null)
            {
                existingRecord.RequestCount++;
                existingRecord.LastUpdatedAt = DateTime.UtcNow;
                await _repository.UpdateAsync(existingRecord, cancellationToken);
                _logger.LogInformation("Updated existing record for sentence: '{Sentence}'. New count: {Count}", trimmedSentence, existingRecord.RequestCount);
                return MapToResponse(existingRecord);
            }

            // Apply business logic: invert each word
            var invertedSentence = InvertWords(trimmedSentence);

            _logger.LogDebug(
                "Inverted sentence: '{Original}' -> '{Inverted}'",
                trimmedSentence,
                invertedSentence);

            // Create entity
            var record = new InversionRecord
            {
                Request = trimmedSentence,
                Response = invertedSentence,
                CreatedAt = DateTime.UtcNow,
                LastUpdatedAt = DateTime.UtcNow,
                RequestCount = 1
            };

            // Persist to database
            var savedRecord = await _repository.AddAsync(record, cancellationToken);

            _logger.LogInformation(
                "Successfully processed and saved inversion request. Record ID: {RecordId}",
                savedRecord.Id);

            // Map to DTO
            return MapToResponse(savedRecord);
        }
        catch (ArgumentException)
        {
            throw; // Re-throw business validation exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while processing inversion request for sentence: '{Sentence}'",
                sentence);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<InversionResponse>> GetAllInversionsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var records = await _repository.GetAllAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} inversion records for client",
                records.Count);

            return records.Select(MapToResponse).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while retrieving all inversion records");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<InversionResponse>> SearchByWordAsync(
        string word,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(word))
        {
            _logger.LogWarning("Attempted to search with null or empty word");
            throw new ArgumentException("Search word cannot be null or empty", nameof(word));
        }

        try
        {
            var records = await _repository.SearchByWordAsync(word.Trim(), cancellationToken);

            _logger.LogInformation(
                "Found {Count} records matching word: '{Word}'",
                records.Count,
                word);

            return records.Select(MapToResponse).ToList();
        }
        catch (ArgumentException)
        {
            throw; // Re-throw business validation exceptions
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while searching for word: '{Word}'",
                word);
            throw;
        }
    }

    /// <summary>
    /// Core algorithm: Inverts each word in the sentence while preserving word order and spacing.
    /// Time Complexity: O(n) where n is the total number of characters.
    /// Space Complexity: O(n) for storing the result.
    /// </summary>
    /// <param name="sentence">The sentence to process</param>
    /// <returns>Sentence with each word reversed</returns>
    private string InvertWords(string sentence)
    {
        if (string.IsNullOrEmpty(sentence))
            return string.Empty;

        // Split by spaces, preserving empty entries to maintain spacing
        var words = sentence.Split(' ', StringSplitOptions.None);

        // Reverse each word character-by-character
        var invertedWords = words.Select(word => 
            new string(word.Reverse().ToArray())
        );

        // Rejoin with spaces
        return string.Join(" ", invertedWords);
    }

    /// <summary>
    /// Maps an InversionRecord entity to an InversionResponse DTO.
    /// Separates domain models from API contracts.
    /// </summary>
    private static InversionResponse MapToResponse(InversionRecord record)
    {
        return new InversionResponse
        {
            Id = record.Id,
            Request = record.Request,
            Response = record.Response,
            CreatedAt = record.CreatedAt,
            RequestCount = record.RequestCount,
            LastUpdatedAt = record.LastUpdatedAt
        };
    }
}
