using Microsoft.AspNetCore.Mvc;
using point72.api.DTOs;
using point72.api.Services;
using System.Net;

namespace point72.api.Controllers;

/// <summary>
/// API controller for word inversion operations.
/// Provides endpoints for inverting sentences, searching records, and retrieving history.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class WordInversionController : ControllerBase
{
    private readonly IWordInversionService _service;
    private readonly ILogger<WordInversionController> _logger;

    public WordInversionController(
        IWordInversionService service,
        ILogger<WordInversionController> logger)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Inverts all words in a sentence and stores the request/response pair.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     POST /api/wordinversion/invert
    ///     {
    ///         "sentence": "abc def"
    ///     }
    ///     
    /// Sample response:
    /// 
    ///     {
    ///         "id": 1,
    ///         "request": "abc def",
    ///         "response": "cba fed",
    ///         "createdAt": "2024-01-16T10:30:00Z"
    ///     }
    /// </remarks>
    /// <param name="request">The sentence to invert</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The inverted sentence with record metadata</returns>
    /// <response code="200">Returns the inverted sentence and record details</response>
    /// <response code="400">If the sentence is null, empty, or invalid</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpPost("invert")]
    [ProducesResponseType(typeof(InversionResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<InversionResponse>> InvertSentence(
        [FromBody] InversionRequest request,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received inversion request for sentence: '{Sentence}'",
            request.Sentence);

        if (!ModelState.IsValid)
        {
            _logger.LogWarning("Invalid model state for inversion request");
            return BadRequest(ModelState);
        }

        var response = await _service.InvertAndSaveAsync(
            request.Sentence,
            cancellationToken);

        _logger.LogInformation(
            "Successfully processed inversion request. Record ID: {RecordId}",
            response.Id);

        return Ok(response);
    }

    /// <summary>
    /// Retrieves all stored request/response pairs.
    /// </summary>
    /// <remarks>
    /// Sample response:
    /// 
    ///     [
    ///         {
    ///             "id": 2,
    ///             "request": "hello world",
    ///             "response": "olleh dlrow",
    ///             "createdAt": "2024-01-16T10:31:00Z"
    ///         },
    ///         {
    ///             "id": 1,
    ///             "request": "abc def",
    ///             "response": "cba fed",
    ///             "createdAt": "2024-01-16T10:30:00Z"
    ///         }
    ///     ]
    /// </remarks>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all inversion records ordered by creation date (newest first)</returns>
    /// <response code="200">Returns the list of all inversion records</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("all")]
    [ProducesResponseType(typeof(IReadOnlyList<InversionResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<InversionResponse>>> GetAll(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Received request to retrieve all inversion records");

        var records = await _service.GetAllInversionsAsync(cancellationToken);

        _logger.LogInformation(
            "Successfully retrieved {Count} inversion records",
            records.Count);

        return Ok(records);
    }

    /// <summary>
    /// Searches for request/response pairs containing a specific word.
    /// </summary>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/wordinversion/search?word=hello
    ///     
    /// Sample response:
    /// 
    ///     [
    ///         {
    ///             "id": 2,
    ///             "request": "hello world",
    ///             "response": "olleh dlrow",
    ///             "createdAt": "2024-01-16T10:31:00Z"
    ///         }
    ///     ]
    /// </remarks>
    /// <param name="word">The word to search for in requests and responses</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of matching inversion records</returns>
    /// <response code="200">Returns the list of matching records</response>
    /// <response code="400">If the search word is null or empty</response>
    /// <response code="500">If an internal server error occurs</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(IReadOnlyList<InversionResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), (int)HttpStatusCode.InternalServerError)]
    public async Task<ActionResult<IReadOnlyList<InversionResponse>>> SearchByWord(
        [FromQuery] string word,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Received search request for word: '{Word}'",
            word);

        if (string.IsNullOrWhiteSpace(word))
        {
            _logger.LogWarning("Search request received with null or empty word");
            return BadRequest(new ProblemDetails
            {
                Status = (int)HttpStatusCode.BadRequest,
                Title = "Validation Error",
                Detail = "Search word cannot be null or empty"
            });
        }

        var records = await _service.SearchByWordAsync(word, cancellationToken);

        _logger.LogInformation(
            "Found {Count} records matching word: '{Word}'",
            records.Count,
            word);

        return Ok(records);
    }
}
