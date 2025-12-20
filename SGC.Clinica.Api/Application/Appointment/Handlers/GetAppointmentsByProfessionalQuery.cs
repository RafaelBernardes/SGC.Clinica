using AutoMapper;
using MediatR;
using SGC.Clinica.Api.Application.Appointment.Dtos;
using SGC.Clinica.Api.Application.Appointment.Queries;
using SGC.Clinica.Api.Repositories.Interfaces;
using SGC.Clinica.Api.Repositories.Specifications;
using AppointmentModel = SGC.Clinica.Api.Domain.Models.Appointment;

namespace SGC.Clinica.Api.Application.Appointment.Handlers
{
    public class GetAppointmentsByProfessionalQueryHandler : IRequestHandler<GetAppointmentsByProfessionalQuery, IEnumerable<AppointmentDto>>
    {
        private readonly IBaseRepository<AppointmentModel> _baseRepository;
        private readonly IMapper _mapper;

        public GetAppointmentsByProfessionalQueryHandler(IBaseRepository<AppointmentModel> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }
        
        public async Task<IEnumerable<AppointmentDto>> Handle(GetAppointmentsByProfessionalQuery request, CancellationToken cancellationToken)
        {
            var appointments = await _baseRepository.ListAsync(new AppointmentsByProfessionalIdSpec(request.professionalId, request.startDate, request.endDate), cancellationToken);
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }
    }

}