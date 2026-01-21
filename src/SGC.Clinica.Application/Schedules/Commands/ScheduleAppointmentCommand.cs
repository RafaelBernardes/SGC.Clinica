using MediatR;
using SGC.Clinica.Api.Application.Schedules.Dtos;

﻿namespace SGC.Clinica.Api.Application.Schedules.Commands
{
    public record ScheduleAppointmentCommand(
        int PatientId,
        int ProfessionalId,
        DateTime ScheduledAt,
        int DurationInMinutes,
        string Reason) : IRequest<AppointmentDto>;
}
