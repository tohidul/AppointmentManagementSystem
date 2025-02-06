using System.ComponentModel.DataAnnotations;

namespace AppointmentManagementSystem.API.Attributes
{
    public class FutureDateAttribute : ValidationAttribute
    {
        public override bool IsValid(object value)
        {
            if (value is DateTime dateTime)
            {
                return dateTime > DateTime.Now;
            }
            return false;
        }

        public override string FormatErrorMessage(string name)
        {
            return $"{name} must be a future date.";
        }
    }
}
