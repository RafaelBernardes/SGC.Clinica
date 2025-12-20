using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Repositories.Specifications
{
    public class AppointmentsByPatientIdSpec : Specification<Appointment>
    {
        public AppointmentsByPatientIdSpec(int patientId) 
            : base(a => a.PatientId == patientId)
        {
            AddInclude(a => a.Patient);
            AddInclude(a => a.Professional);
            AddOrderBy(a => a.ScheduledDate);
        }
    }
    
    public class AppointmentsByProfessionalIdSpec : Specification<Appointment>
    {
        public AppointmentsByProfessionalIdSpec(int professionalId, DateTime? startDate, DateTime? endDate)
            : base(a => a.ProfessionalId == professionalId && 
                        (!startDate.HasValue || a.ScheduledDate >= startDate.Value) &&
                        (!endDate.HasValue || a.ScheduledDate <= endDate.Value))
        {
            AddInclude(a => a.Patient);
            AddInclude(a => a.Professional);
            AddOrderBy(a => a.ScheduledDate);
        }
    }

    public class AppointmentsByDateSpec : Specification<Appointment>
    {
        public AppointmentsByDateSpec(DateTime date, int? professionalId = null)
            : base(a => a.ScheduledDate.Date == date.Date && 
                        (!professionalId.HasValue || a.ProfessionalId == professionalId.Value))
        {
            AddInclude(a => a.Patient);
            AddInclude(a => a.Professional);
            AddOrderBy(a => a.ScheduledDate);
        }
    }
}
