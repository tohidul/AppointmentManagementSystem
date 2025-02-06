using AppointmentManagementSystem.API.Data;
using AppointmentManagementSystem.API.Models.Entities;
using AppointmentManagementSystem.API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AppointmentManagementSystem.API.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly AppointmentManagementSystemDbContext _context;

        public AppointmentRepository(AppointmentManagementSystemDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            return await _context.Appointments.Include(a => a.Doctor).ToListAsync();
        }

        public async Task<Appointment> GetAppointmentByIdAsync(long appointmentId)
        {
            return await _context.Appointments.Include(a => a.Doctor).FirstOrDefaultAsync(a => a.AppointmentId == appointmentId);
        }

        public async Task AddAppointmentAsync(Appointment appointment)
        {
            await _context.Appointments.AddAsync(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAppointmentAsync(Appointment appointment)
        {
            _context.Appointments.Update(appointment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAppointmentAsync(long appointmentId)
        {
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment != null)
            {
                _context.Appointments.Remove(appointment);
                await _context.SaveChangesAsync();
            }
        }
        
    }
}

