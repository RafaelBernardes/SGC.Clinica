using AutoMapper;
using MediatR;
using SGC.Clinica.Application.Schedules.Commands;
using SGC.Clinica.Application.Schedules.Dtos;
using SGC.Clinica.Application.Abstractions.Persistence.Repositories;
using SGC.Clinica.Application.Abstractions.Persistence;
using AppointmentModel = SGC.Clinica.Domain.Models.Appointment;
using SGC.Clinica.Domain.Events.Appointments;


namespace SGC.Clinica.Application.Schedules.Handlers
{
    public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, AppointmentDto>
    {
        private readonly IBaseRepository<AppointmentModel> _appointmentRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly IApplicationDbContext _context;

        public CancelAppointmentCommandHandler(IBaseRepository<AppointmentModel> appointmentRepository, IMapper mapper, IMediator mediator, IApplicationDbContext context)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
            _mediator = mediator;
            _context = context;
        }

        public async Task<AppointmentDto> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

            appointment!.Cancel(request.Reason);

            await _appointmentRepository.UpdateAsync(appointment, cancellationToken);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);

            await _mediator.Publish(new AppointmentCancelledEvent(appointmentDto), cancellationToken);

            return appointmentDto;
        }
    }
}

