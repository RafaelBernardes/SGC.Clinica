using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Infrastructure.Persistence.Data.Configurations
{
    /// <summary>
    /// Configuração de mapeamento para a entidade TimeSlot
    /// Representa um intervalo de tempo disponível para agendamento (ex: Seg 14:00-14:30)
    /// TimeSlots são derivados de Schedules e permitem reservas granulares
    /// </summary>
    public class TimeSlotConfiguration : IEntityTypeConfiguration<TimeSlot>
    {
        public void Configure(EntityTypeBuilder<TimeSlot> builder)
        {
            // Table
            builder.ToTable("TimeSlots");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties - Obrigatórias
            builder.Property(e => e.ScheduleId)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(e => e.ProfessionalId)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(e => e.StartTime)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(e => e.EndTime)
                .IsRequired()
                .HasColumnType("datetime2");

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

            // Foreign Keys
            builder.HasOne(e => e.Schedule)
                .WithMany()
                .HasForeignKey(e => e.ScheduleId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TimeSlot_Schedule_ScheduleId");

            builder.HasOne(e => e.Professional)
                .WithMany()
                .HasForeignKey(e => e.ProfessionalId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_TimeSlot_Professional_ProfessionalId");

            // Indexes - Importante para buscar slots disponíveis
            builder.HasIndex(e => new { e.ProfessionalId, e.StartTime })
                .HasDatabaseName("IX_TimeSlot_ProfessionalId_StartTime");

            builder.HasIndex(e => new { e.ScheduleId, e.IsAvailable })
                .HasDatabaseName("IX_TimeSlot_ScheduleId_IsAvailable");

            builder.HasIndex(e => e.IsAvailable)
                .HasDatabaseName("IX_TimeSlot_IsAvailable");

            builder.HasIndex(e => e.StartTime)
                .HasDatabaseName("IX_TimeSlot_StartTime");

            builder.HasIndex(e => new { e.ProfessionalId, e.IsAvailable, e.StartTime })
                .HasDatabaseName("IX_TimeSlot_ProfessionalId_IsAvailable_StartTime");
        }
    }
}
