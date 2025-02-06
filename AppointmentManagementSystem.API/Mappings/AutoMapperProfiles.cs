using AutoMapper;
using AppointmentManagementSystem.API.Models.Entities;
using AppointmentManagementSystem.API.Models.DTOs;
using AppointmentManagementSystem.API.Models.ViewModels;
using AppointmentManagementSystem.API.Models.ResponseModels;

namespace AppointmentManagementSystem.API.Mappings
{
    public class AutoMapperProfiles: Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<RegisterUserViewModel, UserDto>();
            CreateMap<LoginUserViewModel, UserDto>();
            CreateMap<Doctor, DoctorDto>().ReverseMap();
            CreateMap<DoctorDto, DoctorResponseModel>().ReverseMap();
            CreateMap<CreateDoctorViewModel, DoctorDto>();
            CreateMap<CreateAppointmentViewModel, AppointmentDto>().ReverseMap();
            CreateMap<AppointmentDto, Appointment>()
               .ForMember(dest => dest.Doctor, opt => opt.Ignore());
            CreateMap<Appointment, AppointmentDto>()
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor));
            // Mapping from AppointmentDto to CreateAppointmentResponseModel
            CreateMap<AppointmentDto, CreateAppointmentResponseModel>()
                .ForMember(dest => dest.AppointmentId, opt => opt.MapFrom(src => src.AppointmentId))
                .ForMember(dest => dest.PatientName, opt => opt.MapFrom(src => src.PatientName))
                .ForMember(dest => dest.PatientContactInformation, opt => opt.MapFrom(src => src.PatientContactInformation))
                .ForMember(dest => dest.AppointmentDateAndTime, opt => opt.MapFrom(src => src.AppointmentDateAndTime))
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor));

        }
    }
}
