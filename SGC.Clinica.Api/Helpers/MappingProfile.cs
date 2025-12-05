namespace SGc.Clinica.Api.Helpers
{
    using AutoMapper;
    using SGC.Clinica.Api.Dtos;
    using SGC.Clinica.Api.Models;

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
        }
    }
}