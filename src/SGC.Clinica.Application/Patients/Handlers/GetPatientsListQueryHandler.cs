using AutoMapper;
using MediatR;
using SGC.Clinica.Application.Patients.Dtos;
using SGC.Clinica.Application.Patients.Queries;
using SGC.Clinica.Domain.Models;
using SGC.Clinica.Application.Abstractions.Persistence.Repositories;

namespace SGC.Clinica.Application.Patients.Handlers
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
