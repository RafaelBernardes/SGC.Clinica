using AutoMapper;
using MediatR;
using SGC.Clinica.Api.Application.Schedules.Commands;
using SGC.Clinica.Api.Application.Schedules.Dtos;
using SGC.Clinica.Api.Data.Interfaces;
using SGC.Clinica.Api.Domain.Events.Appointments;
using SGC.Clinica.Api.Repositories.Interfaces;
using AppointmentModel = SGC.Clinica.Api.Domain.Models.Appointment;

namespace SGC.Clinica.Api.Application.Schedules.Handlers
{
    public class ScheduleAppointmentCommandHandler : IRequestHandler<ScheduleAppointmentCommand, AppointmentDto>
    {
        private readonly IBaseRepository<AppointmentModel> _appointmentRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;

        public ScheduleAppointmentCommandHandler(IBaseRepository<AppointmentModel> appointmentRepository, IMapper mapper, IMediator mediator, IApplicationDbContext context)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
            _mediator = mediator;
            _context = context;
        }

        public async Task<AppointmentDto> Handle(ScheduleAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointmentDuration = TimeSpan.FromMinutes(request.DurationInMinutes);
            var newAppointment = AppointmentModel.Create(
                request.PatientId,
                request.ProfessionalId,
                request.ScheduledAt,
                appointmentDuration,
                request.Reason
            );

            var createdAppointment = await _appointmentRepository.AddAsync(newAppointment, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken); 
                       
            var appointmentDto = _mapper.Map<AppointmentDto>(createdAppointment);
            
            await _mediator.Publish(new AppointmentScheduledEvent(appointmentDto), cancellationToken);

            return appointmentDto;
        }
    }
}
