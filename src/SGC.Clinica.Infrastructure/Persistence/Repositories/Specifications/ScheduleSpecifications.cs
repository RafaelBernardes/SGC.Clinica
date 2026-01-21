using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Infrastructure.Persistence.Repositories.Specifications
{
    public class CheckAvailabilitySpecification : Specification<Schedule>
    {
        public CheckAvailabilitySpecification(int professionalId, DayOfWeek scheduledDate, TimeSpan startTime, TimeSpan endTime)
            : base(s => s.ProfessionalId == professionalId && s.DayOfWeek == scheduledDate && s.StartTime <= startTime && s.EndTime >= endTime)
        {
            AddInclude(s => s.Professional);
        }
    }
}