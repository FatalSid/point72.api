using Microsoft.EntityFrameworkCore;
using point72.api.Models;

namespace point72.api.Data;

/// <summary>
/// Application database context for managing word inversion records.
/// Provides access to the InversionRecords table.
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    /// <summary>
    /// DbSet for managing InversionRecord entities.
    /// </summary>
    public DbSet<InversionRecord> InversionRecords { get; set; } = null!;

    /// <summary>
    /// Configures entity models and relationships.
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure InversionRecord entity
        modelBuilder.Entity<InversionRecord>(entity =>
        {
            // Index on CreatedAt for efficient ordering and filtering
            entity.HasIndex(e => e.CreatedAt)
                  .HasDatabaseName("IX_InversionRecords_CreatedAt");

            // Composite index for text search optimization
            entity.HasIndex(e => new { e.Request, e.Response })
                  .HasDatabaseName("IX_InversionRecords_Request_Response");
        });
    }
}
