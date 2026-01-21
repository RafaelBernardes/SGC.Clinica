using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Infrastructure.Persistence.Data.Configurations
{
    /// <summary>
    /// Configuração de mapeamento para a entidade Patient
    /// </summary>
    public class PatientConfiguration : IEntityTypeConfiguration<Patient>
    {
        public void Configure(EntityTypeBuilder<Patient> builder)
        {
            // Table
            builder.ToTable("Patients");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties - Obrigatórias
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(150)
                .HasColumnType("nvarchar(150)");

            builder.Property(e => e.Document)
                .IsRequired()
                .HasMaxLength(14)
                .IsUnicode(false)
                .HasColumnType("varchar(14)");

            builder.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnType("varchar(20)");

            builder.Property(e => e.DateOfBirth)
                .IsRequired()
                .HasColumnType("datetime2");

            // Properties - Opcionais
            builder.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnType("varchar(100)");

            builder.Property(e => e.Occupation)
                .HasMaxLength(100)
                .HasColumnType("nvarchar(100)");

            builder.Property(e => e.Observations)
                .HasMaxLength(500)
                .HasColumnType("nvarchar(500)");

            // Boolean properties
            builder.Property(e => e.Active)
                .IsRequired()
                .HasDefaultValue(true)
                .HasColumnType("bit");

            builder.Property(e => e.HasWhatsAppOptIn)
                .IsRequired()
                .HasDefaultValue(true)
                .HasColumnType("bit");

            builder.Property(e => e.HasSmsOptIn)
                .IsRequired()
                .HasDefaultValue(true)
                .HasColumnType("bit");

            builder.Property(e => e.HasEmailOptIn)
                .IsRequired()
                .HasDefaultValue(true)
                .HasColumnType("bit");

            // Audit Properties
            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnType("datetime2");

            builder.Property(e => e.UpdatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnType("datetime2");

            // Indexes
            builder.HasIndex(e => e.Document)
                .IsUnique()
                .HasDatabaseName("IX_Patient_Document_Unique");

            builder.HasIndex(e => e.Email)
                .HasDatabaseName("IX_Patient_Email");

            builder.HasIndex(e => e.Active)
                .HasDatabaseName("IX_Patient_Active");

            builder.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_Patient_CreatedAt");
        }
    }
}