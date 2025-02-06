using System.ComponentModel.DataAnnotations;
using AppointmentManagementSystem.API.Attributes;

namespace AppointmentManagementSystem.API.Models.ViewModels
{
    public class CreateAppointmentViewModel
    {
        [Required]
        public string PatientName { get; set; }

        [Required]
        public string PatientContactInformation { get; set; }
        [Required]
        [FutureDate(ErrorMessage = "Appointment date must be a future date.")]
        public DateTime AppointmentDateAndTime { get; set; }
        [Required]
        public long DoctorId { get; set; }

    }
}
