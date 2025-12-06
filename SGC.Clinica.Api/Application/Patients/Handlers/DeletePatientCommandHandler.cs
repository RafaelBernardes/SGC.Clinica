using MediatR;
using SGC.Clinica.Api.Application.Patients.Commands;
using SGC.Clinica.Api.Data.Interfaces;
using SGC.Clinica.Api.Domain.Events;
using SGC.Clinica.Api.Models;
using SGC.Clinica.Api.Repositories.Interfaces;

namespace SGC.Clinica.Api.Application.Patients.Handlers
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