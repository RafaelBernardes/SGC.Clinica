using AutoMapper;
using SGC.Clinica.Api.Application.Patients.Dtos;
using SGC.Clinica.Api.Application.Schedules.Dtos;
using SGC.Clinica.Api.Domain.Models;

namespace SGC.Clinica.Api.Helpers
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<AddPatientDto, Patient>();

            CreateMap<Patient, PatientDto>()
                .ForMember(dest => dest.Age, opt => opt.MapFrom(src => 
                    DateTime.Now.Year - src.DateOfBirth.Year - 
                    (DateTime.Now.DayOfYear < src.DateOfBirth.DayOfYear ? 1 : 0)
                ));
            
            CreateMap<UpdatePatientDto, Patient>();

            // Schedules / Appointments
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.Patient.Name))
                .ForMember(dest => dest.ProfessionalName, opt => opt.MapFrom(src => src.Professional.Name));
        }
    }
}