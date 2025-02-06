namespace AppointmentManagementSystem.API.Models.ResponseModels
{
    public class LoginUserResponseModel
    {
        public string Username { get; set; }
        public string JwtToken { get; set; }
    }
}
