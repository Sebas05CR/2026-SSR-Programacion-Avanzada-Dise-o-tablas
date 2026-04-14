using Microsoft.EntityFrameworkCore;
using PRQ.Models;

namespace PRQ.Data;

public class PRQDbContext : DbContext
{
    public DbSet<Automovil> Automoviles { get; set; } = null!;
    public DbSet<Parqueo>   Parqueos    { get; set; } = null!;
    public DbSet<Ingreso>   Ingresos    { get; set; } = null!;

    public PRQDbContext(DbContextOptions<PRQDbContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // ── PRQ_Automoviles ────────────────────────────────────────
        modelBuilder.Entity<Automovil>(entity =>
        {
            entity.ToTable("PRQ_Automoviles");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Fabricante).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Color).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Tipo).IsRequired().HasMaxLength(20);
        });

        // ── PRQ_Parqueos ───────────────────────────────────────────
        modelBuilder.Entity<Parqueo>(entity =>
        {
            entity.ToTable("PRQ_Parqueos");
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Nombre).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Provincia).IsRequired().HasMaxLength(100);
            entity.Property(e => e.PrecioPorHora).HasColumnType("decimal(10,2)");
        });

        // ── PRQ_Ingresos ───────────────────────────────────────────
        modelBuilder.Entity<Ingreso>(entity =>
        {
            entity.ToTable("PRQ_Ingresos");
            entity.HasKey(e => e.Consecutivo);

            entity.HasOne(e => e.Parqueo)
                  .WithMany(p => p.Ingresos)
                  .HasForeignKey(e => e.ParqueoId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.Automovil)
                  .WithMany(a => a.Ingresos)
                  .HasForeignKey(e => e.AutomovilId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.Ignore(e => e.DuracionMinutos);
            entity.Ignore(e => e.DuracionHoras);
            entity.Ignore(e => e.MontoTotal);
        });
    }
}
