using AutoMapper;
using MediatR;
using SGC.Clinica.Api.Application.Patients.Queries;
using SGC.Clinica.Api.Dtos;
using SGC.Clinica.Api.Models;
using SGC.Clinica.Api.Repositories.Interfaces;

namespace SGC.Clinica.Api.Application.Patients.Handlers
{
    public class GetPatientsListQueryHandler : IRequestHandler<GetPatientsListQuery, IEnumerable<PatientDto>>
    {
        private readonly IBaseRepository<Patient> _baseRepository;
        private readonly IMapper _mapper;

        public GetPatientsListQueryHandler(IBaseRepository<Patient> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PatientDto>> Handle(GetPatientsListQuery request, CancellationToken cancellationToken)
        {
            var patients = await _baseRepository.GetAllAsync(cancellationToken);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }
    }
}