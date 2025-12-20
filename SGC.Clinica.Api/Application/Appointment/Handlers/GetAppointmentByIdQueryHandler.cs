using AutoMapper;
using MediatR;
using SGC.Clinica.Api.Application.Appointment.Dtos;
using SGC.Clinica.Api.Application.Appointment.Queries;
using SGC.Clinica.Api.Repositories.Interfaces;
using AppointmentModel = SGC.Clinica.Api.Domain.Models.Appointment;

namespace SGC.Clinica.Api.Application.Appointments.Handlers
{
    public class GetAppointmentByIdQueryHandler : IRequestHandler<GetAppointmentByIdQuery, AppointmentDto?>
    {
        private readonly IBaseRepository<AppointmentModel> _baseRepository;
        private readonly IMapper _mapper;

        public GetAppointmentByIdQueryHandler(IBaseRepository<AppointmentModel> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<AppointmentDto?> Handle(GetAppointmentByIdQuery request, CancellationToken cancellationToken)
        {
            var appointment = await _baseRepository.GetByIdAsync(request.Id, cancellationToken);
            return appointment is null ? null : _mapper.Map<AppointmentDto>(appointment);
        }
    }
}