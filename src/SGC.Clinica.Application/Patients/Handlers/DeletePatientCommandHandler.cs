using MediatR;
using SGC.Clinica.Application.Patients.Commands;
using SGC.Clinica.Application.Abstractions.Persistence;
using SGC.Clinica.Domain.Events;
using SGC.Clinica.Domain.Events.Patients;
using SGC.Clinica.Domain.Models;
using SGC.Clinica.Application.Abstractions.Persistence.Repositories;

namespace SGC.Clinica.Application.Patients.Handlers
{
    public class DeletePatientCommandHandler : IRequestHandler<DeletePatientCommand, Unit>
    {
        private readonly IBaseRepository<Patient> _patientRepository;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public DeletePatientCommandHandler(IBaseRepository<Patient> patientRepository, IApplicationDbContext context, IMediator mediator)
        {
            _patientRepository = patientRepository;
            _context = context;
            _mediator = mediator;
        }

        public async Task<Unit> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
        {
            await _patientRepository.DeleteAsync(request.PatientId, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            await _mediator.Publish(new PatientDeletedEvent(request.PatientId), cancellationToken);

            return Unit.Value;
        }
    }
}
