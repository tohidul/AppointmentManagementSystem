using System;

namespace AppointmentManagementSystem.API.Exceptions
{
    public class DoctorNotFoundException : Exception
    {
        public DoctorNotFoundException(string message) : base(message)
        {
        }
    }
}
