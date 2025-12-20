using AutoMapper;
using MediatR;
using SGC.Clinica.Api.Application.Appointment.Dtos;
using SGC.Clinica.Api.Application.Appointment.Queries;
using SGC.Clinica.Api.Repositories.Interfaces;
using SGC.Clinica.Api.Repositories.Specifications;
using AppointmentModel = SGC.Clinica.Api.Domain.Models.Appointment;

namespace SGC.Clinica.Api.Application.Appointment.Handlers
{
    public class GetAppointmentsByDateQueryHandler : IRequestHandler<GetAppointmentsByDateQuery, IEnumerable<AppointmentDto>>
    {
        private readonly IBaseRepository<AppointmentModel> _baseRepository;
        private readonly IMapper _mapper;

        public GetAppointmentsByDateQueryHandler(IBaseRepository<AppointmentModel> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppointmentDto>> Handle(GetAppointmentsByDateQuery request, CancellationToken cancellationToken)
        {
            var spec = new AppointmentsByDateSpec(request.date, request.professionalId);
            var appointments = await _baseRepository.ListAsync(spec, cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
    }
}