using AutoMapper;
using MediatR;
using SGC.Clinica.Application.Schedules.Commands;
using SGC.Clinica.Application.Schedules.Dtos;
using SGC.Clinica.Application.Abstractions.Persistence;
using SGC.Clinica.Domain.Events.Appointments;
using SGC.Clinica.Application.Abstractions.Persistence.Repositories;
using AppointmentModel = SGC.Clinica.Domain.Models.Appointment;


namespace SGC.Clinica.Application.Schedules.Handlers
{
    public class MarkAppointmentAsNoShowCommandHandler : IRequestHandler<MarkAppointmentAsNoShowCommand, AppointmentDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBaseRepository<AppointmentModel> _appointmentRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public MarkAppointmentAsNoShowCommandHandler(IApplicationDbContext context, IBaseRepository<AppointmentModel> appointmentRepository, IMapper mapper, IMediator mediator)
        {
            _context = context;
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<AppointmentDto> Handle(MarkAppointmentAsNoShowCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

            appointment!.MarkAsNoShow();

            await _context.SaveChangesAsync(cancellationToken);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);

            await _mediator.Publish(new AppointmentNoShowEvent(appointmentDto), cancellationToken);

            return appointmentDto;
        }
    }
}

