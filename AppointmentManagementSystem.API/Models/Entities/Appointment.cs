using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.API.Models.Entities
{
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long AppointmentId { get; set; }

        [Required]
        [MaxLength(200)]
        public string PatientName { get; set; }

        [Required]
        [MaxLength(100)]
        public string PatientContactInformation { get; set; }

        [Required]
        public DateTime AppointmentDateAndTime { get; set; }

        [ForeignKey("Doctor")]
        public long DoctorId { get; set; }

        public Doctor Doctor { get; set; }
    }
}
