

namespace AppointmentManagementSystem.API.Models.ResponseModels
{
    public class CreateAppointmentResponseModel
    {
        public long AppointmentId { get; set; }
        
        public string PatientName { get; set; }
    
        public string PatientContactInformation { get; set; }

        public DateTime AppointmentDateAndTime { get; set; }
        public DoctorResponseModel Doctor { get; set; } 
    }
}
