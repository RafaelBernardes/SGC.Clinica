using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Infrastructure.Persistence.Data.Configurations
{
    /// <summary>
    /// Configuração de mapeamento para a entidade Schedule
    /// Representa o horário semanal de funcionamento de um profissional (ex: Seg 08:00-17:00)
    /// </summary>
    public class ScheduleConfiguration : IEntityTypeConfiguration<Schedule>
    {
        public void Configure(EntityTypeBuilder<Schedule> builder)
        {
            // Table
            builder.ToTable("Schedules");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties - Obrigatórias
            builder.Property(e => e.ProfessionalId)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(e => e.DayOfWeek)
                .IsRequired()
                .HasConversion<int>()
                .HasColumnType("int");

            builder.Property(e => e.StartTime)
                .IsRequired()
                .HasColumnType("time");

            builder.Property(e => e.EndTime)
                .IsRequired()
                .HasColumnType("time");

            builder.Property(e => e.IsAvailable)
                .IsRequired()
                .HasDefaultValue(true)
                .HasColumnType("bit");

            // Audit Properties
            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnType("datetime2");

            builder.Property(e => e.UpdatedAt)
                .HasColumnType("datetime2");

            // Foreign Key
            builder.HasOne(e => e.Professional)
                .WithMany()
                .HasForeignKey(e => e.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_Schedule_Professional_ProfessionalId");

            // Constraint: Um profissional pode ter NO MÁXIMO 1 schedule por dia da semana
            builder.HasIndex(e => new { e.ProfessionalId, e.DayOfWeek })
                .IsUnique()
                .HasDatabaseName("IX_Schedule_ProfessionalId_DayOfWeek_Unique");

            // Indexes
            builder.HasIndex(e => e.ProfessionalId)
                .HasDatabaseName("IX_Schedule_ProfessionalId");

            builder.HasIndex(e => e.IsAvailable)
                .HasDatabaseName("IX_Schedule_IsAvailable");
        }
    }
}
