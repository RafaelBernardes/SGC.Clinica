using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SGC.Clinica.Api.Domain.Models;
using SGC.Clinica.Api.Domain.Enums;

namespace SGC.Clinica.Api.Infrastructure.Persistence.Data.Configurations
{
    /// <summary>
    /// Configuração de mapeamento para a entidade Appointment
    /// </summary>
    public class AppointmentConfiguration : IEntityTypeConfiguration<Appointment>
    {
        public void Configure(EntityTypeBuilder<Appointment> builder)
        {
            // Table
            builder.ToTable("Appointments");

            // Primary Key
            builder.HasKey(e => e.Id);

            // Properties - Obrigatórias
            builder.Property(e => e.PatientId)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(e => e.ProfessionalId)
                .IsRequired()
                .HasColumnType("int");

            builder.Property(e => e.ScheduledDate)
                .IsRequired()
                .HasColumnType("datetime2");

            builder.Property(e => e.Duration)
                .IsRequired()
                .HasColumnType("time");

            builder.Property(e => e.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(20)
                .HasColumnType("varchar(20)");

            // Properties - Opcionais
            builder.Property(e => e.Notes)
                .HasMaxLength(500)
                .HasColumnType("nvarchar(500)");

            builder.Property(e => e.CancellationReason)
                .HasMaxLength(500)
                .HasColumnType("nvarchar(500)");

            // Audit Properties
            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()")
                .HasColumnType("datetime2");

            builder.Property(e => e.UpdatedAt)
                .HasColumnType("datetime2");

            // Foreign Keys
            builder.HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Appointment_Patient_PatientId");

            builder.HasOne(e => e.Professional)
                .WithMany()
                .HasForeignKey(e => e.ProfessionalId)
                .OnDelete(DeleteBehavior.Restrict)
                .HasConstraintName("FK_Appointment_Professional_ProfessionalId");

            // Indexes - Importante para queries de agendamentos
            builder.HasIndex(e => new { e.ProfessionalId, e.ScheduledDate })
                .HasDatabaseName("IX_Appointment_ProfessionalId_ScheduledDate");

            builder.HasIndex(e => e.PatientId)
                .HasDatabaseName("IX_Appointment_PatientId");

            builder.HasIndex(e => e.Status)
                .HasDatabaseName("IX_Appointment_Status");

            builder.HasIndex(e => e.ScheduledDate)
                .HasDatabaseName("IX_Appointment_ScheduledDate");

            builder.HasIndex(e => new { e.ProfessionalId, e.Status })
                .HasDatabaseName("IX_Appointment_ProfessionalId_Status");
        }
    }
}
