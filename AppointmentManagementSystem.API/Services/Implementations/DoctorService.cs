using AppointmentManagementSystem.API.Exceptions;
using AppointmentManagementSystem.API.Models.DTOs;
using AppointmentManagementSystem.API.Models.Entities;
using AppointmentManagementSystem.API.Repositories.Interfaces;
using AppointmentManagementSystem.API.Services.Interfaces;
using AutoMapper;


namespace AppointmentManagementSystem.API.Services
{
    public class DoctorService : IDoctorService
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IMapper _mapper;

        public DoctorService(IDoctorRepository doctorRepository, IMapper mapper)
        {
            _doctorRepository = doctorRepository;
            _mapper = mapper;
        }

        public async Task<(long DoctorId, string DoctorName)> CreateDoctorAsync(DoctorDto doctorDto)
        {
            var doctor = _mapper.Map<Doctor>(doctorDto);
            await _doctorRepository.AddDoctorAsync(doctor);
            return (doctor.DoctorId, doctor.DoctorName);
        }

        public async Task<IEnumerable<DoctorDto>> GetAllDoctorsAsync()
        {
            var doctors = await _doctorRepository.GetAllDoctorsAsync();
            return _mapper.Map<IEnumerable<DoctorDto>>(doctors);

        }

        public async Task<Doctor> GetDoctorByIdAsync(long doctorId)
        {
            return await _doctorRepository.GetDoctorByIdAsync(doctorId);
        }

        public async Task<DoctorDto> UpdateDoctorByIdAsync(long doctorId, DoctorDto doctorDto)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
            {
                throw new DoctorNotFoundException($"Doctor with ID {doctorId} not found.");
            }

            doctor.DoctorName = doctorDto.DoctorName;
            await _doctorRepository.UpdateDoctorAsync(doctor);
            doctorDto = _mapper.Map<DoctorDto>(doctor);
            return doctorDto;

        }

        public async Task DeleteDoctorByIdAsync(long doctorId)
        {
            var doctor = await _doctorRepository.GetDoctorByIdAsync(doctorId);
            if (doctor == null)
            {
                throw new DoctorNotFoundException($"Doctor with ID {doctorId} not found.");
            }

            await _doctorRepository.DeleteDoctorAsync(doctorId);

        }

    }
}
