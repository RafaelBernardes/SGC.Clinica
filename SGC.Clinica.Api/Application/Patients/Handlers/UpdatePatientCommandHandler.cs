using AutoMapper;
using MediatR;
using SGC.Clinica.Api.Data.Interfaces;
using SGC.Clinica.Api.Domain.Events;
using SGC.Clinica.Api.Models;
using SGC.Clinica.Api.Repositories.Interfaces;

namespace SGC.Clinica.Api.Application.Patients.Handlers
{
    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, Patient>
    {
        private readonly IBaseRepository<Patient> _patientRepository;
        private readonly IApplicationDbContext _context;
        private readonly IMediator _mediator;

        public UpdatePatientCommandHandler(IBaseRepository<Patient> patientRepository, IApplicationDbContext context, IMediator mediator)
        {
            _patientRepository = patientRepository;
            _context = context;
            _mediator = mediator;
        }

        public async Task<Patient> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var patientToUpdate = await _patientRepository.GetByIdAsync(request.Id);

            if(patientToUpdate == null)
               throw new KeyNotFoundException("Patient not found");

            patientToUpdate.Update(request.PatientDto.Name, request.PatientDto.DateOfBirth);
            
            await  _context.SaveChangesAsync(cancellationToken);
            await _mediator.Publish(new PatientUpdatedEvent(patientToUpdate), cancellationToken);

            return patientToUpdate;
        }
    }
}