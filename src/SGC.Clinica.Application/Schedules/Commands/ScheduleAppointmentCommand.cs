using MediatR;
using SGC.Clinica.Application.Schedules.Dtos;

﻿namespace SGC.Clinica.Application.Schedules.Commands
{
    public record ScheduleAppointmentCommand(
        int PatientId,
        int ProfessionalId,
        DateTime ScheduledAt,
        int DurationInMinutes,
        string Reason) : IRequest<AppointmentDto>;
}

