using AppointmentManagementSystem.API.Models.DTOs;
using AppointmentManagementSystem.API.Models.Entities;
using AppointmentManagementSystem.API.Repositories.Interfaces;
using AppointmentManagementSystem.API.Services.Interfaces;
using AutoMapper;
using AppointmentManagementSystem.API.Exceptions;

namespace AppointmentManagementSystem.API.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;
        private readonly IDoctorRepository _doctorRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository, IMapper mapper, IDoctorRepository doctorRepository)
        {
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
            _doctorRepository = doctorRepository;
        }

        public async Task<AppointmentDto> CreateAppointmentAsync(AppointmentDto appointmentDto)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(appointmentDto.DoctorId);
            if (doctor == null)
            {
                throw new DoctorNotFoundException($"Doctor with ID {appointmentDto.DoctorId} not found.");
            }
            var doctorDto = _mapper.Map<DoctorDto>(doctor);
            appointmentDto.Doctor = doctorDto;

            var appointment = _mapper.Map<Appointment>(appointmentDto);
            await _appointmentRepository.AddAppointmentAsync(appointment);
            var createdAppointmentDto = _mapper.Map<AppointmentDto>(appointment);
            return createdAppointmentDto;
        }

        public async Task<IEnumerable<AppointmentDto>> GetAllAppointmentsAsync()
        {
            var appointments = await _appointmentRepository.GetAllAppointmentsAsync();
            return _mapper.Map<IEnumerable<AppointmentDto>>(appointments);
        }

        public async Task<AppointmentDto> GetAppointmentByIdAsync(long appointmentId)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            return _mapper.Map<AppointmentDto>(appointment);
        }

        public async Task<AppointmentDto> UpdateAppointmentByIdAsync(long appointmentId, AppointmentDto appointmentDto)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new AppointmentNotFoundException($"Appointment with ID {appointmentId} not found.");
            }

            var doctor = await _doctorRepository.GetDoctorByIdAsync(appointmentDto.DoctorId);
            if (doctor == null)
            {
                throw new DoctorNotFoundException($"Doctor with ID {appointmentDto.DoctorId} not found.");
            }

            appointment.PatientName = appointmentDto.PatientName;
            appointment.PatientContactInformation = appointmentDto.PatientContactInformation;
            appointment.AppointmentDateAndTime = appointmentDto.AppointmentDateAndTime;
            appointment.DoctorId = appointmentDto.DoctorId;

            await _appointmentRepository.UpdateAppointmentAsync(appointment);
            var updatedAppointment = _mapper.Map<AppointmentDto>(appointment);
            return updatedAppointment;
        }

        public async Task DeleteAppointmentByIdAsync(long appointmentId)
        {
            var appointment = await _appointmentRepository.GetAppointmentByIdAsync(appointmentId);
            if (appointment == null)
            {
                throw new AppointmentNotFoundException($"Appointment with ID {appointmentId} not found.");
            }
            await _appointmentRepository.DeleteAppointmentAsync(appointmentId);

        }
    }
}

