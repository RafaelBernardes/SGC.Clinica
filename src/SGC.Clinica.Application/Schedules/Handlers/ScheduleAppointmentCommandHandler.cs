using AutoMapper;
using MediatR;
using SGC.Clinica.Application.Schedules.Commands;
using SGC.Clinica.Application.Schedules.Dtos;
using SGC.Clinica.Domain.Events.Appointments;
using SGC.Clinica.Application.Abstractions.Persistence;
using SGC.Clinica.Application.Abstractions.Persistence.Repositories;
using AppointmentModel = SGC.Clinica.Domain.Models.Appointment;

namespace SGC.Clinica.Application.Schedules.Handlers
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

