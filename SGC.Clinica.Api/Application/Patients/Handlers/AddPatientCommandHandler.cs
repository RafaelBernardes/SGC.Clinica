using AutoMapper;
using MediatR;
using SGC.Clinica.Api.Application.Patients.Commands;
using SGC.Clinica.Api.Data.Interfaces;
using SGC.Clinica.Api.Domain.Events;
using SGC.Clinica.Api.Models;
using SGC.Clinica.Api.Repositories.Interfaces;

namespace SGC.Clinica.Api.Application.Patients.Handlers
{
    public class AddPatientCommandHandler : IRequestHandler<AddPatientCommand, Patient>
    {
        private readonly IBaseRepository<Patient> _patientRepository;
        private readonly IMapper _mapper;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public AddPatientCommandHandler(IBaseRepository<Patient> baseRepository, IMapper mapper, IApplicationDbContext context, IMediator mediator)
        {
            _patientRepository = baseRepository;
            _mapper = mapper;
            _context = context;
            _mediator = mediator;
        }

        public async Task<Patient> Handle(AddPatientCommand request, CancellationToken cancellationToken)
        {
            var patient = _mapper.Map<Patient>(request.PatientDto);
            
            await _patientRepository.AddAsync(patient, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await _mediator.Publish(new PatientCreatedEvent(patient), cancellationToken);

            return patient;
        }
    }
}