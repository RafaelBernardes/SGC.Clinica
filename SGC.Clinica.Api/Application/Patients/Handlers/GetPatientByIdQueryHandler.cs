using AutoMapper;
using MediatR;
using SGC.Clinica.Api.Application.Patients.Queries;
using SGC.Clinica.Api.Dtos;
using SGC.Clinica.Api.Models;
using SGC.Clinica.Api.Repositories.Interfaces;

namespace SGC.Clinica.Api.Application.Patients.Handlers
{
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDto?>
    {
        private readonly IBaseRepository<Patient> _baseRepository;
        private readonly IMapper _mapper;

        public GetPatientByIdQueryHandler(IBaseRepository<Patient> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<PatientDto?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            var patient = await _baseRepository.GetByIdAsync(request.Id, cancellationToken);
            return patient is null ? null : _mapper.Map<PatientDto>(patient);
        }
    }
}