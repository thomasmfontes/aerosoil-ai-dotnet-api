using AeroSoilAI.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace AeroSoilAI.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Propriedade> Propriedades => Set<Propriedade>();

    public DbSet<Sensor> Sensores => Set<Sensor>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Propriedade>(entity =>
        {
            entity.ToTable("TB_PROPRIEDADE");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id)
                .HasColumnName("ID_PROPRIEDADE")
                .ValueGeneratedOnAdd();

            entity.Property(p => p.Nome)
                .HasColumnName("NM_PROPRIEDADE")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(p => p.Localizacao)
                .HasColumnName("DS_LOCALIZACAO")
                .HasMaxLength(200)
                .IsRequired();

            entity.Property(p => p.Hectares)
                .HasColumnName("NR_HECTARES")
                .HasPrecision(10, 2)
                .IsRequired();

            entity.HasMany(p => p.Sensores)
                .WithOne(s => s.Propriedade)
                .HasForeignKey(s => s.PropriedadeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Sensor>(entity =>
        {
            entity.ToTable("TB_SENSOR");

            entity.HasKey(s => s.Id);

            entity.Property(s => s.Id)
                .HasColumnName("ID_SENSOR")
                .ValueGeneratedOnAdd();

            entity.Property(s => s.Tipo)
                .HasColumnName("TP_SENSOR")
                .HasConversion<string>()
                .HasColumnType("VARCHAR2(20)")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(s => s.UltimaLeitura)
                .HasColumnName("VL_ULTIMA_LEITURA")
                .HasPrecision(10, 2)
                .IsRequired();

            entity.Property(s => s.DataAtualizacao)
                .HasColumnName("DT_ATUALIZACAO")
                .IsRequired();

            entity.Property(s => s.PropriedadeId)
                .HasColumnName("ID_PROPRIEDADE")
                .IsRequired();
        });
    }
}