using AppointmentManagementSystem.API.Data;
using AppointmentManagementSystem.API.Models.Entities;
using AppointmentManagementSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.API.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly AppointmentManagementSystemDbContext _context;

        public DoctorRepository(AppointmentManagementSystemDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Doctor>> GetAllDoctorsAsync()
        {
            return await _context.Doctors.ToListAsync();
        }

        public async Task<Doctor> GetDoctorByIdAsync(long doctorId)
        {
            return await _context.Doctors.FindAsync(doctorId);
        }

        public async Task AddDoctorAsync(Doctor doctor)
        {
            await _context.Doctors.AddAsync(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateDoctorAsync(Doctor doctor)
        {
            _context.Doctors.Update(doctor);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteDoctorAsync(long doctorId)
        {
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor != null)
            {
                _context.Doctors.Remove(doctor);
                await _context.SaveChangesAsync();
            }
        }
    }
}
