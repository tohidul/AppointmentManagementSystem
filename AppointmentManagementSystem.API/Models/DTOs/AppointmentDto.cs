

namespace AppointmentManagementSystem.API.Models.DTOs
{
    public class AppointmentDto
    {
        public long AppointmentId { get; set; }
        public string PatientName { get; set; }
        public string PatientContactInformation { get; set; }
        public DateTime AppointmentDateAndTime { get; set; }
        public long DoctorId { get; set; }
        public DoctorDto Doctor { get; set; }
    }
}
