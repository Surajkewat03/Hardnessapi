using Hardnessapi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hardnessapi.Data;

public class HardnessDbContext : DbContext
{
    public HardnessDbContext(DbContextOptions<HardnessDbContext> options) : base(options) { }

    public DbSet<AssemblyHardnessInspection> Coin_QC_Assembly_Hardness_Insp => Set<AssemblyHardnessInspection>();
    public DbSet<AssembalyHardnessInspection> Coin_QC_Assembaly_Hardness_Insp => Set<AssembalyHardnessInspection>();
    public DbSet<PolishingHardnessInspection> Coin_QC_Polishing_Hardness_Insp => Set<PolishingHardnessInspection>();
    public DbSet<FgHardnessInspection> Coin_QC_FG_Hardness_Insp => Set<FgHardnessInspection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AssemblyHardnessInspection>().ToTable("Coin_QC_Assembly_Hardness_Insp");
        modelBuilder.Entity<AssembalyHardnessInspection>().ToTable("Coin_QC_Assembaly_Hardness_Insp");
        modelBuilder.Entity<PolishingHardnessInspection>().ToTable("Coin_QC_Polishing_Hardness_Insp");
        modelBuilder.Entity<FgHardnessInspection>().ToTable("Coin_QC_FG_Hardness_Insp");

        modelBuilder.Entity<AssemblyHardnessInspection>().HasKey(e => new { e.QR_Code, e.MIC, e.Piece_No, e.Inspection_Timestamp });
        modelBuilder.Entity<AssembalyHardnessInspection>().HasKey(e => new { e.QR_Code, e.MIC, e.Piece_No, e.Inspection_Timestamp });
        modelBuilder.Entity<PolishingHardnessInspection>().HasKey(e => new { e.QR_Code, e.MIC, e.Piece_No, e.Inspection_Timestamp });
        modelBuilder.Entity<FgHardnessInspection>().HasKey(e => new { e.QR_Code, e.MIC, e.Piece_No, e.Inspection_Timestamp });
    }
}
