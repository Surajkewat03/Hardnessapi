using Hardnessapi.Entities;
using Microsoft.EntityFrameworkCore;

namespace Hardnessapi.Data;

public class HardnessDbContext : DbContext
{
    public HardnessDbContext(DbContextOptions<HardnessDbContext> options) : base(options) { }

    public DbSet<AssemblyHardnessInspection> Coin_QC_Assembly_Hardness_Insp => Set<AssemblyHardnessInspection>();
    public DbSet<AnnealingHardnessInspection> Coin_QC_Annealing_Hardness_Insp => Set<AnnealingHardnessInspection>();
    public DbSet<PolishingHardnessInspection> Coin_QC_Polishing_Hardness_Insp => Set<PolishingHardnessInspection>();
    public DbSet<FgHardnessInspection> Coin_QC_FG_Hardness_Insp => Set<FgHardnessInspection>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Explicit table mapping for PostgreSQL (Neon)
        modelBuilder.Entity<AssemblyHardnessInspection>().ToTable("coin_qc_assembly_hardness_insp");
        modelBuilder.Entity<AnnealingHardnessInspection>().ToTable("coin_qc_annealing_hardness_insp");
        modelBuilder.Entity<PolishingHardnessInspection>().ToTable("coin_qc_polishing_hardness_insp");
        modelBuilder.Entity<FgHardnessInspection>().ToTable("coin_qc_fg_hardness_insp");

        // Force snake_case column names for PostgreSQL/Neon compatibility
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            foreach (var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToLowerInvariant());
            }
        }
    }
}
