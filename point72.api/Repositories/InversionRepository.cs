using Microsoft.EntityFrameworkCore;
using point72.api.Data;
using point72.api.Models;

namespace point72.api.Repositories;

/// <summary>
/// Repository implementation for InversionRecord entity operations.
/// Provides data access methods with EF Core.
/// </summary>
public class InversionRepository : IInversionRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<InversionRepository> _logger;

    public InversionRepository(
        ApplicationDbContext context,
        ILogger<InversionRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <inheritdoc />
    public async Task<InversionRecord> AddAsync(InversionRecord record, CancellationToken cancellationToken = default)
    {
        if (record == null)
            throw new ArgumentNullException(nameof(record));

        try
        {
            _context.InversionRecords.Add(record);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully added inversion record with ID: {RecordId}",
                record.Id);

            return record;
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Database error occurred while adding inversion record");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<InversionRecord>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var records = await _context.InversionRecords
                .OrderByDescending(r => r.CreatedAt)
                .AsNoTracking()
                .ToListAsync(cancellationToken);

            _logger.LogInformation(
                "Retrieved {Count} inversion records",
                records.Count);

            return records;
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
    public async Task<IReadOnlyList<InversionRecord>> SearchByWordAsync(
        string word,
        CancellationToken cancellationToken = default)
    {
        return await _context.InversionRecords
            .Where(r => r.Request.Contains(word) || r.Response.Contains(word))
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<InversionRecord?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            var record = await _context.InversionRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id, cancellationToken);

            if (record != null)
            {
                _logger.LogInformation(
                    "Retrieved inversion record with ID: {RecordId}",
                    id);
            }
            else
            {
                _logger.LogWarning(
                    "Inversion record with ID: {RecordId} not found",
                    id);
            }

            return record;
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while retrieving inversion record with ID: {RecordId}",
                id);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<InversionRecord?> FindByRequestAsync(
        string request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.InversionRecords
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.Request == request, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex,
                "Error occurred while finding inversion record by request: '{Request}'",
                request);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task UpdateAsync(
        InversionRecord record,
        CancellationToken cancellationToken = default)
    {
        if (record == null)
            throw new ArgumentNullException(nameof(record));

        try
        {
            _context.InversionRecords.Update(record);
            await _context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation(
                "Successfully updated inversion record with ID: {RecordId}",
                record.Id);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogError(
                ex,
                "Database error occurred while updating inversion record with ID: {RecordId}",
                record.Id);
            throw;
        }
    }
}
