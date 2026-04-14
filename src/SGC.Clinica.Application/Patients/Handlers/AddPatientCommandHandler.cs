using MediatR;
using SGC.Clinica.Application.Patients.Commands;
using SGC.Clinica.Application.Abstractions.Persistence;
using SGC.Clinica.Domain.Events.Patients;
using SGC.Clinica.Domain.Models;
using SGC.Clinica.Application.Abstractions.Persistence.Repositories;

namespace SGC.Clinica.Application.Patients.Handlers
{
    public class AddPatientCommandHandler : IRequestHandler<AddPatientCommand, Patient>
    {
        private readonly IBaseRepository<Patient> _patientRepository;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public AddPatientCommandHandler(IBaseRepository<Patient> baseRepository, IApplicationDbContext context, IMediator mediator)
        {
            _patientRepository = baseRepository;
            _context = context;
            _mediator = mediator;
        }

        public async Task<Patient> Handle(AddPatientCommand request, CancellationToken cancellationToken)
        {
            var patient = Patient.Create(request.PatientDto.Name, request.PatientDto.DateOfBirth, request.PatientDto.Phone, request.PatientDto.Email, request.PatientDto.Occupation, request.PatientDto.Observations);
            
            await _patientRepository.AddAsync(patient, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await _mediator.Publish(new PatientCreatedEvent(patient), cancellationToken);

            return patient;
        }
    }
}
