using AutoMapper;
using MediatR;
using SGC.Clinica.Api.Application.Patients.Commands;
using SGC.Clinica.Api.Models;
using SGC.Clinica.Api.Repositories.Interfaces;

namespace SGC.Clinica.Api.Application.Patients.Handlers
{
    public class AddPatientCommandHandler : IRequestHandler<AddPatientCommand, Patient>
    {
        private readonly IBaseRepository<Patient> _baseRepository;

        private readonly IMapper _mapper;

        public AddPatientCommandHandler(IBaseRepository<Patient> baseRepository, IMapper mapper)
        {
            _baseRepository = baseRepository;
            _mapper = mapper;
        }

        public async Task<Patient> Handle(AddPatientCommand request, CancellationToken cancellationToken)
        {
            var patient = _mapper.Map<Patient>(request.PatientDto);
            var newPatient = await _baseRepository.AddAsync(patient, cancellationToken);
            return newPatient;
        }
    }
}