using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

using Syncfusion.Windows.Shared;

namespace Qhta.Unicode.Models;

public partial class _DbContext : DbContext
{
  public _DbContext()
  {
  }

  public _DbContext(DbContextOptions<_DbContext> options)
      : base(options)
  {
  }

                                                 
  public virtual DbSet<UcdBlock> UcdBlocks { get; set; }

  
  public virtual DbSet<UcdRange> UcdRanges { get; set; }

  public virtual DbSet<UcdCodePoint> CodePoints { get; set; }

  public virtual DbSet<WritingSystem> WritingSystems { get; set; }

  public virtual DbSet<WritingSystemKind> WritingSystemKinds { get; set; }

  public virtual DbSet<WritingSystemType> WritingSystemTypes { get; set; }

  
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    if (!optionsBuilder.IsConfigured)
    {
      var configuration = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appSettings.json")
        .Build();

      var connectionString = configuration.GetConnectionString("DefaultConnection");
      optionsBuilder.UseJet(connectionString)
        .LogTo((str)=>Debug.WriteLine(str), LogLevel.Information);
      optionsBuilder.EnableSensitiveDataLogging(true);
    }
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
   
    modelBuilder.Entity<Alias>(entity =>
    {
      entity.HasKey(e => new { e.Ord, e.Alias1 }).HasName("PrimaryKey");

      entity.Property(e => e.Alias1).HasColumnName("Alias");

      entity.HasOne(d => d.UcdCodePoint).WithMany(p => p.Aliases)
              .HasForeignKey(d => d.Ord)
              .OnDelete(DeleteBehavior.ClientSetNull)
              .HasConstraintName("UnicodeDataAliases");
    });

    //modelBuilder.Entity<AliasType>(entity =>
    //{
    //  entity.HasKey(e => e.Id).HasName("PrimaryKey");

    //  entity.Property(e => e.Id).HasColumnName("ID");
    //  entity.Property(e => e.Type).HasMaxLength(255);
    //});


    modelBuilder.Entity<UcdBlock>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PrimaryKey");

      entity.HasIndex(e => e.Id, "ID");

      entity.HasIndex(e => e.WritingSystemId, "WritingSystemID");

      entity.Property(e => e.Id)
              .HasColumnType("counter")
              .HasColumnName("ID");
      entity.Property(e => e.BlockName).HasMaxLength(255);
      entity.Property(e => e.End).HasDefaultValue(0);
      entity.Property(e => e.Range).HasMaxLength(255);
      entity.Property(e => e.Start).HasDefaultValue(0);
      entity.Property(e => e.WritingSystemId).HasColumnName("WritingSystemID");

      entity.HasOne(ub => ub.WritingSystem)
         .WithMany(ws => ws.UcdBlocks)
        .HasForeignKey(ub => ub.WritingSystemId);
    });

    modelBuilder.Entity<UcdRange>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PrimaryKey");
      
      entity.HasIndex(e => e.Range, "Range");
      entity.HasIndex(e => e.BlockId, "BlockID");
      entity.HasIndex(e => e.WritingSystemId, "WritingSystemID");

      entity.Property(e => e.Range)
              .HasMaxLength(12)
              .HasColumnName("Range");
      entity.Property(e => e.Language).HasMaxLength(255);
      entity.Property(e => e.RangeName).HasMaxLength(255);
      entity.Property(e => e.Standard).HasMaxLength(255);
      entity.Property(e => e.WritingSystemId).HasColumnName("WritingSystemID");

      entity.HasOne(d => d.UcdBlock).WithMany(p => p.UcdRanges)
        .HasForeignKey(d => d.BlockId);

      entity.HasOne(d => d.WritingSystem).WithMany(p => p.UcdRanges)
        .HasForeignKey(d => d.WritingSystemId);


    });

    modelBuilder.Entity<UcdCodePoint>(entity =>
    {
      entity.HasKey(e => e.Id).HasName("PrimaryKey");

      entity.HasIndex(e => e.CharName, "CharName");
      entity.Property(e => e.Bidir).HasMaxLength(3);
      entity.Property(e => e.Code).HasMaxLength(6);
      entity.Property(e => e.Comment).HasMaxLength(255);
      entity.Property(e => e.Ctg).HasMaxLength(2);
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

      entity.HasIndex(e => new { e.Abbr, e.Ext }, "Abbr").IsUnique();

      entity.HasIndex(e => e.Ctg, "Ctg").IsUnique();

      entity.HasIndex(e => new { e.Name, e.Type }, "FullName").IsUnique();

      entity.HasIndex(e => e.KeyPhrase, "KeyPhrase").IsUnique();

      entity.HasIndex(e => e.ParentId, "ParentID");

      entity.HasIndex(e => e.Kind, "Kind");
      entity.HasIndex(e => e.Type, "Type");

      entity.Property(e => e.Id)
              .HasColumnName("ID");
      entity.Property(e => e.Name).HasMaxLength(255);
      entity.Property(e => e.KeyPhrase).HasMaxLength(255);
      entity.Property(e => e.Abbr).HasMaxLength(255);
      entity.Property(e => e.Ctg).HasMaxLength(2);
      entity.Property(e => e.Ext).HasMaxLength(10);
      entity.Property(e => e.Iso)
              .HasMaxLength(255)
              .HasColumnName("ISO");
      entity.Property(e => e.Kind).HasMaxLength(15);
      entity.Property(e => e.ParentId).HasColumnName("ParentID");
      entity.Property(e => e.Type).HasMaxLength(10);
              
      entity.Property(e => e.Type).HasConversion<byte>();
      entity.Property(e => e.Kind).HasConversion<byte>();

      entity.HasOne(d => d.ParentSystem).WithMany(p => p.Children)
              .HasForeignKey(d => d.ParentId)
              .HasConstraintName("WritingSystemsWritingSystems");
    });

    modelBuilder.Entity<WritingSystemKind>(entity =>
    {
      entity.Property(e => e.Id).HasConversion<byte>();
      entity.HasKey(e => e.Id).HasName("PrimaryKey");
      entity.Property(e => e.Kind).HasMaxLength(15);
      entity.HasIndex(e => e.Kind, "Kind").IsUnique();
    });

    modelBuilder.Entity<WritingSystemType>(entity =>
    {
      entity.Property(e => e.Id).HasConversion<byte>();
      entity.HasKey(e => e.Id).HasName("PrimaryKey");
      entity.Property(e => e.Type).HasMaxLength(10);
      entity.HasIndex(e => e.Type, "Type").IsUnique();
    });


    OnModelCreatingPartial(modelBuilder);
  }

  partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
