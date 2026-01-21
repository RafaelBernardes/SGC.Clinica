using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Infrastructure.Persistence.Data.Configurations
{
    /// <summary>
    /// Configuração de mapeamento para a entidade Professional (Base)
    /// Utiliza Table Per Hierarchy (TPH) com discriminator para Physioterapist
    /// </summary>
    public class ProfessionalConfiguration : IEntityTypeConfiguration<Professional>
    {
        public void Configure(EntityTypeBuilder<Professional> builder)
        {
            // Table
            builder.ToTable("Professionals");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Table Per Hierarchy (TPH) - Inheritance Strategy
            builder.HasDiscriminator<string>("professional_type")
                .HasValue<Professional>("professional")
                .HasValue<Physioterapist>("physioterapist");

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

            // Boolean property
            builder.Property(e => e.Active)
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
                .HasDatabaseName("IX_Professional_Document_Unique");

            builder.HasIndex(e => e.Active)
                .HasDatabaseName("IX_Professional_Active");

            builder.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_Professional_CreatedAt");
        }
    }
}
