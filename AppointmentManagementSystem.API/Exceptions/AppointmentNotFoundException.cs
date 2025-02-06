using System;

namespace AppointmentManagementSystem.API.Exceptions
{
    public class AppointmentNotFoundException : Exception
    {
        public AppointmentNotFoundException()
        {
        }

        public AppointmentNotFoundException(string message)
            : base(message)
        {
        }

        public AppointmentNotFoundException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
