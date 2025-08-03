using System.Diagnostics;
using System.IO;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Qhta.Unicode.Models;

/// <summary>
/// Database context for the Unicode data model. Contains DbSet properties for various entities such as UcdCodePoint, UcdBlock, WritingSystem, etc.
/// </summary>
public partial class _DbContext : DbContext, IDisposable
{
  /// <summary>
  /// Default constructor for the _DbContext.
  /// </summary>
  public _DbContext()
  {
  }

  /// <summary>
  /// Initializes a new instance of the <see cref="_DbContext"/> class using the specified options.
  /// </summary>
  /// <param name="options">The options to be used by the <see cref="_DbContext"/>. This parameter cannot be null.</param>
  public _DbContext(DbContextOptions<_DbContext> options)
      : base(options)
  {
  }

  /// <summary>
  /// Gets or sets the collection of Unicode code points.
  /// </summary>
  public virtual DbSet<UcdCodePoint> CodePoints { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Gets or sets the collection of Unicode Character Database (UCD) blocks.
  /// </summary>
  public virtual DbSet<UcdBlock> UcdBlocks { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Gets or sets the collection of writing systems available in the database.
  /// </summary>
  public virtual DbSet<WritingSystem> WritingSystems { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Gets or sets the collection of writing system kinds in the database.
  /// </summary>
  public virtual DbSet<WritingSystemKindEntity> WritingSystemKinds { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Gets or sets the collection of writing system types in the database.
  /// </summary>
  public virtual DbSet<WritingSystemTypeEntity> WritingSystemTypes { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Gets or sets the collection of Unicode category entities.
  /// </summary>
  public virtual DbSet<UnicodeCategoryEntity> UnicodeCategories { [DebuggerStepThrough] get; set; }


  /// <summary>
  /// Gets or sets the collection of name generation methods in the database.
  /// </summary>
  public virtual DbSet<NameGenMethodEntity> NameGenMethods { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// Configures the database context options.
  /// </summary>
  /// <remarks>This method is called automatically by the framework to configure the database context. It sets
  /// up the connection string from the "appSettings.json" file if the options are not already configured.</remarks>
  /// <param name="optionsBuilder">The builder used to configure the database context options.</param>
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appSettings.json")
        .Build();

      var connectionString = configuration.GetConnectionString("DefaultConnection");
      optionsBuilder.UseJet(connectionString);
      //.LogTo((str)=>Debug.WriteLine(str), LogLevel.Error);
      //optionsBuilder.EnableSensitiveDataLogging(true);
    }
  }

  /// <summary>
  /// Configures the entity framework model for the context.
  /// </summary>
  /// <remarks>This method is used to define the schema of the database by configuring entity properties, keys,
  /// relationships, and constraints. It is called when the model for a derived context has been initialized, but before
  /// the model has been locked down and used to initialize the context.</remarks>
  /// <param name="modelBuilder">The <see cref="ModelBuilder"/> used to configure the model for the context.</param>
  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {

    modelBuilder.Entity<Alias>(entity =>
    {
      entity.HasKey(e => new { e.Ord, Name = e.Name }).HasName("PrimaryKey");
      entity.Property(e => e.Name).HasColumnName("Alias");
      entity.Property(e => e.Name).HasMaxLength(255);
      entity.Property(e => e.Type).HasConversion<byte>();

      entity.HasOne(d => d.UcdCodePoint).WithMany(p => p.Aliases)
              .HasForeignKey(d => d.Ord)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("UnicodeDataAliases");
    });

    modelBuilder.Entity<UcdBlock>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PrimaryKey");
      entity.HasIndex(e => e.Id, "ID");
      entity.HasIndex(e => e.WritingSystemId, "WritingSystemID");
      entity.Property(e => e.Id).HasColumnType("counter").HasColumnName("ID");
      entity.Property(e => e.BlockName).HasMaxLength(255);
      entity.Property(e => e.End).HasDefaultValue(0);
      entity.Property(e => e.Range).HasMaxLength(255);
      entity.Property(e => e.Start).HasDefaultValue(0);
      entity.Property(e => e.WritingSystemId).HasColumnName("WritingSystemID");

      entity.HasOne(ub => ub.WritingSystem)
         .WithMany(ws => ws.UcdBlocks)
        .HasForeignKey(ub => ub.WritingSystemId);
    });

    modelBuilder.Entity<UcdCodePoint>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PrimaryKey");

      entity.HasIndex(e => e.CharName, "CharName");
      entity.Property(e => e.CharName).HasMaxLength(255);
      entity.Property(e => e.Bidir).HasMaxLength(3);
      entity.Property(e => e.Code).HasMaxLength(6);
      entity.Property(e => e.Comment).HasMaxLength(255);
      entity.Property(e => e.Ctg).HasMaxLength(2);
      entity.Property(e => e.Comb).HasConversion<byte?>();
      entity.Property(e => e.DecDigitVal).HasMaxLength(1);
      entity.Property(e => e.Decomposition).HasMaxLength(255);
      entity.Property(e => e.Description).HasMaxLength(255);
      entity.Property(e => e.DigitVal).HasMaxLength(1);
      entity.Property(e => e.NumVal).HasMaxLength(15);
      entity.Property(e => e.Glyph).HasMaxLength(2);
      entity.Property(e => e.Lower).HasMaxLength(6);
      entity.Property(e => e.Mirr).HasMaxLength(1);
      entity.Property(e => e.OldDescription).HasMaxLength(255);
      entity.Property(e => e.Title).HasMaxLength(6);
      entity.Property(e => e.Upper).HasMaxLength(6);
    });

    modelBuilder.Entity<WritingSystem>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PrimaryKey");
      //entity.HasIndex(e => new { e.Abbr, e.Ext }, "Abbr").IsUnique();
     // entity.HasIndex(e => e.Ctg, "Ctg").IsUnique();
      //entity.HasIndex(e => new { e.Name, e.Type }, "Name").IsUnique();
      //entity.HasIndex(e => e.KeyPhrase, "KeyPhrase").IsUnique();
      entity.HasIndex(e => e.ParentId, "ParentID");
      entity.HasIndex(e => e.Kind, "Kind");
      entity.HasIndex(e => e.Type, "Type");
      entity.Property(e => e.Type).HasConversion<byte>();
      entity.Property(e => e.Kind).HasConversion<byte>();
      entity.Property(e => e.Id).HasColumnName("ID");
      entity.Property(e => e.Name).HasMaxLength(255);
      entity.Property(e => e.KeyPhrase).HasMaxLength(255);
      entity.Property(e => e.Abbr).HasMaxLength(255);
      entity.Property(e => e.Ctg).HasMaxLength(2);
      entity.Property(e => e.Ext).HasMaxLength(10);
      entity.Property(e => e.Iso).HasMaxLength(255).HasColumnName("ISO");
      entity.Property(e => e.Kind).HasMaxLength(15);
      entity.Property(e => e.Type).HasMaxLength(15);
      entity.Property(e => e.ParentId).HasColumnName("ParentID");
      entity.Property(e => e.NameGenMethod).HasConversion<byte>();
      entity.Property(e => e.NameGenMethod).HasMaxLength(15);
      entity.Property(e => e.NameGenFile).HasMaxLength(255);
      entity.HasOne(d => d.ParentSystem).WithMany(p => p.Children)
              .HasForeignKey(d => d.ParentId)
              .HasConstraintName("WritingSystemsWritingSystems");
    });

    modelBuilder.Entity<WritingSystemTypeEntity>(entity =>
    {
      entity.Property(e => e.Id).HasConversion<byte>();
      entity.HasKey(e => e.Id).HasName("PrimaryKey");
      entity.Property(e => e.Name).HasMaxLength(15);
      entity.HasIndex(e => e.Name, "Name").IsUnique();
      entity.Property(e => e.Description).HasMaxLength(255);
    });

    modelBuilder.Entity<WritingSystemKindEntity>(entity =>
    {
      entity.Property(e => e.Id).HasConversion<byte>();
      entity.HasKey(e => e.Id).HasName("PrimaryKey");
      entity.Property(e => e.Name).HasMaxLength(15);
      entity.HasIndex(e => e.Name, "Name").IsUnique();
      entity.Property(e => e.Type).HasConversion<byte>();
      entity.Property(e => e.Description).HasMaxLength(255);
    });

    modelBuilder.Entity<UnicodeCategoryEntity>(entity =>
    {
      entity.Property(e => e.Id).HasConversion<byte>();
      entity.HasKey(e => e.Id).HasName("PrimaryKey");
      entity.Property(e => e.Ctg).HasMaxLength(2);
      entity.HasIndex(e => e.Ctg, "Ctg").IsUnique();
      entity.Property(e => e.Name).HasMaxLength(255);
      entity.Property(e => e.Comment).HasMaxLength(255);
    });

    modelBuilder.Entity<NameGenMethodEntity>(entity =>
    {
      entity.Property(e => e.Id).HasConversion<byte>();
      entity.HasKey(e => e.Id).HasName("PrimaryKey");
      entity.Property(e => e.Name).HasMaxLength(15);
      entity.HasIndex(e => e.Name, "Name").IsUnique();
      entity.Property(e => e.Description).HasMaxLength(255);
    });

    OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

  #region IDisposable implementation
  /// <summary>
  /// Gets a value indicating whether there are unsaved changes in the current context.
  /// </summary>
  public bool ThereAreUnsavedChanges => ChangeTracker.HasChanges();

  /// <summary>
  /// Allows automatic saving of changes to the database when the context is disposed.
  /// </summary>
  public bool AutoSaveChanges = true;

  /// <summary>
  /// Gets the count of entities that have been modified in the current change tracker.
  /// </summary>
  public int GetModifiedEntitiesCount()
  {
    return ChangeTracker.Entries()
      .Count(entry => entry.State == EntityState.Modified);
  }

  /// <summary>
  /// Gets the count of entities that have been added in the current change tracker.
  /// </summary>
  public int GetAddedEntitiesCount()
  {
    return ChangeTracker.Entries()
      .Count(entry => entry.State == EntityState.Added);
  }

  /// <summary>
  /// Gets the count of entities that have been deleted in the current change tracker.
  /// </summary>
  public int GetDeletedEntitiesCount()
  {
    return ChangeTracker.Entries()
      .Count(entry => entry.State == EntityState.Deleted);
  }

  /// <summary>
  /// Disposes the context and saves changes if there are any unsaved changes.
  /// </summary>
  /// <param name="disposing"></param>
  protected virtual void Dispose(bool disposing)
  {
    if (disposing)
    {
      if (ChangeTracker.HasChanges() && AutoSaveChanges)
      {
        SaveChanges();
        Debug.WriteLine("Data changes saved automatically.");
      }
    }
  }

  /// <summary>
  /// Releases all resources used by the current instance of the class.
  /// </summary>
  /// <remarks>This method should be called when the object is no longer needed to free up resources. It calls
  /// the protected <c>Dispose(bool disposing)</c> method with the <c>disposing</c> parameter set to <see
  /// langword="true"/> and suppresses finalization.</remarks>
  public override void Dispose()
  {
    Dispose(true);
    base.Dispose();
    GC.SuppressFinalize(this);
  }

  /// <summary>
  /// Destructs the context and releases all resources used by it.
  /// </summary>
  ~_DbContext()
  {
    Dispose(false);
  }
  #endregion
}
