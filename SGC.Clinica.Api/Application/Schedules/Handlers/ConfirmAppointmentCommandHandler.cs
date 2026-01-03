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
    public class ConfirmAppointmentCommandHandler : IRequestHandler<ConfirmAppointmentCommand, AppointmentDto>
    {
        private readonly IApplicationDbContext _context;
        private readonly IBaseRepository<AppointmentModel> _appointmentRepository;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ConfirmAppointmentCommandHandler(IApplicationDbContext context, IBaseRepository<AppointmentModel> appointmentRepository, IMapper mapper, IMediator mediator)
        {
            _context = context;
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
            _mediator = mediator;
        }

        public async Task<AppointmentDto> Handle(ConfirmAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);

            appointment!.Confirm();

            await _context.SaveChangesAsync(cancellationToken);

            var appointmentDto = _mapper.Map<AppointmentDto>(appointment);

            await _mediator.Publish(new AppointmentConfirmedEvent(appointmentDto), cancellationToken);

            return appointmentDto;
        }
    }
}
