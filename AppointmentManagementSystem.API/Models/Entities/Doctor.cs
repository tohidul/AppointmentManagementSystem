using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.API.Models.Entities
{
    public class Doctor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long DoctorId { get; set; }

        [Required]
        [MaxLength(200)]
        public string DoctorName { get; set; }

        public ICollection<Appointment> Appointments { get; set; }
    }
}
