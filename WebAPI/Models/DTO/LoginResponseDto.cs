namespace WebAPI.Models.DTO
{
    public class LoginResponseDto
    {
        public string JwtToken { get; set; }
        public string ExpirationTime { get; set; }
    }
}